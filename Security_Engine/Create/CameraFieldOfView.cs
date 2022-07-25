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
    public static partial class Create
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Returns the centreline of a CameraDevice object.")]
        [Input("cameraDevice", "The CameraDevice object to compute the field of view for.")]
        [Input("obstacles", "Polyline objects that represents obstacles for camera vision (walls, columns, stairs).")]
        [Output("centreline", "The centreline of the CameraDevice object.")]
        public static List<Line> CameraFieldOfView(this CameraDevice cameraDevice, List<Polyline> obstacles = null)
        {
            PolyCurve cameraCone = cameraDevice.CameraViewCone();
            Polyline cameraConePolyline = cameraCone.CollapseToPolyline(0.1);
            Point cameraLocation = cameraDevice.EyePosition;
            Point targetLocation = cameraDevice.TargetPosition;
            double radius = targetLocation.Distance(cameraLocation);
            Plane cameraPlane = BH.Engine.Geometry.Create.Plane(cameraLocation, Vector.ZAxis);

            //cone points
            List<Point> conePoints = new List<Point>();
            foreach (Line coneLine in cameraConePolyline.SubParts())
            {
                conePoints.AddRange(coneLine.ControlPoints());
            }

            //cone boundary points
            List<Point> intersectPoints = new List<Point>();
            intersectPoints.Add(conePoints[1]);
            intersectPoints.Add(conePoints[conePoints.Count - 2]);

            //points that intersect with obstacles
            List<Polyline> intersectObstacles = new List<Polyline>();
            foreach (Polyline obstacle in obstacles)
            {
                Polyline projObstacle = obstacle.Project(cameraPlane);
                List<Polyline> intersection = projObstacle.BooleanIntersection(cameraConePolyline);
                if (intersection.Count != 0)
                {
                    intersectObstacles.Add(projObstacle);
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
            rayLines = rayLines.OrderBy(x => x.Angle(rayLines[0])).ToList();

            //split ray lines and find visible line
            List<Dictionary<Line, Polyline>> linesDict = new List<Dictionary<Line, Polyline>>();
            foreach (Line rayLine in rayLines)
            {
                Line visibleLine = rayLine.VisibleLine(intersectObstacles);
                linesDict.Add(visibleLine.LineWallDictionary(intersectObstacles));
            }


            return null;
        }

        /***************************************************/

        [Description("Returns the polycurve cone of the camera view.")]
        [Input("cameraDevice", "The CameraDevice object to compute the camera view cone for.")]
        [Output("conePolyCurve", "PolyCurve represents camera view cone.")]
        public static PolyCurve CameraViewCone(this CameraDevice cameraDevice)
        {
            Point cameraLocation = cameraDevice.EyePosition;
            Point targetLocation = cameraDevice.TargetPosition;
            double radius = targetLocation.Distance(cameraLocation);
            double horizontal = cameraDevice.HorizontalFieldOfView;
            double angle = 2 * Math.Atan(horizontal / 2 / radius);

            Vector direction = BH.Engine.Geometry.Create.Vector(cameraLocation, cameraDevice.TargetPosition);
            Vector startPointDir = direction.Rotate(-angle / 2, Vector.ZAxis);
            Vector endPointDir = direction.Rotate(angle / 2, Vector.ZAxis);
            Point startPoint = cameraLocation + startPointDir;
            Point endPoint = cameraLocation + endPointDir;

            Arc coneArc = BH.Engine.Geometry.Create.Arc(startPoint, targetLocation, endPoint);
            Polyline simplifyArc = coneArc.CollapseToPolyline(0.1);
            Line startLine = BH.Engine.Geometry.Create.Line(cameraLocation, startPoint);
            Line endLine = BH.Engine.Geometry.Create.Line(endPoint, cameraLocation);

            PolyCurve conePolyCurve = new PolyCurve();
            conePolyCurve.Curves = new List<ICurve>() { startLine, simplifyArc, endLine };

            return conePolyCurve;
        }

        /***************************************************/

        [Description("Signed angle between two lines in XY plane.")]
        [Input("line1", "First line to compute the angle for.")]
        [Input("line2", "Second line to compute the angle for.")]
        [Output("angle", "Singed angle between two lines.")]
        private static double Angle(this Line line1, Line line2)
        {
            Vector line1Dir = line1.Direction();
            Vector line2Dir = line2.Direction();
            double angle = line1Dir.SignedAngle(line2Dir, Vector.ZAxis);

            return angle;
        }

        /***************************************************/

        [Description("Split line in obstacles.")]
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
        [Input("rayLine", "Line to check if located inside obstacle.")]
        [Input("obstacles", "Polyline objects to split the line in.")]
        [Output("splitLines", "Lines splited by the obstacles.")]
        private static bool IsInsideObstacle(this Line line, List<Polyline> obstacles)
        {
            foreach (Polyline obstacle in obstacles)
            {
                foreach (Line obstLine in obstacle.SubParts())
                {
                    if (obstLine.IsOnCurve(line.Start) && obstLine.IsOnCurve(line.End))
                        return false;
                }
                if (obstacle.IsContaining(line, true))
                {
                    return true;
                }
            }
            return false;
        }

        /***************************************************/

        [Description("Create visible line from ray line passing through obstacles.")]
        [Input("rayLine", "Line to create visible line from.")]
        [Input("obstacles", "Polyline objects to split the line in.")]
        [Output("splitLines", "Lines splited by the obstacles.")]
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
                        endPoint = outsideLines[1].End;
                    }
                }
                visibleLine = new Line { Start = startPoint, End = endPoint };
            }

            return visibleLine;
        }

        /***************************************************/

        [Description("Create dictionary from line and obstacle.")]
        [Input("line", "Line to create visible line from.")]
        [Input("obstacles", "Polyline objects to split the line in.")]
        [Output("splitLines", "Lines splited by the obstacles.")]
        private static Dictionary<Line, Polyline> LineWallDictionary(this Line line, List<Polyline> obstacles)
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
    }
}



