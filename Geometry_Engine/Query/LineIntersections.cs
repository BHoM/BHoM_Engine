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

using BH.Engine.Base;
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
        /**** Public Methods                            ****/
        /***************************************************/

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
                    if (useInfiniteLine || Query.IsInRange(boxes[i], boxes[j]))
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
                Point pt = l.PlaneIntersection((Plane)arc.CoordinateSystem);
                if (pt != null && Math.Abs(pt.Distance(center) - arc.Radius) <= tolerance)
                    iPts.Add(pt);
            }
            else
            {
                //Curves coplanar
                Circle c = new Circle { Centre = center, Normal = arc.CoordinateSystem.Z, Radius = arc.Radius };
                iPts = c.LineIntersections(l);
            }

            List<Point> output = new List<Point>();
            
            double halfAngle = arc.Angle() / 2;
            double tolAngle = tolerance / arc.Radius;
            double sqrd = 2 * Math.Pow(arc.Radius, 2) * (1 - Math.Cos(Math.Abs(halfAngle + tolAngle))); // Cosine rule
            {
                foreach (Point pt in iPts)
                {
                    if ((l.Infinite || pt.Distance(l) <= tolerance) && midPoint.SquareDistance(pt) <= sqrd )
                        output.Add(pt);
                }
            }

            return output;
        }

        /***************************************************/

        public static List<Point> LineIntersections(this Circle circle, Line line, bool useInfiniteLine = false, double tolerance = Tolerance.Distance)
        {
            Line l = line.DeepClone();
            l.Infinite = useInfiniteLine ? true : l.Infinite;

            List<Point> iPts = new List<Point>();

            Plane p = new Plane { Origin = circle.Centre, Normal = circle.Normal };
            if (Math.Abs(circle.Normal.DotProduct(l.Direction())) > Tolerance.Angle)
            {   // Not Coplanar
                Point pt = l.PlaneIntersection(p);
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

        public static List<Point> LineIntersections(this Polyline curve, Line line, bool useInfiniteLine = false, double tolerance = Tolerance.Distance)
        {
            Line l = line.DeepClone();
            l.Infinite = useInfiniteLine ? true : line.Infinite;

            List<Point> iPts = new List<Point>();
            foreach (Line ln in curve.SubParts())
            {
                Point pt = ln.LineIntersection(l);
                if (pt != null)
                    iPts.Add(pt);
            }

            return iPts.CullDuplicates(tolerance);
        }

        /***************************************************/

        public static List<Point> LineIntersections(this PolyCurve curve, Line line, bool useInfiniteLine = false, double tolerance = Tolerance.Distance)
        {
            Line l = line.DeepClone();
            l.Infinite = useInfiniteLine ? true : line.Infinite;

            List<Point> iPts = new List<Point>();
            foreach (ICurve c in curve.SubParts())
            {
                iPts.AddRange(c.ILineIntersections(l));
            }

            return iPts.CullDuplicates(tolerance);
        }

        /***************************************************/

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

        public static List<Point> ILineIntersections(this ICurve curve1, Line line, bool useInfiniteLine = false, double tolerance = Tolerance.Distance)
        {
            return LineIntersections(curve1 as dynamic, line, useInfiniteLine, tolerance);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static List<Point> LineIntersections(this ICurve curve, Line line, bool useInfiniteLine = false, double tolerance = Tolerance.Distance)
        {
            Base.Compute.RecordError($"LineIntersections is not implemented for ICurves of type: {curve.GetType().Name}.");
            return null;
        }

        /***************************************************/
    }
}


