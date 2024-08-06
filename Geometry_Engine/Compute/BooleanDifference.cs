/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /****          public Methods - Lines           ****/
        /***************************************************/

        [Description("Returns the parts of the first line that are not overlapping with the reference line, i.e. removes that parts of the first line that is overlapping with the reference line. If the lines are not colinear or within tolerance distance of each other the full first line is returned.")]
        [Input("line", "The line to remove overlaps from.")]
        [Input("refLine", "The reference line. Any parts of this line that are overlapping with the first line are removed from the first line.")]
        [Input("tolerance", "Tolerance used for checking colinearity and proximity of the two lines.", typeof(Length))]
        [Output("line", "The parts of the first line not overlapping with the reference line.")]
        public static List<Line> BooleanDifference(this Line line, Line refLine, double tolerance = Tolerance.Distance)
        {
            return new List<Line> { line }.BooleanDifference(new List<Line> { refLine }, tolerance);
        }

        /***************************************************/

        [Description("Returns the parts of the first lines that are not overlapping with any of the reference lines, i.e. removes that parts of the first lines that is overlapping with any of the reference lines. If a line and a reference line are not colinear or within distance tolerance of each other, no action will be taken for that particular pair of lines.")]
        [Input("lines", "The lines to remove overlaps from.")]
        [Input("refLines", "The reference lines. Any parts of these lines that are overlapping with the first lines are removed from the first lines.")]
        [Input("tolerance", "Tolerance used for checking colinearity and proximity between two lines.", typeof(Length))]
        [Output("lines", "The parts of the first lines not overlapping with the reference lines.")]
        public static List<Line> BooleanDifference(this List<Line> lines, List<Line> refLines, double tolerance = Tolerance.Distance)
        {
            double sqTol = tolerance * tolerance;
            lines = lines.Where(x => x != null && x.SquareLength() > sqTol).ToList();
            List<Line> refLeft = refLines.Where(x => x != null && x.SquareLength() > sqTol).ToList();

            List<Line> result = new List<Line>();
            foreach (var cluster in lines.ClusterCollinear(tolerance))
            {
                List<Line> toCull = new List<Line>();
                for (int i = refLeft.Count - 1; i >= 0; i--)
                {
                    if (refLeft[i].IsCollinear(cluster[0]))
                    {
                        toCull.Add(refLeft[i]);
                        refLeft.RemoveAt(i);
                    }
                }

                if (toCull.Count != 0)
                    result.AddRange(cluster.BooleanDifferenceCollinear(toCull, tolerance));
                else
                    result.AddRange(cluster);
            }

            return result;
        }


        /***************************************************/
        /****         public Methods - Regions          ****/
        /***************************************************/

        [Description("Returns the areas of the closed Polyline region that are not overlapping with the closed Polyline reference regions, i.e. removes that areas of the first region that is overlapping with any of the reference regions. Requires the region and all reference regions to be closed. The method only considers reference regions that are co-planar with the region being evaluated.")]
        [Input("region", "The line to remove overlaps from. Should be a closed planar curve.")]
        [Input("refRegions", "The reference regions. Any parts of these regions that are overlapping with the first region are removed from the first region. All regions required to be closed. Reference regions not coplanar with the region are not considered.")]
        [Input("tolerance", "Tolerance used for checking co-planarity and proximity of the regions.", typeof(Length))]
        [Output("regions", "The region parts of the first region not overlapping with any of the reference regions.")]
        public static List<Polyline> BooleanDifference(this Polyline region, List<Polyline> refRegions, double tolerance = Tolerance.Distance)
        {
            if (!region.IsClosed(tolerance) || refRegions.Any(x => !x.IsClosed()))
            {
                Base.Compute.RecordError("Boolean Difference works on closed regions.");
                return new List<Polyline> { region };
            }

            List<Polyline> allRegions = new List<Polyline> { region };
            Plane p = region.FitPlane();
            if (p == null)
                return new List<Polyline>();

            foreach (Polyline refRegion in refRegions)
            {
                Plane refPlane = refRegion.FitPlane();
                if (refPlane != null && p.IsCoplanar(refPlane))
                    allRegions.Add(refRegion);
            }

            if (allRegions.Count == 1)
                return new List<Polyline> { region };

            Vector normal = region.Normal();

            for (int i = 0; i < allRegions.Count; i++)
                if (!allRegions[i].IsClockwise(normal))
                    allRegions[i] = allRegions[i].Flip();

            double sqTolerance = tolerance * tolerance;
            List<Polyline> tmpResult = new List<Polyline>();
            List<Point>[] iPts = new List<Point>[allRegions.Count];

            for (int k = 0; k < iPts.Count(); k++)
            {
                iPts[k] = new List<Point>();
            }

            List<BoundingBox> regionBounds = allRegions.Select(x => x.Bounds()).ToList();
            for (int i = 0; i < allRegions.Count - 1; i++)
            {
                if (regionBounds[0].IsInRange(regionBounds[i]))
                {
                    for (int j = i + 1; j < allRegions.Count; j++)
                    {
                        if (regionBounds[0].IsInRange(regionBounds[j]) && regionBounds[i].IsInRange(regionBounds[j]))
                        {
                            List<Point> tmpPts = allRegions[i].LineIntersections(allRegions[j]);
                            iPts[i].AddRange(tmpPts);
                            iPts[j].AddRange(tmpPts);
                        }
                    }
                }
            }

            for (int j = 0; j < allRegions.Count; j++)
            {
                List<Polyline> splReg = new List<Polyline>();
                if (iPts[j].Count >= 1)
                    splReg = allRegions[j].SplitAtPoints(iPts[j]);
                else
                    splReg.Add(allRegions[j]);

                foreach (Polyline segment in splReg)
                {
                    bool flag = false;
                    List<Point> mPts = new List<Point> { segment.PointAtParameter(0.5) };

                    if (regionBounds[0].IsContaining(mPts) && region.IsContaining(mPts, true, tolerance))
                        flag = true;

                    for (int k = 1; k < allRegions.Count; k++)
                    {
                        if (regionBounds[k].IsContaining(mPts, true, tolerance) && allRegions[k].IsContaining(mPts, false, tolerance))
                        {
                            flag = false;
                            break;
                        }
                    }

                    if (flag)
                        tmpResult.Add(segment);
                }
            }

            for (int i = 0; i < tmpResult.Count; i++)
            {
                for (int j = 0; j < tmpResult.Count; j++)
                {
                    if (i != j && tmpResult[i].IsSimilarSegment(tmpResult[j], tolerance))
                    {
                        for (int k = i > j ? i + 1 : j + 1; k < tmpResult.Count(); k++)
                            if (tmpResult[k].IsSimilarSegment(tmpResult[j], tolerance))
                                tmpResult.RemoveAt(k);

                        if (!tmpResult[i].PointAtParameter(0.5).IsOnCurve(region, tolerance) &&
                            !tmpResult[j].PointAtParameter(0.5).IsOnCurve(region, tolerance))
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
                        else
                        {
                            if (tmpResult[i].TangentAtParameter(0.5).IsEqual(tmpResult[j].TangentAtParameter(0.5)))
                            {
                                tmpResult.RemoveAt(Math.Max(j, i));
                                tmpResult.RemoveAt(Math.Min(j, i));
                                if (i > 0)
                                    i--;
                                else
                                    j = 0;
                            }
                            else
                            {
                                tmpResult.RemoveAt(Math.Min(j, i));
                                if (i > j)
                                    i--;
                                else
                                    j = 0;
                            }
                        }
                    }
                }
            }

            List<Polyline> result = Join(tmpResult, tolerance).Select(x => x.Close(tolerance)).ToList();

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

        [Description("Returns the areas of the closed ICurve region that are not overlapping with the closed ICurve reference regions, i.e. removes that areas of the first region that is overlapping with any of the reference regions. Requires the region and all reference regions to be closed. The method only considers reference regions that are co-planar with the region being evaluated.")]
        [Input("region", "The line to remove overlaps from. Should be a closed planar curve.")]
        [Input("refRegions", "The reference regions. Any parts of these regions that are overlapping with the first region are removed from the first region. All regions required to be closed. Reference regions not coplanar with the region are not considered.")]
        [Input("tolerance", "Tolerance used for checking co-planarity and proximity of the regions.", typeof(Length))]
        [Output("regions", "The region parts of the first region not overlapping with any of the reference regions.")]
        public static List<PolyCurve> BooleanDifference(this ICurve region, IEnumerable<ICurve> refRegions, double tolerance = Tolerance.Distance)
        {
            List<ICurve> refRegionsList = refRegions.ToList();

            if (region is NurbsCurve || region is Ellipse || refRegionsList.Any(x => x is NurbsCurve || x is Ellipse))
            {
                Base.Compute.RecordError("NurbsCurves and ellipses are not implemented for BooleanDifference.");
                return null;
            }

            if (!region.IIsClosed(tolerance) || refRegionsList.Any(x => !x.IIsClosed()))
            {
                Base.Compute.RecordError("Boolean Difference works on closed regions.");
                if (region is PolyCurve)
                    return new List<PolyCurve> { region as PolyCurve };
                else
                    return new List<PolyCurve> { new PolyCurve { Curves = region.ISubParts().ToList() } };
            }        
            
            List<ICurve> allRegions = new List<ICurve> { region };
            Plane p = region.IFitPlane();
            if (p == null)
                return new List<PolyCurve>();

            foreach (ICurve refRegion in refRegionsList)
            {
                Plane refPlane = refRegion.IFitPlane();
                if (refPlane != null && p.IsCoplanar(refPlane))
                    allRegions.Add(refRegion);
            }

            if (allRegions.Count == 1)
                if (region is PolyCurve)
                    return new List<PolyCurve> { region as PolyCurve };
                else
                    return new List<PolyCurve> { new PolyCurve { Curves = region.ISubParts().ToList() } };

            Vector normal = region.INormal();

            for (int i = 0; i < allRegions.Count; i++)
                if (!allRegions[i].IIsClockwise(normal))
                    allRegions[i] = allRegions[i].IFlip();

            double sqTolerance = tolerance * tolerance;
            List<ICurve> tmpResult = new List<ICurve>();
            List<Point>[] iPts = new List<Point>[allRegions.Count];

            for (int k = 0; k < iPts.Count(); k++)
            {
                iPts[k] = new List<Point>();
            }

            List<BoundingBox> regionBounds = allRegions.Select(x => x.IBounds()).ToList();
            for (int i = 0; i < allRegions.Count - 1; i++)
            {
                if (regionBounds[0].IsInRange(regionBounds[i]))
                {
                    for (int j = i + 1; j < allRegions.Count; j++)
                    {
                        if (regionBounds[0].IsInRange(regionBounds[j]) && regionBounds[i].IsInRange(regionBounds[j]))
                        {
                            List<Point> tmpPts = allRegions[i].ICurveIntersections(allRegions[j]);
                            iPts[i].AddRange(tmpPts);
                            iPts[j].AddRange(tmpPts);
                        }
                    }
                }
            }

            for (int j = 0; j < allRegions.Count; j++)
            {
                List<ICurve> splReg = new List<ICurve>();
                if (iPts[j].Count >= 1)
                    splReg = allRegions[j].ISplitAtPoints(iPts[j]);
                else
                    splReg.Add(allRegions[j]);                

                foreach (ICurve segment in splReg)
                    {
                        bool flag = false;
                        List<Point> mPts = new List<Point> { segment.IPointAtParameter(0.5) };

                        if (regionBounds[0].IsContaining(mPts) && region.IIsContaining(mPts, true, tolerance))
                            flag = true;

                        for (int k = 1; k < allRegions.Count; k++)
                        {
                            if (regionBounds[k].IsContaining(mPts, true, tolerance) && allRegions[k].IIsContaining(mPts, false, tolerance))
                            {
                                flag = false;
                                break;
                            }
                        }

                        if (flag)
                            tmpResult.Add(segment);
                    }
             }

            for (int i = 0; i < tmpResult.Count; i++)
            {
                for (int j = 0; j < tmpResult.Count; j++)
                {
                    if (i != j && tmpResult[i].IsSimilarSegment(tmpResult[j], tolerance))
                    {
                        for (int k = i > j ? i + 1 : j + 1; k < tmpResult.Count(); k++)
                            if (tmpResult[k].IsSimilarSegment(tmpResult[j], tolerance))
                                tmpResult.RemoveAt(k);
                        
                        if (!tmpResult[i].IPointAtParameter(0.5).IIsOnCurve(region,tolerance) &&
                            !tmpResult[j].IPointAtParameter(0.5).IIsOnCurve(region, tolerance))
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
                        else
                        {
                            if (tmpResult[i].ITangentAtParameter(0.5).IsEqual(tmpResult[j].ITangentAtParameter(0.5)))
                            {
                                tmpResult.RemoveAt(Math.Max(j, i));
                                tmpResult.RemoveAt(Math.Min(j, i));
                                if (i > 0)
                                    i--;
                                else
                                    j = 0;
                            }
                            else
                            {
                                tmpResult.RemoveAt(Math.Min(j, i));
                                if (i > j)
                                    i--;
                                else
                                    j = 0;
                            }
                        }
                    }
                }
            }

            List<PolyCurve> result = IJoin(tmpResult, tolerance).Select(x => x.Close(tolerance)).ToList();

            int res = 0;
            while (res < result.Count)
            {
                if (result[res].Area(tolerance) <= sqTolerance)
                    result.RemoveAt(res);
                else
                    res++;
            }

            return result;
        }


        /***************************************************/
        /****              Private Methods              ****/
        /***************************************************/

        private static List<Line> BooleanDifferenceCollinear(this List<Line> lines1, List<Line> lines2, double tolerance)
        {
            Line dirLine = lines1.Select(x => x.Start).Union(lines1.Select(x => x.End)).Union(lines2.Select(x => x.Start)).Union(lines2.Select(x => x.End)).FitLine(tolerance);
            Vector dir = dirLine.Direction(tolerance);
            (Point, Point) extents1 = lines1.Extents(dir, tolerance);
            (Point, Point) extents2 = lines2.Extents(dir, tolerance);
            Point min = (extents1.Item1 - extents2.Item1).DotProduct(dir) < 0 ? extents1.Item1 : extents2.Item1;
            min = dirLine.ClosestPoint(min, true);

            List<(double, double)> ranges1 = lines1.SortedDomains(min, tolerance);
            List<(double, double)> ranges2 = lines2.SortedDomains(min, tolerance);

            List<(double, double)> mergedRanges1 = ranges1.MergeRanges(tolerance);
            List<(double, double)> mergedRanges2 = ranges2.MergeRanges(tolerance);

            List<(double, double)> differenceRanges = new List<(double, double)>();
            int i = 0;
            foreach ((double, double) range in mergedRanges2)
            {
                double exclusionStart = range.Item1;
                for (; i < mergedRanges1.Count; i++)
                {
                    if (exclusionStart - mergedRanges1[i].Item2 < -tolerance)
                        break;

                    if (mergedRanges1[i].Item2 - mergedRanges1[i].Item1 > tolerance)
                        differenceRanges.Add(mergedRanges1[i]);
                }

                for (; i < mergedRanges1.Count; i++)
                {
                    if (mergedRanges1[i].Item1 - range.Item1 < -tolerance)
                        differenceRanges.Add((mergedRanges1[i].Item1, Math.Min(mergedRanges1[i].Item2, range.Item1)));

                    if (range.Item2 - mergedRanges1[i].Item2 < tolerance)
                    {
                        mergedRanges1[i] = (Math.Max(mergedRanges1[i].Item1, range.Item2), mergedRanges1[i].Item2);
                        break;
                    }
                }
            }

            for (; i < mergedRanges1.Count; i++)
            {
                if (mergedRanges1[i].Item2 - mergedRanges1[i].Item1 > tolerance)
                    differenceRanges.Add(mergedRanges1[i]);
            }

            return differenceRanges.Select(x => new Line { Start = min + dir * x.Item1, End = min + dir * x.Item2 }).ToList();
        }

        /***************************************************/
    }
}





