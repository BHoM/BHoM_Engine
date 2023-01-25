/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** public Methods - Vectors                  ****/
        /***************************************************/

        //TODO: Testing needed!!
        public static Line PlaneIntersection(this Plane plane1, Plane plane2, double tolerance = Tolerance.Distance)
        {

            if (plane1.Normal.IsParallel(plane2.Normal) != 0)
                return null;

            //Calculate tangent of line perpendicular to the normal of the two planes
            Vector tangent = plane1.Normal.CrossProduct(plane2.Normal).Normalise();

            //d-values from plane equation: ax+by+cz+d=0
            double d1 = -plane1.Normal.DotProduct(Create.Vector(plane1.Origin));
            double d2 = -plane2.Normal.DotProduct(Create.Vector(plane2.Origin));

            Point orgin;

            Vector n1 = plane1.Normal;
            Vector n2 = plane2.Normal;

            if (Math.Abs(tangent.Z) >= Tolerance.Angle)
            {
                double x0 = (n1.Y * d2 - n2.Y * d1) / (n1.X * n2.Y - n2.X * n1.Y);
                double y0 = (n2.X * d1 - n1.X * d2) / (n1.X * n2.Y - n2.X * n1.Y);

                orgin = new Point { X = x0, Y = y0, Z = 0 };
            }
            else if (Math.Abs(tangent.Y) >= Tolerance.Angle)
            {
                double x0 = (n1.Z * d2 - n2.Z * d1) / (n1.X * n2.Z - n2.X * n1.Z);
                double z0 = (n2.X * d1 - n1.X * d2) / (n1.X * n2.Z - n2.X * n1.Z);
                orgin = new Point { X = x0, Y = 0, Z = z0 };
            }
            else
            {
                double y0 = (n1.Z * d2 - n2.Z * d1) / (n1.Y * n2.Z - n2.Y * n1.Z);
                double z0 = (n2.Y * d1 - n1.Y * d2) / (n1.Y * n2.Z - n2.Y * n1.Z);
                orgin = new Point { X = 0, Y = y0, Z = z0 };
            }

            Line result = new Line { Start = orgin, End = orgin + tangent };
            result.Infinite = true;
            return result;
        }


        /***************************************************/
        /**** public Methods - Bounding Box             ****/
        /***************************************************/

        public static Polyline PlaneIntersection(this BoundingBox boundingBox, Plane plane, double tolerance = Tolerance.Distance)
        {
            List<Point> controlPoints = new List<Point>();
            foreach (Line line in boundingBox.Edges())
            {
                Point intPt = line.PlaneIntersection(plane);
                if (intPt != null)
                    controlPoints.Add(intPt);
            }

            controlPoints = controlPoints.CullDuplicates(tolerance);
            if (controlPoints.Count < 2)
                return null;

            Point average = controlPoints.Average();
            Vector firstVector = controlPoints[0] - average;
            controlPoints.Sort(delegate (Point p1, Point p2) { return firstVector.SignedAngle(p1 - average, plane.Normal).CompareTo(firstVector.SignedAngle(p2 - average, plane.Normal)); });
            controlPoints.Add(controlPoints.First());

            return new Polyline { ControlPoints = controlPoints };
        }


        /***************************************************/
        /**** public Methods - Curves                   ****/
        /***************************************************/

        public static List<Point> PlaneIntersections(this Arc curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            //Construct cricle for intersection
            Circle circle = new Circle { Centre = curve.CoordinateSystem.Origin, Normal = curve.CoordinateSystem.Z, Radius = curve.Radius };
            //Get circle intersection points
            List<Point> circleInter = circle.PlaneIntersections(plane, tolerance);

            if (circleInter.Count < 0)
                return new List<Point>();

            //TODO: With the updated definition of the arc, this could probably be improved upon.
            //Using old methodology for now

            Point st = curve.StartPoint();
            Point mid = curve.PointAtParameter(0.5);
            Point end = curve.EndPoint();

            //Construct lines for checking
            Line line1 = new Line { Start = st, End = mid };
            Line line2 = new Line { Start = mid, End = end };

            List<Point> interPoints = new List<Point>();

            //Check if interpoints are on the arc
            for (int i = 0; i < circleInter.Count; i++)
            {
                if (line1.LineIntersection(new Line { Start = circle.Centre, End = circleInter[i] }, false, tolerance) != null)
                    interPoints.Add(circleInter[i]);
                else if (line2.LineIntersection(new Line { Start = circle.Centre, End = circleInter[i] }, false, tolerance) != null)
                    interPoints.Add(circleInter[i]);
            }

            return interPoints;

        }

        /***************************************************/

        //TODO: Testing needed!!
        public static List<Point> PlaneIntersections(this Circle curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            if (curve.Normal.IsParallel(plane.Normal) != 0)
                return new List<Point>();

            Line l = plane.PlaneIntersection(new Plane { Origin = curve.Centre, Normal = curve.Normal }, tolerance);

            Point tempPt = l.ClosestPoint(curve.Centre, true);

            double centreDist = tempPt.Distance(curve.Centre);

            if (Math.Abs(curve.Radius - centreDist) < tolerance)
                return new List<Point> { tempPt };
            else if (centreDist < curve.Radius)
            {
                Vector v = l.Direction();
                double dist = Math.Sqrt(curve.Radius * curve.Radius - centreDist * centreDist);
                v = v * dist;
                return new List<Point> { tempPt + v, tempPt - v };
            }
            else
            {
                return new List<Point>();
            }
        }

        /***************************************************/

        public static List<Point> PlaneIntersections(this Ellipse curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            if(curve.IsNull())
                return new List<Point>();

            Vector normal = curve.Normal();
            if (normal.IsParallel(plane.Normal) != 0)
                return new List<Point>();

             //Get line from intersection of planes
            Line l = plane.PlaneIntersection(new Plane { Origin = curve.Centre, Normal = normal }, tolerance);

            //Get intersection points between ellipse and infinite line
            return curve.LineIntersections(l, true, tolerance);
        }

        /***************************************************/

        public static List<Point> PlaneIntersections(this Line curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            List<Point> result = new List<Point>();
            Point iPt = curve.PlaneIntersection(plane, false, tolerance);
            if (iPt != null) result.Add(iPt);
            return result;
        }

        /***************************************************/

        public static Point PlaneIntersection(this Line line, Plane plane, bool useInfiniteLine = true, double tolerance = Tolerance.Distance)
        {
            useInfiniteLine &= line.Infinite;

            Vector dir = (line.End - line.Start);//.Normalise();

            //Return null if parallel
            if (Math.Abs(dir * plane.Normal) < tolerance)
                return null;

            double t = (plane.Normal * (plane.Origin - line.Start)) / (plane.Normal * dir);

            double tTol = tolerance / dir.Length();

            // Return null if intersection out of segment limits
            if (!useInfiniteLine && (t < -tTol || t > 1 + tTol))
                return null;

            return line.Start + t * dir;
        }

        /***************************************************/

        //TODO: Resolve. Method is not implemented and have input not matching the interface.
        [NotImplemented]
        public static List<Point> PlaneIntersections(this NurbsCurve c, Plane p, out List<double> curveParameters, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        //TODO: Testing needed!!
        public static List<Point> PlaneIntersections(this PolyCurve curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            return curve.Curves.SelectMany(x => x.IPlaneIntersections(plane, tolerance)).ToList();
            //throw new NotImplementedException();
        }

        /***************************************************/

        //TODO: Testing needed!!
        public static List<Point> PlaneIntersections(this Polyline curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            List<Point> result = new List<Point>();
            List<int> sameSide = plane.Side(curve.ControlPoints, tolerance);

            int previousSide = sameSide[0];
            int Length = curve.IsClosed(tolerance) && sameSide[sameSide.Count - 1] == 0 ? sameSide.Count - 1 : sameSide.Count;

            for (int i = 1; i < Length; i++)
            {
                if (sameSide[i] != previousSide)
                {
                    if (previousSide != 0)
                    {
                        Line line = new Line { Start = curve.ControlPoints[i - 1], End = curve.ControlPoints[i] };
                        Point pt = PlaneIntersection(line, plane, false, tolerance);
                        if (pt != null)
                            result.Add(pt);
                    }
                    else
                    {
                        result.Add(curve.ControlPoints[i - 1]);
                    }
                    previousSide = sameSide[i];
                }
            }

            if (sameSide[sameSide.Count - 1] == 0 && previousSide != sameSide[sameSide.Count - 1] && result.Count % 2 == 1)
            {
                result.Add(curve.IEndPoint());
            }

            return result;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<Point> IPlaneIntersections(this ICurve curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            return PlaneIntersections(curve as dynamic, plane, tolerance);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static List<Point> PlaneIntersections(this ICurve curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            Base.Compute.RecordError($"PlaneIntersections is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}


