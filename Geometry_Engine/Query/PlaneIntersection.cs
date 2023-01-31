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
using System.ComponentModel;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** public Methods - Vectors                  ****/
        /***************************************************/

        [Description("Computes the intersection between two planes and returns the resulting line. The returned line will have a length of one, with IsInfinite set to true and direction computed as the normalised vector resulting from the cross product between normal of first and second plane.\n" + 
                     "The start and end points of the line are set according to the following:\n" + 
                     "- If the line direction has a vertical (Z) component: The start point of the returned line will be the point along the line with Z == 0, and the end point will be the start point moved with the direction vector.\n" +
                     "- Else, if the line direction has a Y component:  The start point of the returned line will be the point along the line with Y == 0, and the end point will be the start point moved with the direction vector.\n" +
                     "- Else: The start point of the returned line will be the point along the line with X == 0, and the end point will be the start point moved with the direction vector.\n" + 
                     "Null is returned for two parallel planes.")]
        [Input("plane1", "First plane for intersection.")]
        [Input("plane2", "Second plane for intersection.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("interLine", "The resulting infinite intersection line between the two planes, with a segment length set to 1.")]
        public static Line PlaneIntersection(this Plane plane1, Plane plane2, double tolerance = Tolerance.Distance)
        {
            //TODO: Testing needed!!

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

        [Description("Computes the intersection between a BoundingBox and a Plane, and returns the resulting Polyline if any intersection is found. Returns null if no intersection is found.")]
        [Input("boundingBox", "The BoundingBox to intersect with the Plane.")]
        [Input("plane", "The plane to intersect with the box.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("interCurve", "The resulting polyline from the intersection between the plane and the boundingbox.")]
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

        [Description("Computes and returns the intersection point(s) between the Arc and the Plane. Returns an empty list if the plane is parallel with the curve plane, including the case where the curve is fully within the plane.")]
        [Input("curve", "The Arc to intersect with the Plane.")]
        [Input("plane", "The Plane to intersect with the Arc.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("interPts", "The resulting intersection points between the Arc and the Plane.")]
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

        [Description("Computes and returns the intersection point(s) between the Circle and the Plane. Returns an empty list if the plane is parallel with the curve plane, including the case where the curve is fully within the plane.")]
        [Input("curve", "The Circle to intersect with the Plane.")]
        [Input("plane", "The Plane to intersect with the Circle.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("interPts", "The resulting intersection points between the Circle and the Plane.")]
        public static List<Point> PlaneIntersections(this Circle curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            //TODO: Testing needed!!
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

        [Description("Computes and returns the intersection point(s) between the Ellipse and the Plane. Returns an empty list if the plane is parallel with the curve plane, including the case where the curve is fully within the plane.")]
        [Input("curve", "The Ellipse to intersect with the Plane.")]
        [Input("plane", "The Plane to intersect with the Ellipse.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("interPts", "The resulting intersection points between the Ellipse and the Plane.")]
        public static List<Point> PlaneIntersections(this Ellipse curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            if (curve.IsNull())
                return new List<Point>();

            //Projecting the curve axes scaled by radius as onto the plane normal and curve centre to the plane (including sign) gives the equation
            //f(t) = a*cos(t)+b*sin(t)+c that is the distance from the ellipse to the plane.
            //Solving for f(t) = 0 given the ellipse parameter at the intersection point

            //Magnitude of projecting curve axes to plane normal
            double a = curve.Axis1.DotProduct(plane.Normal);    
            double b = curve.Axis2.DotProduct(plane.Normal);

            if ((Math.Abs(a) + Math.Abs(b)) < tolerance)  //Curve and plane are parallel
                return new List<Point>();

            a = a * curve.Radius1;                                              //First axis projected to the plane normal and scaled by the radius 
            b = b * curve.Radius2;                                              //Second axis projected to the plane normal and scaled by the radius
            double c = plane.Normal.DotProduct(curve.Centre - plane.Origin);    //Signed distance from the centre to the plane

            List<double> ts = new List<double>();

            bool isOverlapping = true;
            bool checkTangency = false;
            //Special case a == c
            if (Math.Abs(a - c) < tolerance)
            {
                //Below if statement is to avoid division by 0. 
                //Still holds, even if b == 0, as Atan(t) -> PI/2 as t -> infinity+
                //Hence, 2(PI-Atan(a/b)) -> 2(PI-PI/2)=PI as b -> 0
                //For future reference, Atan(t) -> -PI/2 as t -> infinity- , which results in expression evaluation to 3PI, but
                //Sin(PI) = Sin(3PI) = 0 and Cos(PI) = Cos(3PI) = -1, hence will evalutate to the same position on the ellipse and no need to differentiate between the 0- and 0+ case of b
                if (Math.Abs(b) > 1e-16)
                    ts.Add(2 * (Math.PI - Math.Atan(a / b)));
                else
                    ts.Add(Math.PI);
            }
            else
            {
                double sqrtTerm = a * a + b * b - c * c;
                isOverlapping = sqrtTerm > 0;  //If this is true, there will be an intersection
                checkTangency = Math.Abs(sqrtTerm) / (2 * Math.Max(curve.Radius1, curve.Radius2)) < tolerance;  //If true, need to check if the intersection is tangential

                if (!isOverlapping)
                {
                    if (checkTangency)
                        sqrtTerm = 0;   //Set to 0, to be picked up in tangency check
                    else
                        return new List<Point>();   //Not overlapping and not tangential
                }

                sqrtTerm = Math.Sqrt(sqrtTerm);
                ts.Add(2 * Math.Atan((b - sqrtTerm) / (a - c)));
                ts.Add(2 * Math.Atan((b + sqrtTerm) / (a - c)));
            }

            //Function for getting the point on the ellipse based ont the angle parameter
            Func<double, Point> elipsePt = t => curve.Centre + Math.Cos(t) * curve.Radius1 * curve.Axis1 + Math.Sin(t) * curve.Radius2 * curve.Axis2;

            
            if (checkTangency)
            {
                //Get out the point at the mid parameter
                Point tangentPt = elipsePt((ts[0] + ts[1]) / 2);

                //If point at mid parameter is within tolerance distance from the plane, return it
                if (tangentPt.Distance(plane) < tolerance)
                    return new List<Point> { tangentPt };
                else if (!isOverlapping)    //If not tangential and not overlapping, there is no intersection. Return empty list
                    return new List<Point>();
            }

            //All checks done, simply return the points at the found parameters
            return ts.Select(elipsePt).ToList();
        }

        /***************************************************/

        [Description("Computes and returns the intersection point between the Line and the Plane. Returns an empty list if the plane is parallel with the curve plane, including the case where the curve is fully within the plane.")]
        [Input("curve", "The Line to intersect with the Plane.")]
        [Input("plane", "The Plane to intersect with the Line.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("interPt", "The resulting intersection point between the Line and the Plane.")]
        public static List<Point> PlaneIntersections(this Line curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            List<Point> result = new List<Point>();
            Point iPt = curve.PlaneIntersection(plane, false, tolerance);
            if (iPt != null) result.Add(iPt);
            return result;
        }

        /***************************************************/

        [Description("Computes and returns the intersection point between the Line and the Plane. Returns null if the plane is parallel with the curve plane, including the case where the curve is fully within the plane.")]
        [Input("line", "The Line to intersect with the Plane.")]
        [Input("plane", "The Plane to intersect with the Line.")]
        [Input("useInfiniteLine", "If true, the intersection will computed based on the infinite line. If false, will be based on the finite line segment.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("interPt", "The resulting intersection point between the Line and the Plane.")]
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
        [Description("Method not implemented.")]
        [NotImplemented]
        public static List<Point> PlaneIntersections(this NurbsCurve c, Plane p, out List<double> curveParameters, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [Description("Computes and returns the intersection point(s) between the PolyCurve and the Plane. Returns an empty list if the plane is parallel with the curve plane, including the case where the curve is fully within the plane.")]
        [Input("curve", "The PolyCurve to intersect with the Plane.")]
        [Input("plane", "The Plane to intersect with the PolyCurve.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("interPts", "The resulting intersection points between the PolyCurve and the Plane.")]
        public static List<Point> PlaneIntersections(this PolyCurve curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            //TODO: Testing needed!!
            return curve.Curves.SelectMany(x => x.IPlaneIntersections(plane, tolerance)).ToList();
            //throw new NotImplementedException();
        }

        /***************************************************/

        [Description("Computes and returns the intersection point(s) between the Polyline and the Plane. Returns an empty list if the plane is parallel with the curve plane, including the case where the curve is fully within the plane.")]
        [Input("curve", "The Polyline to intersect with the Plane.")]
        [Input("plane", "The Plane to intersect with the Polyline.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("interPts", "The resulting intersection points between the Polyline and the Plane.")]
        public static List<Point> PlaneIntersections(this Polyline curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            //TODO: Testing needed!!
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

        [Description("Computes and returns the intersection point(s) between the curve and the Plane. Returns an empty list if the plane is parallel with the curve plane, including the case where the curve is fully within the plane.")]
        [Input("curve", "The curve to intersect with the Plane.")]
        [Input("plane", "The Plane to intersect with the curve.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("interPts", "The resulting intersection points between the curve and the Plane.")]
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


