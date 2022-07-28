/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

        [Description("Returns the polycurve that represents security camera field of view.")]
        [Input("cameraDevice", "CameraDevice object to compute the field of view for.")]
        [Input("obstacles", "Polyline objects that represents obstacles for camera vision (walls, columns, stairs).")]
        [Output("fieldOfView", "PolyCurve object that represents security camera field of view.")]
        public static PolyCurve CameraFieldOfView(this CameraDevice cameraDevice, List<Polyline> obstacles, double distanceTolerance = BH.oM.Geometry.Tolerance.Distance, double angleTolerance = 0.01)
        {
            PolyCurve cameraCone = cameraDevice.ViewCone();
            Polyline cameraConePolyline = cameraCone.CollapseToPolyline(angleTolerance);
            Point cameraLocation = cameraDevice.EyePosition;
            Point targetLocation = cameraDevice.TargetPosition;
            double radius = targetLocation.Distance(cameraLocation);
            Plane cameraPlane = BH.Engine.Geometry.Create.Plane(cameraLocation, Vector.ZAxis);

            //cone points
            List<Point> conePoints = cameraConePolyline.ControlPoints();

            //add cone boundary points as intersectPoints
            List<Point> intersectPoints = new List<Point>();
            intersectPoints.Add(conePoints[1]);
            intersectPoints.Add(conePoints[conePoints.Count - 2]);

            //simplify obstacles
            List<Polyline> projObstacles = obstacles.Select(x => x.FixAndProject(cameraPlane, distanceTolerance)).ToList();
            projObstacles = projObstacles.BooleanUnion(distanceTolerance);

            //points that intersect with obstacles
            List<Polyline> intersectObstacles = new List<Polyline>();
            foreach (Polyline obstacle in projObstacles)
            {
                if (obstacle.IsContaining(new List<Point> { cameraLocation }))
                {
                    Base.Compute.RecordWarning("Camera Device is inside obstacle. Null value will be returned.");
                    return null;
                }
                List<Polyline> intersection = obstacle.BooleanIntersection(cameraConePolyline, distanceTolerance, angleTolerance);
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
            intersectPoints = intersectPoints.CullDuplicates();

            //create and sort extended ray lines passing through intersected points
            List<Line> rayLines = new List<Line>();
            foreach (Point point in intersectPoints)
            {
                Line line = BH.Engine.Geometry.Create.Line(cameraLocation, point);
                Line rayLine = line.Extend(0, radius - line.Length());
                rayLines.Add(rayLine);
            }
            rayLines = rayLines.CullDuplicateLines();
            rayLines = rayLines.OrderBy(x => x.SingedAngle(rayLines[0], Vector.ZAxis)).ToList();

            //split ray lines and find visible line
            List<Dictionary<Line, Polyline>> linesDict = new List<Dictionary<Line, Polyline>>();
            foreach (Line rayLine in rayLines)
            {
                Line visibleLine = rayLine.VisibleLine(intersectObstacles);
                linesDict.Add(visibleLine.LineObstacleDictionary(intersectObstacles));
            }

            //create points chain
            List<Point> pointsChain = linesDict.PointsChain(cameraLocation, radius, distanceTolerance);

            //create cone
            Arc coneArc = cameraCone.Curves[1] as Arc;
            PolyCurve cameraViewPolyCurve = pointsChain.CameraViewPolyCurve(cameraLocation, radius, coneArc, distanceTolerance);

            return cameraViewPolyCurve;
        }

        /***************************************************/

        [Description("Project obstacle on given plane and fix if possible.")]
        [Input("obstacle", "Obstacle to check and fix.")]
        [Input("cameraPlane", "Plane to project the obstacle on.")]
        [Output("newObstacle", "Obstacle after projecting and fixing.")]

        private static Polyline FixAndProject(this Polyline obstacle, Plane cameraPlane, double tolerance)
        {
            if (obstacle.IsInPlane(cameraPlane, tolerance))
                return obstacle;

            if (!obstacle.IsClosed(tolerance) || !obstacle.IsPlanar(tolerance))
            {
                Base.Compute.RecordWarning("Obstacle polyline is open and not planar. It will be closed and projected on camera plane.");
                List<Point> projPoints = obstacle.ControlPoints.Select(x => x.Project(cameraPlane)).ToList();
                return BH.Engine.Geometry.Create.Polyline(projPoints);
            }
            else if (obstacle.Normal().IsEqual(cameraPlane.Normal, tolerance) || obstacle.Normal().IsEqual(-cameraPlane.Normal, tolerance))
            {
                Base.Compute.RecordWarning("Obstacle polyline is parallel, but not in camera plane. It will be projected on camera plane.");
                return obstacle.Project(cameraPlane);
            }
            else
            {
                Base.Compute.RecordWarning("Polyline is in different plane than camera plane. It will be projected on camera plane.");
                return obstacle.Project(cameraPlane);
            }
        }

        /***************************************************/

        [Description("Split line in given obstacles.")]
        [Input("rayLine", "Line to split.")]
        [Input("obstacles", "Polyline objects to split the line in.")]
        [Output("splitLines", "Lines splited by the obstacles.")]
        private static List<Line> SplitLinesInObstacles(this Line line, List<Polyline> obstacles)
        {
            List<Point> splitPoints = new List<Point>();
            foreach (Polyline obstacle in obstacles)
            {
                foreach (Line obstLine in obstacle.SubParts())
                {
                    Point point = line.LineIntersection(obstLine);
                    if (point != null)
                        splitPoints.Add(point);
                }
            }
            return line.SplitAtPoints(splitPoints);
        }

        /***************************************************/

        [Description("Check if line is inside obstacle.")]
        [Input("line", "Line to check if it's inside obstacle.")]
        [Input("obstacles", "Polyline objects to check if line is inside.")]
        [Output("bool", "Boolean result of the checking.")]
        private static bool IsInsideObstacle(this Line line, List<Polyline> obstacles)
        {
            foreach (Polyline obstacle in obstacles)
            {
                foreach (Line obstLine in obstacle.SubParts())
                {
                    if (obstLine.IsOnCurve(line.Start) && obstLine.IsOnCurve(line.End))
                        return false;
                }
                if (obstacle.IsContaining(new List<Point> { line.Centroid() }, true))
                {
                    return true;
                }
            }
            return false;
        }

        /***************************************************/

        [Description("Create visible line from rayline passing through obstacles.")]
        [Input("obstacles", "Polyline objects that are needed to create visible line.")]
        [Output("visibleLine", "Line that is visible for the camera view.")]
        private static Line VisibleLine(this Line rayLine, List<Polyline> obstacles)
        {
            List<Line> splitLines = rayLine.SplitLinesInObstacles(obstacles);
            List<Line> outsideLines = new List<Line>();
            foreach (Line splitLine in splitLines)
            {
                if (!splitLine.IsInsideObstacle(obstacles))
                    outsideLines.Add(splitLine);
            }
            //create visible line
            Line visibleLine = new Line();
            if (outsideLines.Count == 1)
                visibleLine = outsideLines[0];
            else
            {
                Point startPoint = outsideLines[0].Start;
                Point endPoint = outsideLines[0].End;
                for (int i = 1; i < outsideLines.Count; i++)
                {
                    if (endPoint.IsEqual(outsideLines[i].Start))
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
        [Output("dictionary", "Dictionary for lines and obstacles.")]
        private static Dictionary<Line, Polyline> LineObstacleDictionary(this Line line, List<Polyline> obstacles)
        {
            Dictionary<Line, Polyline> lineObstDict = new Dictionary<Line, Polyline>();
            if (obstacles.Count == 0)
            {
                lineObstDict.Add(line, null);
            }
            for (int i = 0; i < obstacles.Count; i++)
            {
                if (obstacles[i].IsContaining(new List<Point> { line.End }))
                {
                    lineObstDict.Add(line, obstacles[i]);
                    return lineObstDict;
                }
                else if (i == obstacles.Count - 1)
                {
                    lineObstDict.Add(line, null);
                }
            }

            return lineObstDict;
        }

        /***************************************************/

        [Description("Create points chain for the camera field of view.")]
        [Input("lineObstacledictionary", "Dictionary of visible lines and obstacles.")]
        [Input("cameraLocation", "Location of the camera device.")]
        [Input("radius", "Radius of the camera view cone.")]
        [Output("pointsChain", "Points chain of the camera field of view.")]
        private static List<Point> PointsChain(this List<Dictionary<Line, Polyline>> lineObstacledictionary, Point cameraLocation, double radius, double tolerance)
        {
            List<Point> pointsChain = new List<Point>();
            pointsChain.Add(cameraLocation);
            Point lastPoint = cameraLocation;
            for (int i = 0; i < lineObstacledictionary.Count; i++)
            {
                Line line = lineObstacledictionary[i].Keys.First();
                if (line.Start.IsEqual(cameraLocation))
                {
                    Point point = line.End;
                    pointsChain.Add(point);
                    lastPoint = point;
                }
                else
                {
                    Point pt1 = line.Start;
                    Point pt2 = line.End;

                    if (lastPoint.Distance(pt1) < lastPoint.Distance(pt2) && !((Math.Abs(lastPoint.Distance(cameraLocation) - radius) < tolerance) && (Math.Abs(pt2.Distance(cameraLocation) - radius) < tolerance)))
                    {
                        Polyline obst = lineObstacledictionary[i].Values.First();
                        Polyline lastObst = lineObstacledictionary[i - 1].Values.First();
                        if (!lastPoint.IsEqual(cameraLocation) && lastObst == obst)
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
        [Input("cameraLocation", "Location of the camera device.")]
        [Input("radius", "Radius of the camera view cone.")]
        [Output("viewCone", "PolyCurve that represents camera field of view.")]
        private static PolyCurve CameraViewPolyCurve(this List<Point> pointsChain, Point cameraLocation, double radius, Arc coneArc, double tolerance)
        {
            List<ICurve> curves = new List<ICurve>();
            for (int i = 1; i < pointsChain.Count; i++)
            {
                Point pt1 = pointsChain[i - 1];
                Point pt2 = pointsChain[i];

                if ((Math.Abs(pt1.Distance(cameraLocation) - radius) < tolerance) && (Math.Abs(pt2.Distance(cameraLocation) - radius) < tolerance))
                {
                    double p1Param = coneArc.ParameterAtPoint(pt1, tolerance);
                    double p2Param = coneArc.ParameterAtPoint(pt2, tolerance);
                    double p3Param = (p1Param + p2Param) / 2;
                    Point pt3 = coneArc.PointAtParameter(p3Param);
                    Arc newArc = BH.Engine.Geometry.Create.Arc(pt1, pt3, pt2, tolerance);
                    curves.Add(newArc);
                }
                else
                {
                    Line line = BH.Engine.Geometry.Create.Line(pt1, pt2);
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



