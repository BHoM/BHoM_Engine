/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System.ComponentModel;
using BH.oM.Geometry;
using BH.oM.Security.Elements;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using BH.Engine.Geometry;
using System;
using System.Linq;

namespace BH.Engine.Security
{
    public static partial class Compute
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Returns the PolyCurve that represents security camera field of view.")]
        [Input("cameraDevice", "CameraDevice object to compute the field of view for.")]
        [Input("obstacles", "Polyline objects that represents obstacles for camera vision (walls, columns, stairs).")]
        [Input("distanceTolerance", "Distance tolerance for the method.")]
        [Input("angleTolerance", "Angular tolerance for the method.")]
        [Output("fieldOfView", "PolyCurve object that represents security camera field of view.")]
        public static PolyCurve CameraFieldOfView(this CameraDevice cameraDevice, List<Polyline> obstacles, double distanceTolerance = BH.oM.Geometry.Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            if (cameraDevice == null || obstacles == null)
                return null;

            PolyCurve cameraCone = cameraDevice.ViewCone();
            Polyline cameraConePolyline = cameraCone.CollapseToPolyline(angleTolerance);
            Point cameraLocation = cameraDevice.EyePosition;
            Point targetLocation = cameraDevice.TargetPosition;
            double radius = targetLocation.Distance(cameraLocation);
            Arc coneArc = cameraCone.Curves[1] as Arc;
            Plane cameraPlane = BH.Engine.Geometry.Create.Plane(cameraLocation, Vector.ZAxis);

            //cone points
            List<Point> conePoints = cameraConePolyline.ControlPoints();

            //add cone boundary points as intersectPoints
            List<Point> intersectPoints = new List<Point>();
            intersectPoints.Add(conePoints[1]);
            intersectPoints.Add(conePoints[conePoints.Count - 2]);

            //simplify obstacles
            List<Polyline> projObstacles = obstacles.Select(x => x.Project(cameraPlane, distanceTolerance)).ToList();

            //points that intersect with obstacles
            List<Polyline> intersectObstacles = new List<Polyline>();
            foreach (Polyline obstacle in projObstacles)
            {
                if (obstacle.IsContaining(new List<Point> { cameraLocation }, true, distanceTolerance))
                {
                    Base.Compute.RecordWarning("Camera Device is inside obstacle that will be skipped.");
                    continue;
                }

                foreach (Line obstLine in obstacle.SubParts())
                {
                    List<Point> coneArcIntersection = coneArc.CurveIntersections(obstLine, distanceTolerance);
                    intersectPoints.AddRange(coneArcIntersection);
                }

                List<Polyline> intersection = obstacle.BooleanIntersection(cameraConePolyline, distanceTolerance);
                if (intersection.Count != 0)
                {
                    intersectObstacles.Add(obstacle);
                    foreach (Polyline intersectPolyline in intersection)
                    {
                        List<Point> points = intersectPolyline.ControlPoints;
                        intersectPoints.AddRange(points);
                    }
                }
            }

            intersectPoints = intersectPoints.CullDuplicates(distanceTolerance);

            //collect endpoints of extended ray lines
            List<Point> endPoints = new List<Point>();
            foreach (Point point in intersectPoints)
            {
                Line line = BH.Engine.Geometry.Create.Line(cameraLocation, point);
                Line rayLine = line.Extend(0, radius - line.Length(), false, distanceTolerance);
                endPoints.Add(rayLine.End);
            }

            //cull endpoints
            endPoints = endPoints.CullDuplicates(distanceTolerance);

            //create and sort extended lines
            List<Line> rayLines = new List<Line>();
            foreach (Point point in endPoints)
            {
                Line line = BH.Engine.Geometry.Create.Line(cameraLocation, point);
                rayLines.Add(line);
            }
            rayLines = rayLines.OrderBy(x => x.SignedAngle(rayLines[0], Vector.ZAxis)).ToList();

            //split ray lines and find visible line
            List<Tuple<Line, Polyline>> linesDict = new List<Tuple<Line, Polyline>>();
            foreach (Line rayLine in rayLines)
            {
                Line visibleLine = rayLine.VisibleLine(intersectObstacles, distanceTolerance);
                linesDict.Add(visibleLine.LineObstacleDictionary(intersectObstacles, distanceTolerance));
            }

            //create points chain
            List<Point> pointsChain = linesDict.PointsChain(cameraLocation, radius, distanceTolerance);

            //create cone
            PolyCurve cameraViewPolyCurve = pointsChain.CameraViewPolyCurve(coneArc, distanceTolerance);

            return cameraViewPolyCurve;
        }

        /***************************************************/

        [Description("Project obstacle polyline on camera plane.")]
        [Input("obstacle", "Obstacle to project.")]
        [Input("cameraPlane", "Plane to project the obstacle on.")]
        [Input("tolerance", "Distance tolerance for the method.")]
        [Output("newObstacle", "Obstacle polyline projected on camera plane.")]
        private static Polyline Project(this Polyline obstacle, Plane cameraPlane, double tolerance)
        {
            //close open polylines
            if (!obstacle.IsClosed(tolerance))
                obstacle = obstacle.Close(tolerance);

            if (obstacle.IsInPlane(cameraPlane, tolerance))
                return obstacle;

            Vector polylineNormal = obstacle.Normal();
            Vector cameraPlaneNormal = cameraPlane.Normal;

            if (!obstacle.IsPlanar(tolerance))
                Base.Compute.RecordWarning("Obstacle polyline is not planar. It will be closed and projected on camera plane.");
            else if (polylineNormal.IsEqual(cameraPlaneNormal, tolerance) || polylineNormal.IsEqual(-cameraPlaneNormal, tolerance))
                Base.Compute.RecordWarning("Obstacle polyline is parallel, but not in camera plane. It will be projected on camera plane.");
            else
                Base.Compute.RecordWarning("Polyline is in different plane than camera plane. It will be projected on camera plane.");

            return obstacle.Project(cameraPlane);
        }

        /***************************************************/

        [Description("Split line in given obstacles.")]
        [Input("rayLine", "Line to split.")]
        [Input("obstacles", "Polyline objects to split the line in.")]
        [Input("tolerance", "Distance tolerance for the method.")]
        [Output("splitLines", "Lines splited by the obstacles.")]
        private static List<Line> SplitLinesInObstacles(this Line line, List<Polyline> obstacles, double tolerance)
        {
            List<Point> splitPoints = new List<Point>();
            foreach (Polyline obstacle in obstacles)
            {
                foreach (Line obstLine in obstacle.SubParts())
                {
                    splitPoints.AddRange(line.LineIntersections(obstLine, false, tolerance));
                }
            }

            return line.SplitAtPoints(splitPoints, tolerance);
        }

        /***************************************************/

        [Description("Check if line is inside obstacle.")]
        [Input("line", "Line to check if it's inside obstacle.")]
        [Input("obstacles", "Polyline objects to check if line is inside.")]
        [Input("tolerance", "Distance tolerance for the method.")]
        [Output("bool", "Boolean result of the checking.")]
        private static bool IsInsideObstacle(this Line line, List<Polyline> obstacles, double tolerance)
        {
            foreach (Polyline obstacle in obstacles)
            {
                foreach (Line obstLine in obstacle.SubParts())
                {
                    if (obstLine.IsOnCurve(line.Start, tolerance) && obstLine.IsOnCurve(line.End, tolerance) && (obstLine.Direction().IsEqual(line.Direction(), tolerance) || obstLine.Direction().IsEqual(-line.Direction(), tolerance)))
                        return false;
                }
                if (obstacle.IsContaining(new List<Point> { line.Centroid(tolerance) }, true, tolerance))
                {
                    return true;
                }
            }
            return false;
        }

        /***************************************************/

        [Description("Create visible line from rayline passing through obstacles.")]
        [Input("rayLine", "Line to split and make visible.")]
        [Input("obstacles", "Polyline objects that are needed to create visible line.")]
        [Input("tolerance", "Distance tolerance for the method.")]
        [Output("visibleLine", "Line that is visible for the camera view.")]
        private static Line VisibleLine(this Line rayLine, List<Polyline> obstacles, double tolerance)
        {
            List<Line> splitLines = rayLine.SplitLinesInObstacles(obstacles, tolerance);
            List<Line> outsideLines = new List<Line>();
            foreach (Line splitLine in splitLines)
            {
                if (!splitLine.IsInsideObstacle(obstacles, tolerance))
                    outsideLines.Add(splitLine);
            }
            //create visible line
            Line visibleLine;
            if (outsideLines.Count == 1)
                visibleLine = outsideLines[0];
            else
            {
                Point startPoint = outsideLines[0].Start;
                Point endPoint = outsideLines[0].End;
                for (int i = 1; i < outsideLines.Count; i++)
                {
                    if (endPoint.IsEqual(outsideLines[i].Start, tolerance))
                    {
                        startPoint = outsideLines[0].End;
                        endPoint = outsideLines[i].End;
                    }
                }
                visibleLine = new Line { Start = startPoint, End = endPoint };
            }

            return visibleLine;
        }

        /***************************************************/

        [Description("Create dictionary from line and obstacle.")]
        [Input("line", "Line to create dictionary for.")]
        [Input("obstacles", "Polyline objects that represents obstacles for lines.")]
        [Input("tolerance", "Distance tolerance for the method.")]
        [Output("dictionary", "Dictionary for lines and obstacles.")]
        private static Tuple<Line, Polyline> LineObstacleDictionary(this Line line, List<Polyline> obstacles, double tolerance)
        {
            Tuple<Line, Polyline> lineObstDict = new Tuple<Line, Polyline>(line, null);
            if (obstacles.Count > 0)
                for (int i = 0; i < obstacles.Count; i++)
                {
                    if (obstacles[i].IsContaining(new List<Point> { line.End }, true, tolerance))
                    {
                        lineObstDict = new Tuple<Line, Polyline>(line, obstacles[i]);
                        return lineObstDict;
                    }
                }

            return lineObstDict;
        }

        /***************************************************/

        [Description("Create points chain for the camera field of view.")]
        [Input("lineObstacledictionary", "Dictionary of visible lines and obstacles.")]
        [Input("cameraLocation", "Location of the camera device.")]
        [Input("radius", "Radius of the camera view cone.")]
        [Input("tolerance", "Distance tolerance for the method.")]
        [Output("pointsChain", "Points chain of the camera field of view.")]
        private static List<Point> PointsChain(this List<Tuple<Line, Polyline>> lineObstacleDictionary, Point cameraLocation, double radius, double tolerance)
        {
            List<Point> pointsChain = new List<Point>();
            pointsChain.Add(cameraLocation);
            Point lastPoint = cameraLocation;
            for (int i = 0; i < lineObstacleDictionary.Count; i++)
            {
                Line line = lineObstacleDictionary[i].Item1;
                if (line.Start.IsEqual(cameraLocation, tolerance))
                {
                    Point point = line.End;
                    pointsChain.Add(point);
                    lastPoint = point;
                }
                else
                {
                    Point pt1 = line.Start;
                    Point pt2 = line.End;

                    if ((lastPoint.Distance(pt1) - lastPoint.Distance(pt2)) < tolerance && !((Math.Abs(lastPoint.Distance(cameraLocation) - radius) < tolerance) && (Math.Abs(pt2.Distance(cameraLocation) - radius) < tolerance)) && i > 0)
                    {
                        Polyline obst = lineObstacleDictionary[i].Item2;
                        Polyline lastObst = lineObstacleDictionary[i - 1].Item2;
                        if (!lastPoint.IsEqual(cameraLocation, tolerance) && lastObst == obst)
                        {
                            pointsChain.Add(pt2);
                            pointsChain.Add(pt1);
                            lastPoint = pt1;
                            continue;
                        }
                        pointsChain.Add(pt1);
                        pointsChain.Add(pt2);
                        lastPoint = pt2;
                    }
                    else
                    {
                        pointsChain.Add(pt2);
                        pointsChain.Add(pt1);
                        lastPoint = pt1;
                    }
                }
            }
            pointsChain = pointsChain.CullDuplicates(tolerance);
            pointsChain.Add(cameraLocation);

            return pointsChain;
        }

        /***************************************************/

        [Description("Create PolyCurve that represents camera field of view.")]
        [Input("pointsChain", "Points chain of the camera view cone.")]
        [Input("coneArc", "Camera cone arc.")]
        [Input("tolerance", "Distance tolerance for the method.")]
        [Output("viewCone", "PolyCurve that represents camera field of view.")]
        private static PolyCurve CameraViewPolyCurve(this List<Point> pointsChain, Arc coneArc, double tolerance)
        {
            List<ICurve> curves = new List<ICurve>();
            for (int i = 1; i < pointsChain.Count; i++)
            {
                Point pt1 = pointsChain[i - 1];
                Point pt2 = pointsChain[i];

                if ((pt1.Distance(coneArc) < tolerance) && (pt2.Distance(coneArc) < tolerance) && !(pt1.Distance(pt2) < tolerance))
                {
                    double p1Param = coneArc.ParameterAtPoint(pt1, tolerance);
                    double p2Param = coneArc.ParameterAtPoint(pt2, tolerance);
                    double startAngle = coneArc.EndAngle * p1Param;
                    double endAngle = coneArc.EndAngle * p2Param;
                    Arc newArc = Create.Arc(coneArc.CoordinateSystem, coneArc.Radius, startAngle, endAngle);

                    curves.Add(newArc);
                }
                else
                {
                    Line line = Create.Line(pt1, pt2);
                    curves.Add(line);
                }
            }

            PolyCurve viewCone = new PolyCurve();
            viewCone.Curves = curves;

            return viewCone;
        }

        /***************************************************/

    }
}





