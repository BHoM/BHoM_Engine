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

using BH.Engine.Base;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Quantities.Attributes;
using BH.oM.Geometry.CoordinateSystem;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculates and returns the intersection point of two Lines. If the lines are collinear or no intersection point can be found, null is returned.")]
        [Input("line1", "First Line to intersect.")]
        [Input("line2", "Second Line to intersect.")]
        [Input("useInfiniteLines", "If true or if a lines Infinite property is true, a intersection point found that is outside the domain of that lines start and end point is accepted. If false, and the lines Infinite property is false, the found intersection point needs to be on the finite line segment, between or on the start and end point, if not, null is returned.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Used for checking if the intersection point is within acceptable distance from the Lines.", typeof(Length))]
        [Input("angleTolerance", "Angle tolerance to be used in the method. Used for checking if the lines are collinear", typeof(Angle))]
        [Output("intersection", "The intersection point of the two Lines. If no intersection point is found, null is returned.")]
        public static Point LineIntersection(this Line line1, Line line2, bool useInfiniteLines = false, double tolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            double sqTol = tolerance * tolerance;

            BH.oM.Base.Output<double, double> intParamsOutput = line1.SkewLineProximity(line2, angleTolerance);
            if (intParamsOutput == null)
                return null;
            else
            {
                double[] intParams = new double[] { intParamsOutput.Item1, intParamsOutput.Item2 };
                double t1 = intParams[0];
                double t2 = intParams[1];
                
                Point intPt1 = line1.Start + t1 * (line1.End - line1.Start);
                Point intPt2 = line2.Start + t2 * (line2.End - line2.Start);

                if (intPt1.SquareDistance(intPt2) <= sqTol)
                {
                    bool line1Infinite = line1.Infinite || useInfiniteLines;
                    bool line2Infinte = line2.Infinite || useInfiniteLines;
                    Point intPt = (intPt1 + intPt2) * 0.5;
                    if (!line1Infinite && ((t1 < 0 && line1.Start.SquareDistance(intPt) > sqTol) || (t1 > 1 && line1.End.SquareDistance(intPt) > sqTol)))
                        return null;
                    if (!line2Infinte && ((t2 < 0 && line2.Start.SquareDistance(intPt) > sqTol) || (t2 > 1 && line2.End.SquareDistance(intPt) > sqTol)))
                        return null;

                    return intPt;
                }

                return null;
            }
        }

        /***************************************************/

        [Description("Calculates and returns the intersection points of two Lines. If the lines are colienar and overlapping, the end points of the overlaps are returned.")]
        [Input("line1", "First Line to intersect.")]
        [Input("line2", "Second Line to intersect.")]
        [Input("useInfiniteLines", "If true or if a lines Infinite property is true, a intersection point found that is outside the domain of that lines start and end point is accepted. If false, and the lines Infinite property is false, the found intersection point needs to be on the finite line segment, between or on the start and end point.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Used for checking if the intersection point is within acceptable distance from the Lines.", typeof(Length))]
        [Output("intersections", "The intersection points of the two Lines.")]
        public static List<Point> LineIntersections(this Line line1, Line line2, bool useInfiniteLines = false, double tolerance = Tolerance.Distance)
        {
            List<Point> result = new List<Point>();

            if (!line1.IsCollinear(line2))
            {
                Point iPt = line1.LineIntersection(line2, useInfiniteLines, tolerance);
                if (iPt != null)
                    result.Add(iPt);
            }
            else if (!useInfiniteLines)
            {
                double sqTolerance = tolerance * tolerance;
                
                if (!line1.Infinite)
                {
                    if (line1.Start.SquareDistance(line2) <= sqTolerance)
                        result.Add(line1.Start);
                    if (line1.End.SquareDistance(line2) <= sqTolerance)
                        result.Add(line1.End);
                }

                if (!line2.Infinite)
                {
                    if (line2.Start.SquareDistance(line1) <= sqTolerance)
                        result.Add(line2.Start);
                    if (line2.End.SquareDistance(line1) <= sqTolerance)
                        result.Add(line2.End);
                }

                result = result.CullDuplicates(tolerance);
            }

            return result;            
        }

        /***************************************************/

        [Description("Calculates and returns all intersection from a collection of Lines.")]
        [Input("lines", "Collection of lines to get all intersections from.")]
        [Input("useInfiniteLine", "If true or if a lines Infinite property is true, a intersection point found that is outside the domain of that lines start and end point is accepted. If false, and the lines Infinite property is false, the found intersection point needs to be on the finite line segment, between or on the start and end point.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Used for checking if the intersection point is within acceptable distance from the Lines.", typeof(Length))]
        [Output("intersections", "All intersection points of collection of Lines.")]
        public static List<Point> LineIntersections(this List<Line> lines, bool useInfiniteLine = false, double tolerance = Tolerance.Distance)
        {
            // TODO: implement PointMatrix for proximity analysis
            // TODO: write the equation of each line to a list the first time it is computed?
            // TODO: if !useInfiniteLine use sweep line algo?

            List<BoundingBox> boxes = new List<BoundingBox>();
            if (!useInfiniteLine)
                boxes = lines.Select(x => x.Bounds()).ToList();

            List<Point> intersections = new List<Point>();
            for (int i = 0; i < lines.Count - 1; i++)
            {
                for (int j = i + 1; j < lines.Count; j++)
                {
                    Point result;
                    if (useInfiniteLine || Query.IsInRange(boxes[i], boxes[j], tolerance))
                    {
                        result = LineIntersection(lines[i], lines[j], useInfiniteLine, tolerance);
                        if (result != null)
                            intersections.Add(result);
                    }
                }
            }

            return intersections;
        }

        /***************************************************/

        [Description("Calculates and returns the intersection points of a Line and an Arc.")]
        [Input("arc", "Arc to intersect with the Line.")]
        [Input("line", "Line to intersect with the Arc.")]
        [Input("useInfiniteLine", "If true or if the Infinite property of the Line is true, a intersection point found that is outside the domain of the Lines start and end point is accepted. If false, and the Lines Infinite property is false, the found intersection point needs to be on the finite line segment, between or on the start and end point.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Used for checking if the intersection point is within acceptable distance from the Lines.", typeof(Length))]
        [Output("intersections", "The intersection points of the Arc and the Line.")]
        public static List<Point> LineIntersections(this Arc arc, Line line, bool useInfiniteLine = false, double tolerance = Tolerance.Distance)
        {
            Line l = line.DeepClone();
            l.Infinite = useInfiniteLine ? true : l.Infinite;

            List<Point> iPts = new List<Point>();
            Point midPoint = arc.PointAtParameter(0.5);

            Point center = arc.Centre();

            //Check if curves are coplanar
            if (Math.Abs(arc.CoordinateSystem.Z.DotProduct(l.Direction())) > Tolerance.Angle)
            {
                //Curves not coplanar
                Point pt = l.PlaneIntersection((Plane)arc.CoordinateSystem, tolerance : tolerance);
                if (pt != null && Math.Abs(pt.Distance(center) - arc.Radius) <= tolerance)
                    iPts.Add(pt);
            }
            else
            {
                //Curves coplanar
                Circle c = new Circle { Centre = center, Normal = arc.CoordinateSystem.Z, Radius = arc.Radius };
                iPts = c.LineIntersections(l, tolerance: tolerance);
            }

            List<Point> output = new List<Point>();

            double halfAngle = arc.Angle() / 2;
            double tolAngle = tolerance / arc.Radius;
            double sqrd = 2 * Math.Pow(arc.Radius, 2) * (1 - Math.Cos(Math.Abs(halfAngle + tolAngle))); // Cosine rule

            foreach (Point pt in iPts)
            {
                if ((l.Infinite || pt.Distance(l) <= tolerance) && midPoint.SquareDistance(pt) <= sqrd)
                    output.Add(pt);
            }


            return output;
        }

        /***************************************************/

        [Description("Calculates and returns the intersection points of a Line and a Circle.")]
        [Input("circle", "Circle to intersect with the Line.")]
        [Input("line", "Line to intersect with the Circle.")]
        [Input("useInfiniteLine", "If true or if the Infinite property of the Line is true, a intersection point found that is outside the domain of the Lines start and end point is accepted. If false, and the Lines Infinite property is false, the found intersection point needs to be on the finite line segment, between or on the start and end point.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Used for checking if the intersection point is within acceptable distance from the Lines.", typeof(Length))]
        [Output("intersections", "The intersection points of the Circle and the Line.")]
        public static List<Point> LineIntersections(this Circle circle, Line line, bool useInfiniteLine = false, double tolerance = Tolerance.Distance)
        {
            Line l = line.DeepClone();
            l.Infinite = useInfiniteLine ? true : l.Infinite;

            List<Point> iPts = new List<Point>();

            Plane p = new Plane { Origin = circle.Centre, Normal = circle.Normal };
            if (Math.Abs(circle.Normal.DotProduct(l.Direction())) > Tolerance.Angle)
            {   // Not Coplanar
                Point pt = l.PlaneIntersection(p, tolerance: tolerance);
                if (pt!=null && Math.Abs(pt.Distance(circle.Centre) - circle.Radius) <= tolerance)    // On Curve
                    iPts.Add(pt);
            }
            else
            {   // Coplanar
                Point pt = l.ClosestPoint(circle.Centre, true);
                double d = pt.Distance(circle.Centre);

                if (Math.Abs(circle.Radius - d) <= tolerance) // On Curve
                    iPts.Add(pt);
                else if (circle.Radius - d > 0)   // In Curve
                {
                    double o = Math.Sqrt(circle.Radius * circle.Radius - d * d);
                    Vector v = l.Direction() * o;
                    iPts.Add(pt + v);
                    iPts.Add(pt - v);
                }
            }

            if (l.Infinite)
                return iPts;

            List<Point> output = new List<Point>();
            foreach (Point pt in iPts)
            {
                if (pt.Distance(l) <= tolerance)
                    output.Add(pt);
            }

            return output;
        }

        /***************************************************/

        [Description("Calculates and returns the intersection points of a Line and a Ellipse.")]
        [Input("ellipse", "Ellipse to intersect with the Line.")]
        [Input("line", "Line to intersect with the Ellipse.")]
        [Input("useInfiniteLine", "If true or if the Infinite property of the Line is true, a intersection point found that is outside the domain of the Lines start and end point is accepted. If false, and the Lines Infinite property is false, the found intersection point needs to be on the finite line segment, between or on the start and end point.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Used for checking if the intersection point is within acceptable distance from the Lines.", typeof(Length))]
        [Output("intersections", "The intersection points of the Ellipse and the Line.")]
        public static List<Point> LineIntersections(this Ellipse ellipse, Line line, bool useInfiniteLine = false, double tolerance = Tolerance.Distance)
        {
            Line l = new Line { Start = line.Start, End = line.End, Infinite = useInfiniteLine || line.Infinite };
            List<Point> iPts = new List<Point>();

            Plane p = new Plane { Origin = ellipse.Centre, Normal = ellipse.Normal() };
            if (Math.Abs(p.Normal.DotProduct(l.Direction())) > Tolerance.Angle)
            {   // Not Coplanar
                Point pt = l.PlaneIntersection(p, tolerance: tolerance);
                if (pt != null && pt.Distance(ellipse) <= tolerance)    // On Curve
                    iPts.Add(pt);
            }
            else
            {   // Coplanar
                //Transform to ellipse coordinates
                Cartesian coordinateSystem = Create.CartesianCoordinateSystem(ellipse.Centre, ellipse.Axis1, ellipse.Axis2);
                TransformMatrix transform = Create.OrientationMatrixLocalToGlobal(coordinateSystem);
                Line lineLoc = l.Transform(transform);

                Point p1 = lineLoc.Start;
                Point p2 = lineLoc.End;
                double rx = ellipse.Radius1;
                double ry = ellipse.Radius2;

                bool tangential = false;
                bool mayBeTangential = false;
                bool isOverlapping = true;
                Point p3 = new Point();
                Point p4 = new Point();

                double dx = p2.X- p1.X;
                double dy = p2.Y- p1.Y;
                bool isSteepSlope = Math.Abs(dx) < Math.Abs(dy);

                double a, b, c, s, si;

                if (isSteepSlope)
                {
                    //Closer to vertical line, evaluate with slope from Y
                    s = dx / dy;
                    si = p2.X - (s * p2.Y);

                    a = (rx * rx) + (ry * ry * s * s);
                    b = 2.0 * ry * ry * si * s;
                    c = ry * ry * si * si - ry * ry * rx * rx;
                }
                else
                {
                    //Closer to horizontal line. Evaluate with slope from X
                    s = dy / dx;
                    si = p2.Y - (s * p2.X);

                    a = (ry * ry) + (rx * rx * s * s);
                    b = 2.0 * rx * rx * si * s;
                    c = rx * rx * si * si - rx * rx * ry * ry;
                }

                double sqrtTerm = (b * b) - (4.0 * a * c);
                double checkVal = sqrtTerm / (4 * a);
                double radicand_sqrt;

                double maxRadius = Math.Max(rx, ry);

                //This value have been found by testing to capture potential tangetial cases
                if (Math.Abs(checkVal) < tolerance * maxRadius * maxRadius * 10)
                {
                    //Intersection might be tangential, but extra (more expensive) checks are required
                    mayBeTangential = true;
                    radicand_sqrt = Math.Sqrt(Math.Abs(sqrtTerm));
                    isOverlapping = sqrtTerm > 0;
                }
                else if (checkVal < 0)  //no intersection
                    return new List<Point>();
                else
                    radicand_sqrt = Math.Sqrt(sqrtTerm);

                if (isSteepSlope)
                {
                    //Initialise values based vertical slope
                    p3.Y = (-b - radicand_sqrt) / (2.0 * a);
                    p4.Y = (-b + radicand_sqrt) / (2.0 * a);
                    p3.X = s * p3.Y + si;
                    p4.X = s * p4.Y + si;
                }
                else
                {
                    //Initialise based on non-vertical slope
                    p3.X = (-b - radicand_sqrt) / (2.0 * a);
                    p4.X = (-b + radicand_sqrt) / (2.0 * a);
                    p3.Y = s * p3.X + si;
                    p4.Y = s * p4.X + si;
                }

                double z = (p1.Z + p2.Z) / 2;
                p3.Z = z;
                p4.Z = z;

                if (mayBeTangential)
                {
                    double sqTol = tolerance * tolerance;

                    if (p3.SquareDistance(p4) <= sqTol) //Check both points within range of each other
                    {
                        p3 = (p3 + p4) / 2;
                        tangential = true;
                    }
                    else    //If not in range, check if midpoint is within tolerance distance of the ellipse
                    {
                        Point midPt = (p3 + p4) / 2;
                        Point closePt = ClosestPointEllipseLocal(rx, ry, midPt, tolerance);

                        if (midPt.SquareDistance(closePt) <= tolerance * tolerance)
                        {
                            //Is tangential
                            p3 = (midPt + closePt) / 2;
                            tangential = true;
                        }
                        else if (!isOverlapping)  //Not tangential, and not overlapping -> no intersection
                            return new List<Point>();
                    }
                }

                //Tranform back to global coordinates
                transform = Create.OrientationMatrixGlobalToLocal(coordinateSystem);
                iPts.Add(p3.Transform(transform));

                if(!tangential)
                    iPts.Add(p4.Transform(transform));
            }

            if (l.Infinite)
                return iPts;

            List<Point> output = new List<Point>();
            foreach (Point pt in iPts)
            {
                if (pt.Distance(l) <= tolerance)
                    output.Add(pt);
            }

            return output;
        }

        /***************************************************/

        [Description("Calculates and returns the intersection points of a Line and a Polyline.")]
        [Input("curve", "Polyline to intersect with the Line.")]
        [Input("line", "Line to intersect with the Polyline.")]
        [Input("useInfiniteLine", "If true or if the Infinite property of the Line is true, a intersection point found that is outside the domain of the Lines start and end point is accepted. If false, and the Lines Infinite property is false, the found intersection point needs to be on the finite line segment, between or on the start and end point.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Used for checking if the intersection point is within acceptable distance from the Lines.", typeof(Length))]
        [Output("intersections", "The intersection points of the Polyline and the Line.")]
        public static List<Point> LineIntersections(this Polyline curve, Line line, bool useInfiniteLine = false, double tolerance = Tolerance.Distance)
        {
            Line l = line.DeepClone();
            l.Infinite = useInfiniteLine ? true : line.Infinite;

            List<Point> iPts = new List<Point>();
            foreach (Line ln in curve.SubParts())
            {
                Point pt = ln.LineIntersection(l, tolerance : tolerance);
                if (pt != null)
                    iPts.Add(pt);
            }

            return iPts.CullDuplicates(tolerance);
        }

        /***************************************************/

        [Description("Calculates and returns the intersection points of a Line and a PolyCurve.")]
        [Input("curve", "PolyCurve to intersect with the Line.")]
        [Input("line", "Line to intersect with the PolyCurve.")]
        [Input("useInfiniteLine", "If true or if the Infinite property of the Line is true, a intersection point found that is outside the domain of the Lines start and end point is accepted. If false, and the Lines Infinite property is false, the found intersection point needs to be on the finite line segment, between or on the start and end point.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Used for checking if the intersection point is within acceptable distance from the Lines.", typeof(Length))]
        [Output("intersections", "The intersection points of the PolyCurve and the Line.")]
        public static List<Point> LineIntersections(this PolyCurve curve, Line line, bool useInfiniteLine = false, double tolerance = Tolerance.Distance)
        {
            Line l = line.DeepClone();
            l.Infinite = useInfiniteLine ? true : line.Infinite;

            List<Point> iPts = new List<Point>();
            foreach (ICurve c in curve.SubParts())
            {
                iPts.AddRange(c.ILineIntersections(l, tolerance: tolerance));
            }

            return iPts.CullDuplicates(tolerance);
        }

        /***************************************************/

        [Description("Calculates and returns the intersection points of two Polylines.")]
        [Input("curve1", "First Polyline to intersect with the second Polyline.")]
        [Input("curve2", "Second Polyline to intersect with the first Polyline.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Used for checking if the intersection point is within acceptable distance from the Lines.", typeof(Length))]
        [Input("angleTolerance", "Angle tolerance to be used in the method. Used for checking if the polyline segments are collinear.", typeof(Angle))]
        [Output("intersections", "The intersection points of the two Polylines.")]
        public static List<Point> LineIntersections(this Polyline curve1, Polyline curve2, double tolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            List<Point> iPts = new List<Point>();
            List<Line> subparts = curve2.SubParts();
            foreach (Line l1 in curve1.SubParts())
            {
                foreach(Line l2 in subparts)
                {
                    Point pt = l1.LineIntersection(l2, false, tolerance, angleTolerance);
                    if (pt != null)
                        iPts.Add(pt);
                }
            }

            return iPts.CullDuplicates(tolerance);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [PreviousInputNames("curve", "curve1")]
        [Description("Calculates and returns the intersection points of a Line and an ICurve.")]
        [Input("curve", "ICurve to intersect with the Line.")]
        [Input("line", "Line to intersect with the ICurve.")]
        [Input("useInfiniteLine", "If true or if the Infinite property of the Line is true, a intersection point found that is outside the domain of the Lines start and end point is accepted. If false, and the Lines Infinite property is false, the found intersection point needs to be on the finite line segment, between or on the start and end point.")]
        [Input("tolerance", "Distance tolerance to be used in the method. Used for checking if the intersection point is within acceptable distance from the Lines.", typeof(Length))]
        [Output("intersections", "The intersection points of the ICurve and the Line.")]
        public static List<Point> ILineIntersections(this ICurve curve, Line line, bool useInfiniteLine = false, double tolerance = Tolerance.Distance)
        {
            if (curve == null || line == null)
                return new List<Point>();

            return LineIntersections(curve as dynamic, line, useInfiniteLine, tolerance);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        [Description("Fallback method for curve types not yet supported.")]
        private static List<Point> LineIntersections(this ICurve curve, Line line, bool useInfiniteLine = false, double tolerance = Tolerance.Distance)
        {
            Base.Compute.RecordError($"LineIntersections is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}



