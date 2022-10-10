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
    public static partial class Compute
    {
        /***************************************************/
        /****          public Methods - Lines           ****/
        /***************************************************/

        public static List<Line> BooleanDifference(this Line line, Line refLine, double tolerance = Tolerance.Distance)
        {
            if (refLine.Length() <= tolerance)
                return new List<Line> { line.DeepClone() };

            if (line.IsCollinear(refLine, tolerance))
            {
                List<Line> splitLine = line.SplitAtPoints(refLine.ControlPoints());

                if (splitLine.Count == 3)
                    splitLine.RemoveAt(1);
                else if (splitLine.Count == 2)
                {
                    Point aPt = refLine.ControlPoints().Average();

                    if (line.Start.SquareDistance(aPt) < line.End.SquareDistance(aPt))
                        splitLine.RemoveAt(0);
                    else
                        splitLine.RemoveAt(1);
                }
                else
                {
                    double sqTol = tolerance * tolerance;
                    Point aPt = splitLine[0].ControlPoints().Average();
                    Point aRPt = refLine.ControlPoints().Average();

                    if (aRPt.SquareDistance(splitLine[0]) > sqTol && aPt.SquareDistance(refLine) > sqTol)
                        splitLine = new List<Line> { line.DeepClone() };
                    else
                        splitLine = new List<Line>();
                }

                return splitLine;
            }
            return new List<Line> { line.DeepClone() };
        }

        /***************************************************/

        public static List<Line> BooleanDifference(this List<Line> lines, List<Line> refLines, double tolerance = Tolerance.Distance)
        {
            List<Line> result = new List<Line>();

            foreach (Line line in lines)
            {
                List<Line> splitLine = new List<Line> { line.DeepClone() };
                int k = 0;
                bool split = false;
                do
                {
                    split = false;
                    for (int i = k; i < refLines.Count; i++)
                    {
                        if (split)
                            break;

                        for (int j = 0; j < splitLine.Count; j++)
                        {
                            Line l = splitLine[j];
                            List<Line> bd = l.BooleanDifference(refLines[i], tolerance);

                            if (bd.Count == 0)
                            {
                                k = refLines.Count;
                                splitLine = new List<Line>();
                                split = true;
                                break;
                            }
                            else if (bd.Count > 1 || Math.Abs(bd[0].Length() - l.Length()) > tolerance)
                            {
                                k = i + 1;
                                split = true;
                                splitLine.RemoveAt(j);
                                splitLine.AddRange(bd);
                                break;
                            }
                        }
                    }
                } while (split);

                result.AddRange(splitLine);
            }
            return result;
        }


        /***************************************************/
        /****         public Methods - Regions          ****/
        /***************************************************/
        
        public static List<Polyline> BooleanDifference(this Polyline region, List<Polyline> refRegions, double tolerance = Tolerance.Distance)
        {
            if (!region.IsClosed(tolerance) || refRegions.Any(x => !x.IsClosed()))
            {
                Base.Compute.RecordError("Boolean Difference works on closed regions.");
                return new List<Polyline> { region };
            }

            List<Polyline> allRegions = new List<Polyline> { region };
            Plane p = region.FitPlane();
            foreach (Polyline refRegion in refRegions)
            {
                if (p.IsCoplanar(refRegion.FitPlane()))
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
            foreach (ICurve refRegion in refRegionsList)
            {
                if (p.IsCoplanar(refRegion.IFitPlane()))
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
    }
}



