/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
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

            Line l1 = line1.Clone();
            Line l2 = line2.Clone();
            l1.Infinite |= useInfiniteLines;
            l2.Infinite |= useInfiniteLines;
            BH.oM.Reflection.Output<double, double> intParamsOutput = l1.SkewLineProximity(l2, angleTolerance);

            if (intParamsOutput == null)
                return null;
            else
            {
                double[] intParams = new double[] { intParamsOutput.Item1, intParamsOutput.Item2 };
                double t1 = intParams[0];
                double t2 = intParams[1];
                
                Point intPt1 = l1.Start + t1 * (l1.End - l1.Start);
                Point intPt2 = l2.Start + t2 * (l2.End - l2.Start);

                if (intPt1.SquareDistance(intPt2) <= sqTol)
                {
                    Point intPt = (intPt1 + intPt2) * 0.5;
                    if (!l1.Infinite && ((t1 < 0 && l1.Start.SquareDistance(intPt) > sqTol) || (t1 > 1 && l1.End.SquareDistance(intPt) > sqTol)))
                        return null;
                    if (!l2.Infinite && ((t2 < 0 && l2.Start.SquareDistance(intPt) > sqTol) || (t2 > 1 && l2.End.SquareDistance(intPt) > sqTol)))
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
            Line l = line.Clone();
            l.Infinite = useInfiniteLine ? true : l.Infinite;

            double sqtol = tolerance * tolerance;
            List<Point> iPts = new List<Point>();
            Point midPoint = arc.PointAtParameter(0.5);

            Point center = arc.Centre();
            double sqrRadius = arc.Radius * arc.Radius;

            //Check if curves are coplanar
            if (Math.Abs(arc.CoordinateSystem.Z.DotProduct(l.Direction())) > Tolerance.Angle)
            {
                //Curves not coplanar
                Point pt = l.PlaneIntersection((Plane)arc.CoordinateSystem);
                if (pt != null && Math.Abs(pt.SquareDistance(center) - sqrRadius) <= sqtol)
                    iPts.Add(pt);
            }
            else
            {
                //Curves coplanar
                Circle c = new Circle { Centre = center, Normal = arc.CoordinateSystem.Z, Radius = arc.Radius };
                iPts = c.LineIntersections(l);
            }

            List<Point> output = new List<Point>();
            double sqrd = midPoint.SquareDistance(arc.IStartPoint());
            {
                foreach (Point pt in iPts)
                {
                    if ((l.Infinite || pt.Distance(l) <= tolerance) && midPoint.SquareDistance(pt) <= sqrd + sqtol)
                        output.Add(pt);
                }
            }

            return output;
        }

        /***************************************************/

        public static List<Point> LineIntersections(this Circle circle, Line line, bool useInfiniteLine = false, double tolerance = Tolerance.Distance)
        {
            Line l = line.Clone();
            l.Infinite = useInfiniteLine ? true : l.Infinite;

            double sqtol = tolerance * tolerance;
            List<Point> iPts = new List<Point>();

            Plane p = new Plane { Origin = circle.Centre, Normal = circle.Normal };
            if (Math.Abs(circle.Normal.DotProduct(l.Direction())) > Tolerance.Angle)
            {   // Not Coplanar
                Point pt = l.PlaneIntersection(p);
                if (pt!=null && Math.Abs(pt.SquareDistance(circle.Centre) - circle.Radius * circle.Radius) <= sqtol)    // On Curve
                    iPts.Add(pt);
            }
            else
            {   // Coplanar
                Point pt = l.ClosestPoint(circle.Centre, true);
                double sqrDiff = circle.Radius * circle.Radius - pt.SquareDistance(circle.Centre);

                if (Math.Abs(sqrDiff) <= sqtol) // On Curve
                    iPts.Add(pt);
                else if (sqrDiff > 0)   // In Curve
                {
                    double o = Math.Sqrt(sqrDiff);
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
            Line l = line.Clone();
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
            Line l = line.Clone();
            l.Infinite = useInfiniteLine ? true : line.Infinite;

            List<Point> iPts = new List<Point>();
            foreach (ICurve c in curve.SubParts())
            {
                iPts.AddRange(c.ILineIntersections(l));
            }

            return iPts.CullDuplicates(tolerance);
        }

        /***************************************************/

        public static List<Point> LineIntersections(this Polyline curve1, Polyline curve2, double tolerance = Tolerance.Distance)
        {
            List<Point> iPts = new List<Point>();
            List<Line> subparts = curve2.SubParts();
            foreach (Line l1 in curve1.SubParts())
            {
                foreach(Line l2 in subparts)
                {
                    Point pt = l1.LineIntersection(l2);
                    if (pt != null)
                        iPts.Add(pt);
                }
            }

            return iPts.CullDuplicates(tolerance);
        }

        /***************************************************/

        [NotImplemented]
        public static List<Point> LineIntersections(this NurbsCurve curve, Line line, bool useInfiniteLine = false, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<Point> ILineIntersections(this ICurve curve1, Line line, bool useInfiniteLine = false, double tolerance = Tolerance.Distance)
        {
            return LineIntersections(curve1 as dynamic, line, useInfiniteLine, tolerance);
        }

        /***************************************************/
    }
}
