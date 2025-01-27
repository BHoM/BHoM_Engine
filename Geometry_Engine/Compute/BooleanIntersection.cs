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

using BH.Engine.Base;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /****      public Methods - Bounding Boxes      ****/
        /***************************************************/

        [Description("Creates and returns the boolean intersection of all provided bounding boxes, which is a new Bounding box that is the overlap of all of the provided boxes.")]
        [Input("boxes", "Collection of bounding boxes to compute the intersection of.")]
        [Input("tolerance", "Tolerance to be used to check if the overlap is within acceptable range.", typeof(Length))]
        [Output("intersection", "The bounding box that is the intersection of all of the provided boxes.")]
        public static BoundingBox BooleanIntersection(this List<BoundingBox> boxes, double tolerance = Tolerance.Distance)
        {
            double minX = double.MinValue;
            double minY = double.MinValue;
            double minZ = double.MinValue;
            double maxX = double.MaxValue;
            double maxY = double.MaxValue;
            double maxZ = double.MaxValue;

            foreach (BoundingBox box in boxes)
            {
                minX = Math.Max(box.Min.X, minX);
                minY = Math.Max(box.Min.Y, minY);
                minZ = Math.Max(box.Min.Z, minZ);
                maxX = Math.Min(box.Max.X, maxX);
                maxY = Math.Min(box.Max.Y, maxY);
                maxZ = Math.Min(box.Max.Z, maxZ);

                if (minX > maxX + tolerance || minY > maxY + tolerance || minZ > maxZ + tolerance)
                    return null;
            }

            return new BoundingBox { Min = new Point { X = minX, Y = minY, Z = minZ }, Max = new Point { X = maxX, Y = maxY, Z = maxZ } };
        }

        /***************************************************/
        /****          public Methods - Lines           ****/
        /***************************************************/

        [Description("Computes the boolean intersection of two lines, e.g. the overlap of the lines and returns a new line matching this overlap. If the curves are not overlapping null will be returned.")]
        [Input("line", "First line to intersect.")]
        [Input("refLine", "Second line ot intersect.")]
        [Input("tolerance", "Tolerance to be used in the method.", typeof(Length))]
        [Output("intersection", "The line corresponding to the overlap of the two provided lines or null if no overlap could be found.")]
        public static Line BooleanIntersection(this Line line, Line refLine, double tolerance = Tolerance.Distance)
        {
            List<Line> result = new List<Line> { line }.BooleanIntersection(new List<Line> { refLine }, tolerance);
            return result.Count == 0 ? null : result[0];
        }

        /***************************************************/

        [Description("Computes the boolean intersection of a line and a collection of other lines, e.g. all overlaps of the first line and the reference lines and returns a new collection of lines matching the overlap.")]
        [Input("line", "First line to intersect.")]
        [Input("refLines", "A collection of reference lines to intersect with the first line.")]
        [Input("tolerance", "Tolerance to be used in the method.", typeof(Length))]
        [Output("intersection", "The collection of lines corresponding to the overlaps of the first line and any of the reference lines.")]
        public static List<Line> BooleanIntersection(this Line line, List<Line> refLines, double tolerance = Tolerance.Distance)
        {
            return new List<Line> { line }.BooleanIntersection(refLines, tolerance);
        }

        /***************************************************/


        [Description("Computes the boolean intersection of two collections of lines, e.g. all overlaps of the first set of lines with the reference lines and returns a new collection of lines matching the overlap.")]
        [Input("lines", "First list of lines to intersect.")]
        [Input("refLines", "A list of reference lines to intersect with the first list of lines.")]
        [Input("tolerance", "Tolerance to be used in the method.", typeof(Length))]
        [Output("intersection", "The list of lines corresponding to the overlaps of the first list of lines and any of the reference lines.")]
        public static List<Line> BooleanIntersection(this List<Line> lines, List<Line> refLines, double tolerance = Tolerance.Distance)
        {
            double sqTol = tolerance * tolerance;
            lines = lines.Where(x => x != null && x.SquareLength() > sqTol).ToList();
            List<Line> refLeft = refLines.Where(x => x != null && x.SquareLength() > sqTol).ToList();
            
            List<Line> result = new List<Line>();
            foreach (var cluster in lines.ClusterCollinear(tolerance))
            {
                List<Line> toIntersect = new List<Line>();
                for (int i = refLeft.Count - 1; i >= 0; i--)
                {
                    if (refLeft[i].IsCollinear(cluster[0]))
                    {
                        toIntersect.Add(refLeft[i]);
                        refLeft.RemoveAt(i);
                    }
                }

                if (toIntersect.Count != 0)
                    result.AddRange(cluster.BooleanIntersectionCollinear(toIntersect, tolerance));
            }

            return result;
        }

        /***************************************************/

        [Description("Computes the boolean intersection of a collection of lines, e.g. the overlap of all the provided lines and returns this overlap as a new line.")]
        [Input("lines", "The list of lines to intersect.")]
        [Input("tolerance", "Tolerance to be used in the method.", typeof(Length))]
        [Output("intersection", "The line corresponding to the overlap all of the provided lines.")]
        public static Line BooleanIntersection(this List<Line> lines, double tolerance = Tolerance.Distance)
        {
            if (lines.IsCollinear(tolerance))
                return lines.BooleanIntersectionCollinear(tolerance);
            else
                return null;
        }



        /***************************************************/
        /****         public Methods - Regions          ****/
        /***************************************************/

        [Description("Computes the boolean intersection of two closed coplanar polyline regions, e.g. the overlap(s) of the regions and returns collection of Polylines matching the overlap(s).")]
        [Input("region", "First region to intersect.")]
        [Input("refRegion", "Second region ot intersect.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Input("angleTolerance", "Angle tolerance to be used in the method.", typeof(Length))]
        [Output("intersection", "The Polyline region(s) corresponding to the overlap(s) of the two provided regions.")]
        public static List<Polyline> BooleanIntersection(this Polyline region, Polyline refRegion, double tolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            if (!region.IsClosed(tolerance) || !refRegion.IsClosed(tolerance))
            {
                Base.Compute.RecordError("Boolean Union works on closed regions.");
                return new List<Polyline>();
            }

            if (!region.IsCoplanar(refRegion, tolerance))
                return new List<Polyline>();

            double sqTol = tolerance * tolerance;
            List<Polyline> tmpResult = new List<Polyline>();
            List<Point> iPts = region.LineIntersections(refRegion, tolerance, angleTolerance);

            List<Polyline> splitRegion1 = region.SplitAtPoints(iPts, tolerance);
            List<Polyline> splitRegion2 = refRegion.SplitAtPoints(iPts, tolerance);

            foreach (Polyline segment in splitRegion1)
            {
                List<Point> mPts = new List<Point> { segment.IPointAtParameter(0.5) };

                if (refRegion.IsContaining(mPts, true, tolerance))
                    tmpResult.Add(segment);
            }

            foreach (Polyline segment in splitRegion2)
            {
                List<Point> mPts = new List<Point> { segment.IPointAtParameter(0.5) };

                if (region.IsContaining(mPts, true, tolerance))
                    tmpResult.Add(segment);
            }

            bool regSameDir = false;
            if (Math.Abs(region.Normal().DotProduct(refRegion.Normal()) - 1) <= tolerance)
                regSameDir = true;

            for (int i = 0; i < tmpResult.Count; i++)
            {
                for (int j = 0; j < tmpResult.Count; j++)
                {
                    if (i != j && tmpResult[i].IsSimilarSegment(tmpResult[j], tolerance))
                    {
                        bool sameDir = tmpResult[i].TangentAtParameter(0.5).IsEqual(tmpResult[j].TangentAtParameter(0.5));
                        if ((regSameDir && sameDir) || (!regSameDir && !sameDir))
                        {
                            tmpResult.RemoveAt(Math.Min(j, i));
                            if (i > j)
                                i--;
                            else
                                j = 0;
                        }
                        else
                        {
                            tmpResult.RemoveAt(Math.Max(j, i));
                            tmpResult.RemoveAt(Math.Min(j, i));
                            if (i > 0)
                                i--;
                            else
                                j = 0;
                        }
                    }
                }
            }

            List<Polyline> result = Join(tmpResult, tolerance).Select(x => x.Close(tolerance)).ToList();

            int res = 0;
            while (res < result.Count)
            {
                if (result[res].Area() <= sqTol)
                    result.RemoveAt(res);
                else
                    res++;
            }

            return result;
        }

        /***************************************************/

        [Description("Computes the boolean intersection of a collection of Polyline regions, e.g. the overlap(s) of all the provided regions and returns collection of Polylines matching the overlap(s).")]
        [Input("regions", "The collection of Polyline regions to intersect.")]
        [Input("tolerance", "Tolerance to be used in the method.", typeof(Length))]
        [Output("intersection", "The Polyline region(s) corresponding to the overlap(s) of all of the provided regions.")]
        public static List<Polyline> BooleanIntersection(this List<Polyline> regions, double tolerance = Tolerance.Distance)
        {
            if (regions.Count < 2)
                return regions;

            List<Polyline> result = new List<Polyline>();

            foreach (Polyline region in regions)
            {
                if (region.Area() <= tolerance)
                    return result;
            }

            result.Add(regions[0]);
            for (int i = 1; i < regions.Count; i++)
            {
                List<Polyline> newResult = new List<Polyline>();
                result.ForEach(r => newResult.AddRange(r.BooleanIntersection(regions[i], tolerance)));
                result = newResult;
            }

            return result;
        }

        /***************************************************/

        [Description("Computes the boolean intersection of two closed coplanar curve regions, e.g. the overlap(s) of the regions and returns collection of Polycurves matching the overlap(s).")]
        [Input("region", "First region to intersect.")]
        [Input("refRegion", "Second region ot intersect.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("intersection", "The Polycurve region(s) corresponding to the overlap(s) of the two provided regions.")]
        public static List<PolyCurve> BooleanIntersection(this ICurve region, ICurve refRegion, double tolerance = Tolerance.Distance)
        {
            if (region is NurbsCurve || region is Ellipse || refRegion is NurbsCurve || refRegion is Ellipse)
            {
                Base.Compute.RecordError("NurbsCurves and ellipses are not implemented for BooleanIntersection.");
                return null;
            }

            if (!region.IIsClosed(tolerance) || !refRegion.IIsClosed(tolerance))
            {
                Base.Compute.RecordError("Boolean Intersection works on closed regions.");
                return new List<PolyCurve>();
            }

            if (!region.IIsCoplanar(refRegion, tolerance))
                return new List<PolyCurve>();

            double sqTol = tolerance * tolerance;
            List<ICurve> tmpResult = new List<ICurve>();
            List<Point> iPts = region.ICurveIntersections(refRegion, tolerance);
            List<ICurve> splitRegion1 = new List<ICurve>();
            List<ICurve> splitRegion2 = new List<ICurve>();

            if (iPts.Count == 0)
            {
                splitRegion1.Add(region);
                splitRegion2.Add(refRegion);
            }
            else
            {
                splitRegion1.AddRange(region.ISplitAtPoints(iPts, tolerance));
                splitRegion2.AddRange(refRegion.ISplitAtPoints(iPts, tolerance));
            }

            foreach (ICurve segment in splitRegion1)
            {
                List<Point> mPts = new List<Point> { segment.IPointAtParameter(0.5) };

                if (refRegion.IIsContaining(mPts, true, tolerance))
                    tmpResult.Add(segment);
            }

            foreach (ICurve segment in splitRegion2)
            {
                List<Point> cPts = new List<Point> { segment.IPointAtParameter(0.5) };

                if (region.IIsContaining(cPts, true, tolerance))
                    tmpResult.Add(segment);
            }
             
            bool regSameDir = false;
            if (Math.Abs(region.INormal().DotProduct(refRegion.INormal()) - 1) <= tolerance)
                regSameDir = true;

            for (int i = 0; i < tmpResult.Count; i++)
            {
                 for (int j = 0; j < tmpResult.Count; j++)
                {
                    if (i != j && tmpResult[i].IsSimilarSegment(tmpResult[j], tolerance))
                    {
                        bool sameDir = tmpResult[i].ITangentAtParameter(0.5).IsEqual(tmpResult[j].ITangentAtParameter(0.5));
                        if ((regSameDir && sameDir) || (!regSameDir && !sameDir))
                        {
                            tmpResult.RemoveAt(Math.Min(j, i));
                            if (i > j)
                                i--;
                            else
                                j = 0;
                        }
                        else
                        {
                            tmpResult.RemoveAt(Math.Max(j, i));
                            tmpResult.RemoveAt(Math.Min(j, i));
                            if (i > 0)
                                i--;
                            else
                                j = 0;
                        }
                    }                        
                }
            }

            List<PolyCurve> result = IJoin(tmpResult, tolerance).Select(x => x.Close(tolerance)).ToList();

            int res = 0;
            while (res < result.Count)
            {
                if (result[res].Area() <= sqTol || !result[res].IIsClosed(tolerance))
                    result.RemoveAt(res);
                else
                    res++;
            }

            return result;

        }

        /***************************************************/

        [Description("Computes the boolean intersection of a collection of curve regions, e.g. the overlap(s) of all the provided regions and returns collection of PolyCurve regions matching the overlap(s).")]
        [Input("regions", "The collection of Curve regions to intersect.")]
        [Input("tolerance", "Tolerance to be used in the method.", typeof(Length))]
        [Output("intersection", "The PolyCurve region(s) corresponding to the overlap(s) of all of the provided regions.")]
        public static List<PolyCurve> BooleanIntersection(this IEnumerable<ICurve> regions, double tolerance = Tolerance.Distance)
        {
            List<ICurve> regionsList = regions.ToList();

            List<PolyCurve> regionListPolyCurve = new List<PolyCurve>();

            foreach (ICurve curve in regionsList)
            {
                if (curve is PolyCurve)
                    regionListPolyCurve.Add(curve as PolyCurve);
                else
                    regionListPolyCurve.Add(new PolyCurve { Curves = curve.ISubParts().ToList() });
            }

            if (regionListPolyCurve.Count < 2)
                return regionListPolyCurve;

            List<PolyCurve> result = new List<PolyCurve>();

            foreach (PolyCurve region in regionListPolyCurve)
            {
                if (region.Area() <= tolerance)
                    return result;
            }

            result.Add(regionListPolyCurve[0]);
            for (int i = 1; i < regionListPolyCurve.Count; i++)
            {
                result = result.SelectMany(x => x.BooleanIntersection(regionListPolyCurve[i], tolerance)).ToList();
            }

            return result;
        }


        /***************************************************/
        /***          Private methods                    ***/
        /***************************************************/

        [Description("Checks if two segments are similar by comparing the start and end points (allowing for reversed order, i.e. start against end) as well as the mid points.")]
        private static bool IsSimilarSegment(this ICurve curve, ICurve refCurve, double tolerance = Tolerance.Distance)
        {
            double sqTol = tolerance * tolerance;
            Point sp = curve.IStartPoint();
            Point rsp = refCurve.IStartPoint();
            Point ep = curve.IEndPoint();
            Point rep = refCurve.IEndPoint();
            if ((sp.SquareDistance(rsp) <= sqTol && ep.SquareDistance(rep) <= sqTol) ||
                   (sp.SquareDistance(rep) <= sqTol && ep.SquareDistance(rsp) <= sqTol))
            {
                Point mp = curve.IPointAtParameter(0.5);
                Point rmp = refCurve.IPointAtParameter(0.5);
                return mp.SquareDistance(rmp) <= sqTol;
            }
            return false;
        }

        /***************************************************/

        private static List<Line> BooleanIntersectionCollinear(this List<Line> lines, List<Line> lines2, double tolerance)
        {
            Vector dir = lines[0].Direction();
            (Point, Point) extents1 = lines.Extents(dir, tolerance);
            (Point, Point) extents2 = lines2.Extents(dir, tolerance);
            Point min = (extents1.Item1 - extents2.Item1).DotProduct(dir) < 0 ? extents1.Item1 : extents2.Item1;

            List<(double, double)> ranges1 = lines.SortedDomains(min, tolerance);
            List<(double, double)> ranges2 = lines2.SortedDomains(min, tolerance);

            List<(double, double)> mergedRanges1 = ranges1.MergeRanges(tolerance);
            List<(double, double)> mergedRanges2 = ranges2.MergeRanges(tolerance);

            List<(double, double)> intersectedRanges = new List<(double, double)>();
            int i = 0;
            foreach ((double, double) range in mergedRanges1)
            {
                for (; i < mergedRanges2.Count; i++)
                {
                    if (range.Item1 - mergedRanges2[i].Item2 < -tolerance && range.Item2 - mergedRanges2[i].Item1 > tolerance)
                        intersectedRanges.Add((Math.Max(range.Item1, mergedRanges2[i].Item1), Math.Min(range.Item2, mergedRanges2[i].Item2)));

                    if (range.Item2 - mergedRanges2[i].Item2 < tolerance)
                        break;
                }
            }

            return intersectedRanges.Select(x => new Line { Start = min + dir * x.Item1, End = min + dir * x.Item2 }).ToList();
        }

        /***************************************************/

        private static Line BooleanIntersectionCollinear(this List<Line> lines, double tolerance)
        {
            Line dirLine = lines.Select(x => x.Start).Union(lines.Select(x => x.End)).FitLine(tolerance);
            Vector dir = dirLine.Direction(tolerance);
            (Point, Point) extents = lines.Extents(dir, tolerance);
            Point min = dirLine.ClosestPoint(extents.Item1, true);

            List<(double, double)> ranges = lines.SortedDomains(min, tolerance);
            double maxStart = ranges.Select(x => x.Item1).OrderByDescending(x => x).First();
            double minEnd = ranges.Select(x => x.Item2).OrderBy(x => x).First();

            if (minEnd - maxStart > tolerance)
                return new Line { Start = min + dir * maxStart, End = min + dir * minEnd };
            else
                return null;
        }

        /***************************************************/

    }
}






