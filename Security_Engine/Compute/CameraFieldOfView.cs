/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.Engine.Geometry;
using BH.Engine.Spatial;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Security.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Create = BH.Engine.Geometry.Create;

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

            double coneAngle = cameraDevice.Angle;

            PolyCurve cameraConeLeft = cameraDevice.ViewCone(coneAngle/2);

            PolyCurve cameraConeRight = cameraDevice.ViewCone(-coneAngle / 2);
            
            Point cameraLocation = cameraDevice.EyePosition;
            Point targetLocation = cameraDevice.TargetPosition;
            double radius = targetLocation.Distance(cameraLocation);

            Plane cameraPlane = BH.Engine.Geometry.Create.Plane(cameraLocation, Vector.ZAxis);

            PolyCurve cameraViewPolyCurveLeft = GenerateCameraFieldOfView(cameraConeLeft,obstacles,cameraPlane,cameraLocation,radius,angleTolerance,distanceTolerance);

            PolyCurve cameraViewPolyCurveRight = GenerateCameraFieldOfView(cameraConeRight, obstacles, cameraPlane, cameraLocation, radius, angleTolerance, distanceTolerance);

            List<Line> lines = new List<Line>();
            List<Arc> arcs = new List<Arc>();

            foreach (ICurve curve in cameraViewPolyCurveLeft.Curves)
            {
                if (curve is Line line)
                    lines.Add(line);
                else if (curve is Arc arc)
                    arcs.Add(arc);                
            }

            foreach (ICurve curve in cameraViewPolyCurveRight.Curves)
            {
                if (curve is Line line)
                    lines.Add(line);
                else if (curve is Arc arc)
                    arcs.Add(arc);
            }

            lines = CombineAndRemoveDuplicates(lines, distanceTolerance);

            List<ICurve> curves = new List<ICurve>();
            curves.AddRange(lines);
            curves.AddRange(arcs);
           
            PolyCurve outPolyCurve = new PolyCurve();

            outPolyCurve.Curves = curves;

            outPolyCurve = outPolyCurve.SortCurves(angleTolerance);

            if (!(outPolyCurve.Curves[0] is Line))
                outPolyCurve.Curves = ChangeOrderToStartWithLine(outPolyCurve.Curves);

            outPolyCurve.Curves = CombineAdjacentArcsIntoOne(outPolyCurve.Curves);

            return outPolyCurve;
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

            //reverse points chain to have correct start and end arc angles
            pointsChain.Reverse();

            return pointsChain;
        }

        private class PointEqualityComparer : IEqualityComparer<Point>
        {
            private readonly double _tol;
            public PointEqualityComparer(double tol) { _tol = tol; }
            public bool Equals(Point a, Point b) => a.IsEqual(b, _tol);
            public int GetHashCode(Point p) => 0; // Not efficient, but works for small sets
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

                double distance1 = pt1.Distance(coneArc);
                double distance2 = pt2.Distance(coneArc);
                double distance3 = pt1.Distance(pt2);

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

        private static PolyCurve GenerateCameraFieldOfView(PolyCurve cameraCone, List<Polyline> obstacles, Plane cameraPlane, Point cameraLocation, double radius, double angleTolerance, double distanceTolerance)
        {
            Polyline cameraConePolyline = cameraCone.CollapseToPolyline(angleTolerance);
            Arc coneArc = cameraCone.Curves[1] as Arc;
            Line startLine = cameraCone.Curves[0] as Line;
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

                if (cameraCone.Curves.Count != 1)
                {
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
            rayLines = rayLines.OrderByDescending(x => Math.Abs(x.SignedAngle(startLine, Vector.ZAxis))).ToList();

            //split ray lines and find visible line
            List<Tuple<Line, Polyline>> linesDict = new List<Tuple<Line, Polyline>>();
            
            foreach (Line rayLine in rayLines)
            {
                Line visibleLine = rayLine.VisibleLine(intersectObstacles, distanceTolerance);
                linesDict.Add(visibleLine.LineObstacleDictionary(intersectObstacles, distanceTolerance));
            }                       

            List<Point> chainPoints = linesDict.PointsChain(cameraLocation, radius, distanceTolerance);

            PolyCurve cameraViewPolyCurve = CameraViewPolyCurve(chainPoints, coneArc, distanceTolerance);

            return cameraViewPolyCurve;           
        }

        private static List<Line> CombineAndRemoveDuplicates(List<Line> lines, double tolerance)
        {
            double sqTol = tolerance * tolerance;
            List<Line> result = lines.Select(l => l).ToList();
            List<int> ints = new List<int>();

            for (int i = lines.Count - 2; i >= 0; i--)
            {
                for (int j = lines.Count - 1; j > i; j--)
                {
                    Line l1 = lines[i];
                    Line l2 = lines[j];
                    if ((l1.Start.SquareDistance(l2.Start) <= sqTol && l1.End.SquareDistance(l2.End) <= sqTol) || (l1.Start.SquareDistance(l2.End) <= sqTol && l1.End.SquareDistance(l2.Start) <= sqTol))
                    {
                        ints.Add(i);
                        ints.Add(j);
                    }
                }
            }

            ints = ints.Distinct().ToList();
            ints = ints.OrderByDescending(x => x).ToList();
            for (int i = 0; i < ints.Count; i++)
            {
                lines.RemoveAt(ints[i]);
            }

            return lines;
        }

        private static List<ICurve> CombineAdjacentArcsIntoOne(List<ICurve> startCurves, double tolerance = 1e-6)
        {
            List<ICurve> outCurves = new List<ICurve>();
            int i = 0;
            while (i < startCurves.Count)
            {
                // If current is not an arc, just add and continue
                if (!(startCurves[i] is Arc))
                {
                    outCurves.Add(startCurves[i]);
                    i++;
                    continue;
                }

                // Find run of adjacent arcs
                int arcStart = i;
                int arcEnd = i;
                while (arcEnd + 1 < startCurves.Count && startCurves[arcEnd + 1] is Arc)
                    arcEnd++;

                int arcCount = arcEnd - arcStart + 1;

                if (arcCount == 1)
                {
                    outCurves.Add(startCurves[arcStart]);
                }
                else
                {
                    // Collect and order arcs by connectivity
                    List<Arc> arcRun = new List<Arc>();
                    for (int j = arcStart; j <= arcEnd; j++)
                        arcRun.Add((Arc)startCurves[j]);

                    List<Arc> orderedArcs = OrderArcsByConnectivity(arcRun, tolerance);

                    // If the arcs form a closed loop, create a circle
                    if (orderedArcs.First().StartPoint().Distance(orderedArcs.Last().EndPoint()) < tolerance)
                    {
                        Arc arc = orderedArcs[0];
                        outCurves.Add((Arc)Create.Circle(arc.Centre(), Vector.ZAxis, arc.Radius));
                    }
                    else
                    {
                        // Combine into a single arc using first, middle, and last points
                        Point start = orderedArcs.First().StartPoint();
                        Point end = orderedArcs.Last().EndPoint();
                        Point mid = orderedArcs[orderedArcs.Count / 2].EndPoint();

                        // Try to ensure all three points are unique
                        if (start.Distance(end) < tolerance)
                        {
                            end = orderedArcs[orderedArcs.Count / 2].StartPoint();
                            if (start.Distance(end) < tolerance && orderedArcs.Count > 1)
                                end = orderedArcs[1].EndPoint();
                        }
                        if (start.Distance(mid) < tolerance)
                        {
                            mid = orderedArcs[0].EndPoint();
                            if (start.Distance(mid) < tolerance && orderedArcs.Count > 1)
                                mid = orderedArcs[1].StartPoint();
                        }
                        if (end.Distance(mid) < tolerance)
                        {
                            mid = orderedArcs[0].EndPoint();
                            if (end.Distance(mid) < tolerance && orderedArcs.Count > 1)
                                mid = orderedArcs[1].StartPoint();
                        }

                        // Final check: if still not unique, skip arc creation
                        if (start.Distance(end) < tolerance || start.Distance(mid) < tolerance || end.Distance(mid) < tolerance)
                        {
                            Base.Compute.RecordWarning("Could not find three unique points to create a combined arc. Skipping arc creation.");
                        }
                        else
                        {
                            outCurves.Add(Create.Arc(start, mid, end));
                        }
                    }
                }

                i = arcEnd + 1;
            }
            return outCurves;
        }

        // Helper: Orders arcs so that each arc's end matches the next arc's start
        private static List<Arc> OrderArcsByConnectivity(List<Arc> arcs, double tolerance)
        {
            if (arcs.Count <= 1)
                return arcs.ToList();

            List<Arc> ordered = new List<Arc> { arcs[0] };
            var used = new HashSet<int> { 0 };

            while (ordered.Count < arcs.Count)
            {
                Arc last = ordered.Last();
                bool found = false;
                for (int i = 0; i < arcs.Count; i++)
                {
                    if (used.Contains(i))
                        continue;
                    if (last.EndPoint().Distance(arcs[i].StartPoint()) < tolerance)
                    {
                        ordered.Add(arcs[i]);
                        used.Add(i);
                        found = true;
                        break;
                    }
                    else if (last.EndPoint().Distance(arcs[i].EndPoint()) < tolerance)
                    {
                        // Reverse arc if needed
                        Arc reversed = ReverseArc(arcs[i]);
                        ordered.Add(reversed);
                        used.Add(i);
                        found = true;
                        break;
                    }
                }
                if (!found)
                    break; // Could not connect further
            }
            // If not all arcs could be connected, append the rest as is
            for (int i = 0; i < arcs.Count; i++)
                if (!used.Contains(i))
                    ordered.Add(arcs[i]);
            return ordered;
        }

        private static Arc ReverseArc(Arc arc)
        {
            // Compute the midpoint parameter (halfway between start and end angles)
            double midAngle = arc.StartAngle + (arc.EndAngle - arc.StartAngle) / 2.0;
            // Calculate the midpoint in the arc's coordinate system
            Point midPoint = arc.CoordinateSystem.Origin
                + arc.CoordinateSystem.X * (arc.Radius * Math.Cos(midAngle))
                + arc.CoordinateSystem.Y * (arc.Radius * Math.Sin(midAngle));

            // Create a new arc with swapped start and end, and the computed midpoint
            return Create.Arc(arc.EndPoint(), midPoint, arc.StartPoint());
        }

        private static List<ICurve> ChangeOrderToStartWithLine(List<ICurve> startCurves)
        {
            List <ICurve> outCurves = new List<ICurve>();

            bool flag = false;

            for (int i = 0; i<startCurves.Count; i++)
            {
                ICurve firstLineCurve = startCurves[i];
                if ( firstLineCurve is BH.oM.Geometry.Line)
                {
                    outCurves.Add(firstLineCurve);
                    for (int j = i+1;j < startCurves.Count; j++)
                    {
                        outCurves.Add(startCurves[j]);
                    }
                    for (int j = i-1; j >= 0; j--)
                    {
                        outCurves.Add(startCurves[j]);
                    }
                    flag = true;
                }
                if (flag)
                    break;                
            }

            if (flag)
                return outCurves;
            else
                return startCurves;
        }
    }
}






