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

using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /****          Public Methods - Lines           ****/
        /***************************************************/

        [Description("Joins overlapping lines into single segments, i.e. if the lines overlap, a single line spanning between the extremes will be returned, otherwise both lines are returned separate.")]
        [Input("line", "First line to union.")]
        [Input("refLine", "Second line to union.")]
        [Input("tolerance", "Tolerance used for checking colinearity and proximity of the lines.", typeof(Length))]
        [Output("union", "Boolean union of the lines.")]
        public static List<Line> BooleanUnion(this Line line, Line refLine, double tolerance = Tolerance.Distance, bool keepIntermediatePoints = false)
        {
            double sqTol = tolerance * tolerance;
            List<Line> result = new Line[] { line, refLine }.Where(x => x != null && x.SquareLength() > sqTol).ToList();
            if (line.IsCollinear(refLine, tolerance))
                result = result.BooleanUnionCollinear(tolerance, keepIntermediatePoints);

            return result;
        }

        /***************************************************/

        [Description("Joins overlapping lines into single segments, i.e. each set of collinear lines that overlap with each other will be returned as a single line spanning between the extremes of the set.")]
        [Input("lines", "Lines to union.")]
        [Input("tolerance", "Tolerance used for checking colinearity and proximity of the lines.", typeof(Length))]
        [Output("union", "Boolean union of the lines.")]
        public static List<Line> BooleanUnion(this List<Line> lines, double tolerance = Tolerance.Distance, bool keepIntermediatePoints = false)
        {
            double sqTol = tolerance * tolerance;
            return lines.Where(x => x != null && x.SquareLength() > sqTol)
                        .ToList()
                        .ClusterCollinear(tolerance)
                        .Select(x => x.BooleanUnionCollinear(tolerance, keepIntermediatePoints))
                        .SelectMany(x => x)
                        .ToList();
        }


        /***************************************************/
        /****         public Methods - Regions          ****/
        /***************************************************/

        [Description("Computes the boolean union of a set of closed coplanar curve regions, i.e. overlaps the regions and returns a collection of combined regions.")]
        [Input("regions", "Regions to union.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("union", "Boolean union of the regions.")]
        public static List<Polyline> BooleanUnion(this List<Polyline> regions, double tolerance = Tolerance.Distance)
        {
            if (regions.Count < 2)
                return regions;

            if (regions.Any(x => !x.IsClosed(tolerance)))
            {
                Base.Compute.RecordError("Boolean Union works on closed regions.");
                return regions;
            }

            for (int i = 0; i < regions.Count; i++)
            {
                for (int j = 0; j < regions.Count; j++)
                {
                    if (i != j && regions[i].IsContaining(regions[j], true, tolerance))
                    {
                        regions.RemoveAt(j);
                        i = -1;
                        break;
                    }
                }
            }

            List<Polyline> result = new List<Polyline>();
            double sqTolerance = tolerance * tolerance;

            foreach (List<Polyline> regionCluster in regions.ClusterCoplanar(tolerance))
            {
                Vector normal = regionCluster[0].INormal();

                for (int i = 0; i < regionCluster.Count; i++)
                    if (!regionCluster[i].IsClockwise(normal))
                        regionCluster[i] = regionCluster[i].Flip();

                List<Polyline> tmpResult = new List<Polyline>();

                List<Point>[] iPts = new List<Point>[regionCluster.Count];
                for (int i = 0; i < iPts.Length; i++)
                {
                    iPts[i] = new List<Point>();
                }

                List<BoundingBox> regionBounds = regionCluster.Select(x => x.Bounds()).ToList();
                for (int j = 0; j < regionCluster.Count - 1; j++)
                {
                    regionCluster[j] = regionCluster[j].CleanPolyline(tolerance);
                    for (int k = j + 1; k < regionCluster.Count; k++)
                    {
                        regionCluster[k] = regionCluster[k].CleanPolyline(tolerance);
                        if (regionBounds[j].IsInRange(regionBounds[k]))
                        {
                            List<Point> intPts = regionCluster[j].LineIntersections(regionCluster[k], tolerance);
                            iPts[j].AddRange(intPts);
                            iPts[k].AddRange(intPts);
                        }
                    }
                }

                for (int i = 0; i < regionCluster.Count; i++)
                {
                    if (iPts[i].Count() > 1)
                    {
                        List<Polyline> splReg = regionCluster[i].SplitAtPoints(iPts[i]);
                        foreach (Polyline segment in splReg)
                        {
                            bool flag = true;
                            List<Point> mPts = new List<Point> { segment.PointAtParameter(0.5) };

                            for (int j = 0; j < regionCluster.Count; j++)
                            {
                                if (i != j && regionBounds[j].IsContaining(mPts) && regionCluster[j].IsContaining(mPts, false, tolerance))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                                tmpResult.Add(segment);
                        }
                    }
                    else
                        result.Add(regionCluster[i]);
                }

                for (int i = 0; i < tmpResult.Count; i++)
                {
                    for (int j = 0; j < tmpResult.Count; j++)
                    {
                        if (i != j && tmpResult[i].IsSimilarSegment(tmpResult[j], tolerance))
                        {
                            if (tmpResult[i].TangentAtParameter(0.5).IsEqual(tmpResult[j].TangentAtParameter(0.5)))
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

                result.AddRange(Join(tmpResult, tolerance));
            }

            int res = 0;
            while (res < result.Count)
            {
                if (result[res].Area() <= sqTolerance)
                    result.RemoveAt(res);
                else
                    res++;
            }

            return result;
        }

        /***************************************************/

        [Description("Computes the boolean union of a set of closed coplanar curve regions, i.e. overlaps the regions and returns a collection of combined regions.")]
        [Input("regions", "Regions to union.")]
        [Input("tolerance", "Distance tolerance to be used in the method.", typeof(Length))]
        [Output("union", "Boolean union of the regions.")]
        public static List<PolyCurve> BooleanUnion(this IEnumerable<ICurve> regions, double tolerance = Tolerance.Distance)
        {
            List<ICurve> regionsList = regions.ToList();

            if (regionsList.Any(x => x is NurbsCurve || x is Ellipse))
            {
                Base.Compute.RecordError("NurbsCurves and ellipses are not implemented for BooleanUnion.");
                return null;
            }

            List<PolyCurve> result = new List<PolyCurve>();

            if (regionsList.Count == 0)
                return result;
            else if (regionsList.Count == 1)
            {
                if (regionsList[0] is PolyCurve)
                {
                    result.Add(regionsList[0] as PolyCurve);
                    return result;
                }
                else
                {
                    result.Add(new PolyCurve { Curves = regionsList[0].ISubParts().ToList() });
                    return result;
                }
            }

            if (regionsList.Any(x => !x.IIsClosed(tolerance)))
            {
                Base.Compute.RecordError("Boolean Union works on closed regions.");
                foreach (ICurve curve in regionsList)
                {
                    if (curve is PolyCurve)
                        result.Add(curve as PolyCurve);
                    else
                        result.Add(new PolyCurve { Curves = curve.ISubParts().ToList() });
                }
                return result;
            }

            for (int i = 0; i < regionsList.Count; i++)
            {
                for (int j = 0; j < regionsList.Count; j++)
                {
                    if (i != j && regionsList[i].IIsContaining(regionsList[j], true, tolerance))
                    {
                        regionsList.RemoveAt(j);
                        i = -1;
                        break;
                    }
                }
            }

            List<ICurve> resultICurve = new List<ICurve>();
            double sqTolerance = tolerance * tolerance;

            foreach (List<ICurve> regionCluster in regionsList.IClusterCoplanar(tolerance))
            {
                Vector normal = regionCluster[0].INormal();

                for (int i = 0; i < regionCluster.Count; i++)
                    if (!regionCluster[i].IIsClockwise(normal))
                        regionCluster[i] = regionCluster[i].IFlip();

                List<ICurve> tmpResult = new List<ICurve>();

                List<Point>[] iPts = new List<Point>[regionCluster.Count];
                for (int i = 0; i < iPts.Length; i++)
                {
                    iPts[i] = new List<Point>();
                }

                List<BoundingBox> regionBounds = regionCluster.Select(x => x.IBounds()).ToList();
                for (int j = 0; j < regionCluster.Count - 1; j++)
                {
                    for (int k = j + 1; k < regionCluster.Count; k++)
                    {
                        if (regionBounds[j].IsInRange(regionBounds[k]))
                        {
                            List<Point> intPts = regionCluster[j].ICurveIntersections(regionCluster[k], tolerance);
                            iPts[j].AddRange(intPts);
                            iPts[k].AddRange(intPts);
                        }
                    }
                }

                for (int i = 0; i < regionCluster.Count; i++)
                {
                    if (iPts[i].Count() > 1)
                    {
                        List<ICurve> splReg = regionCluster[i].ISplitAtPoints(iPts[i]);
                        foreach (ICurve segment in splReg)
                        {
                            bool flag = true;
                            List<Point> mPts = new List<Point> { segment.IPointAtParameter(0.5) };

                            for (int j = 0; j < regionCluster.Count; j++)
                            {
                                if (i != j && regionBounds[j].IsContaining(mPts) && regionCluster[j].IIsContaining(mPts, false, tolerance))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                            if (flag)
                                tmpResult.Add(segment);
                        }
                    }
                    else
                        resultICurve.Add(regionCluster[i]);
                }

                for (int i = 0; i < tmpResult.Count; i++)
                {
                    for (int j = 0; j < tmpResult.Count; j++)
                    {
                        if (i != j && tmpResult[i].IsSimilarSegment(tmpResult[j], tolerance))
                        {
                            if (tmpResult[i].ITangentAtParameter(0.5).IsEqual(tmpResult[j].ITangentAtParameter(0.5)))
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

                result.AddRange(IJoin(tmpResult, tolerance));
            }

            foreach (ICurve curve in resultICurve)
            {
                if (curve is PolyCurve)
                    result.Add(curve as PolyCurve);
                else
                    result.Add(new PolyCurve { Curves = curve.ISubParts().ToList() });
            }

            int res = 0;
            while (res < result.Count)
            {
                if (result[res].Area() <= sqTolerance)
                    result.RemoveAt(res);
                else
                    res++;
            }

            return result;
        }


        /***************************************************/
        /***          Private methods                    ***/
        /***************************************************/

        private static (Point, Point) Extents(this List<Line> collinearLines, Vector dir, double tolerance)
        {
            List<Point> startPoints = collinearLines.Select(x => x.Start).ToList();
            List<Point> endPoints = collinearLines.Select(x => x.End).ToList();
            List<Point> sorted;
            if (Math.Abs(dir.X) > tolerance)
                sorted = startPoints.Union(endPoints).OrderBy(x => x.X).ToList();
            else if (Math.Abs(dir.Y) > tolerance)
                sorted = startPoints.Union(endPoints).OrderBy(x => x.Y).ToList();
            else
                sorted = startPoints.Union(endPoints).OrderBy(x => x.Z).ToList();

            if ((sorted.Last() - sorted[0]).DotProduct(dir) < 0)
                sorted.Reverse();

            return (sorted[0], sorted.Last());
        }

        /***************************************************/

        private static List<(double, double)> SortedDomains(this List<Line> collinearLines, Point min, double tolerance)
        {
            List<Point> startPoints = collinearLines.Select(x => x.Start).ToList();
            List<Point> endPoints = collinearLines.Select(x => x.End).ToList();

            List<(double, double)> ranges = new List<(double, double)>();
            for (int i = 0; i < collinearLines.Count; i++)
            {
                double start = startPoints[i].Distance(min);
                double end = endPoints[i].Distance(min);
                (double, double) range = start > end ? (end, start) : (start, end);
                ranges.Add(range);
            }

            ranges.Sort(delegate ((double, double) r1, (double, double) r2)
            {
                return r1.Item1.CompareTo(r2.Item1);
            });

            return ranges;
        }

        /***************************************************/

        private static List<(double, double)> MergeRanges(this List<(double, double)> ranges, double tolerance)
        {
            List<(double, double)> mergedRanges = new List<(double, double)>();
            (double, double) currentRange = ranges[0];
            for (int i = 1; i < ranges.Count; i++)
            {
                if (currentRange.Item2 - ranges[i].Item1 >= -tolerance)
                {
                    if (ranges[i].Item2 > currentRange.Item2)
                        currentRange.Item2 = ranges[i].Item2;
                }
                else
                {
                    mergedRanges.Add(currentRange);
                    currentRange = ranges[i];
                }
            }

            mergedRanges.Add(currentRange);
            return mergedRanges;
        }

        /***************************************************/

        private static List<Line> BooleanUnionCollinear(this List<Line> lines, double tolerance, bool keepIntermediatePoints)
        {
            List<Point> allEnds = lines.Select(x => x.Start).Union(lines.Select(x => x.End)).ToList();
            Line dirLine = allEnds.FitLine(tolerance);
            Vector dir = dirLine.Direction(tolerance);
            (Point, Point) extents = lines.Extents(dir, tolerance);
            Point min = dirLine.ClosestPoint(extents.Item1, true);

            List<(double, double)> ranges = lines.SortedDomains(min, tolerance);
            List<(double, double)> mergedRanges = ranges.MergeRanges(tolerance);

            if (keepIntermediatePoints)
            {
                mergedRanges = mergedRanges.IncludeIntermediatePointsRanges(ranges, tolerance);
                List<Point> nodePool = allEnds.CullDuplicates(tolerance);
                List<Line> result = new List<Line>();
                foreach ((double, double) range in mergedRanges)
                {
                    Point start = (min + dir * range.Item1).ClosestPoint(nodePool);
                    Point end = (min + dir * range.Item2).ClosestPoint(nodePool);
                    result.Add(new Line { Start = start, End = end });
                }

                return result;
            }
            else
                return mergedRanges.Select(x => new Line { Start = min + dir * x.Item1, End = min + dir * x.Item2 }).ToList();
        }

        /***************************************************/

        private static List<(double, double)> IncludeIntermediatePointsRanges(this List<(double, double)> mergedRanges, List<(double, double)> allRanges, double tolerance)
        {
            List<double> steps = allRanges.Select(x => x.Item1).Union(allRanges.Select(x => x.Item2)).ToList();
            steps.Sort();
            int i = 0;
            Dictionary<int, List<double>> extraSteps = new Dictionary<int, List<double>>();
            extraSteps.Add(0, new List<double>());
            foreach (double step in steps)
            {
                while (step - mergedRanges[i].Item2 > tolerance)
                {
                    i++;
                    extraSteps.Add(i, new List<double>());
                }

                if (step - mergedRanges[i].Item1 > tolerance)
                {

                    if (mergedRanges[i].Item2 - step > tolerance && extraSteps[i].All(x => Math.Abs(x - step) > tolerance))
                        extraSteps[i].Add(step);
                }
            }

            List<(double, double)> mergedMerged = new List<(double, double)>();
            foreach (int j in extraSteps.Keys.OrderBy(x => x))
            {
                double start = mergedRanges[j].Item1;
                foreach (double step in extraSteps[j])
                {
                    mergedMerged.Add((start, step));
                    start = step;
                }

                mergedMerged.Add((start, mergedRanges[j].Item2));
            }

            return mergedMerged;
        }

        /***************************************************/
    }
}






