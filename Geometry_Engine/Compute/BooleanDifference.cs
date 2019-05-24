/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
    public static partial class Compute
    {
        /***************************************************/
        /****          public Methods - Lines           ****/
        /***************************************************/
        
        public static List<Line> BooleanDifference(this Line line, Line refLine, double tolerance = Tolerance.Distance)
        {
            if (refLine.Length() <= tolerance)
                return new List<Line> { line.Clone() };

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
                        splitLine = new List<Line> { line.Clone() };
                    else
                        splitLine = new List<Line>();
                }

                return splitLine;
            }
            return new List<Line> { line.Clone() };
        }

        /***************************************************/

        public static List<Line> BooleanDifference(this List<Line> lines, List<Line> refLines, double tolerance = Tolerance.Distance)
        {
            List<Line> result = new List<Line>();
            
            foreach (Line line in lines)
            {
                List<Line> splitLine = new List<Line> { line.Clone() };
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

        [DeprecatedAttribute("2.3", "Replaced with a method returning a list of lists", null, "BooleanDifference")]
        public static List<Polyline> BooleanDifference(this List<Polyline> regions, List<Polyline> refRegions, double tolerance = Tolerance.Distance)
        {
            List<Polyline> result = new List<Polyline>();
            List<Polyline> openings = new List<Polyline>();

            bool isOpening;
            List<Polyline> bDifference;
            foreach (Polyline region in regions)
            {
                List<Polyline> splitRegion = new List<Polyline> { region.Clone() };
                foreach (Polyline refRegion in refRegions)
                {
                    List<Polyline> split = new List<Polyline>();
                    foreach (Polyline sr in splitRegion)
                    {
                        isOpening = false;
                        bDifference = sr.BooleanDifference(refRegion, out isOpening, tolerance);
                        
                        if (isOpening)
                        {
                            split.Add(bDifference[0]);
                            openings.AddRange(bDifference.Skip(1));
                        }
                        else
                            split.AddRange(bDifference);
                    }
                    splitRegion = split;
                }

                result.AddRange(splitRegion);
            }

            result.AddRange(openings);
            return result;
        }

        /***************************************************/

        public static List<Polyline> BooleanDifference(this Polyline region, List<Polyline> refRegions, double tolerance = Tolerance.Distance)
        {
            if (!region.IsClosed(tolerance) || refRegions.Any(x => !x.IsClosed()))
            {
                Reflection.Compute.RecordError("Boolean Difference works on closed regions.");
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

            if (iPts[0].Count == 0)
                return new List<Polyline> { region };

            for (int j = 0; j < allRegions.Count; j++)
            {
                if (iPts[j].Count > 0)
                {
                    List<Polyline> splReg = allRegions[j].SplitAtPoints(iPts[j]);
                    foreach (Polyline segment in splReg)
                    {
                        bool flag = false;
                        List<Point> mPts = new List<Point> { segment.PointAtParameter(0.5) };

                        if (regionBounds[0].IsContaining(mPts) && region.IsContaining(mPts, true, tolerance))
                            flag = true;

                        for (int k = 1; k < allRegions.Count; k++)
                        {
                            if (regionBounds[k].IsContaining(mPts, false, tolerance) && allRegions[k].IsContaining(mPts, false, tolerance))
                            {
                                flag = false;
                                break;
                            }
                        }

                        if (flag)
                            tmpResult.Add(segment);
                    }
                }
            }

            for (int i = 0; i < tmpResult.Count - 1; i++)
            {
                for (int j = i + 1; j < tmpResult.Count; j++)
                {
                    if (tmpResult[i].IsEqual(tmpResult[j]) || tmpResult[i].IsEqual(tmpResult[j].Flip()))
                        tmpResult.RemoveAt(j);
                }
            }

            List<Polyline> result = BH.Engine.Geometry.Compute.Join(tmpResult, tolerance);

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

        public static List<PolyCurve> BooleanDifference(this PolyCurve region, List<PolyCurve> refRegions, double tolerance = Tolerance.Distance)
        {
            if (!region.IsClosed(tolerance) || refRegions.Any(x => !x.IsClosed()))
            {
                Reflection.Compute.RecordError("Boolean Difference works on closed regions.");
                return new List<PolyCurve> { region };
            }

            List<PolyCurve> allRegions = new List<PolyCurve> { region };
            Plane p = region.FitPlane();
            foreach (PolyCurve refRegion in refRegions)
            {
                if (p.IsCoplanar(refRegion.FitPlane()))
                    allRegions.Add(refRegion);
            }

            if (allRegions.Count == 1)
                return new List<PolyCurve> { region };

            double sqTolerance = tolerance * tolerance;
            List<PolyCurve> tmpResult = new List<PolyCurve>();
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
                            List<Point> tmpPts = allRegions[i].CurveIntersections(allRegions[j]);
                            iPts[i].AddRange(tmpPts);
                            iPts[j].AddRange(tmpPts);
                        }
                    }
                }
            }

            if (iPts[0].Count == 0)
                return new List<PolyCurve> { region };

            for (int j = 0; j < allRegions.Count; j++)
            {
                if (iPts[j].Count > 0)
                {
                    List<PolyCurve> splReg = allRegions[j].SplitAtPoints(iPts[j]);
                    foreach (PolyCurve segment in splReg)
                    {
                        bool flag = false;
                        List<Point> mPts = new List<Point> { segment.PointAtParameter(0.5) };

                        if (regionBounds[0].IsContaining(mPts) && region.IsContaining(mPts, true, tolerance))
                            flag = true;

                        for (int k = 1; k < allRegions.Count; k++)
                        {
                            if (regionBounds[k].IsContaining(mPts, false, tolerance) && allRegions[k].IsContaining(mPts, false, tolerance))
                            {
                                flag = false;
                                break;
                            }
                        }

                        if (flag)
                            tmpResult.Add(segment);
                    }
                }
            }

            for (int i = 0; i < tmpResult.Count - 1; i++)
            {
                for (int j = i + 1; j < tmpResult.Count; j++)
                {
                    if (tmpResult[i].IsEqual(tmpResult[j]) || tmpResult[i].IsEqual(tmpResult[j].Flip()))
                        tmpResult.RemoveAt(j);
                }
            }

            List<PolyCurve> result = BH.Engine.Geometry.Compute.Join(tmpResult, tolerance);

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
        /****              Private methods              ****/
        /***************************************************/

        [DeprecatedAttribute("2.3", "Useless as its parent method has been replaced with another one", null, "")]
        private static List<Polyline> BooleanDifference(this Polyline region, Polyline refRegion, out bool isOpening, double tolerance = Tolerance.Distance)
        {
            List<Polyline> cutRegions = region.BooleanIntersection(refRegion, tolerance);
            List<Polyline> result = new List<Polyline>();

            isOpening = false;
            if (region.IsCoplanar(refRegion, tolerance))
            {
                List<Point> iPts = new List<Point>();
                cutRegions.ForEach(cr => iPts.AddRange(region.LineIntersections(cr, tolerance)));
                List<Polyline> splitRegion = region.SplitAtPoints(iPts, tolerance);

                if (splitRegion.Count == 1)
                {
                    result.Add(region.Clone());
                    foreach (Point cPt in refRegion.ControlPoints)
                    {
                        if (!region.IsContaining(new List<Point> { cPt }, true, tolerance))
                            return result;
                    }

                    result.Add(refRegion.Clone());
                    isOpening = true;
                    return result;
                }

                double sqTol = tolerance * tolerance;
                foreach (Polyline segment in splitRegion)
                {
                    List<Point> cPts = segment.SubParts().Select(s => s.ControlPoints().Average()).ToList();
                    cPts.AddRange(segment.ControlPoints);

                    bool isInRegion = false;
                    foreach (Polyline cr in cutRegions)
                    {
                        if (cr.IsContaining(cPts, true, tolerance))
                        {
                            isInRegion = true;
                            break;
                        }
                    }

                    if (!isInRegion)
                        result.Add(segment);
                }

                foreach (Polyline cr in cutRegions)
                {
                    splitRegion = cr.SplitAtPoints(iPts, tolerance);
                    foreach (Polyline segment in splitRegion)
                    {
                        List<Point> cPts = segment.SubParts().Select(s => s.ControlPoints().Average()).ToList();
                        cPts.AddRange(segment.ControlPoints);

                        if (region.IsContaining(cPts, true, tolerance))
                        {
                            foreach (Point pt in cPts)
                            {
                                if (region.ClosestPoint(pt).SquareDistance(pt) > sqTol)
                                {
                                    result.Add(segment);
                                    break;
                                }
                            }
                        }
                    }
                }

                return BH.Engine.Geometry.Compute.Join(result, tolerance);
            }

            return new List<Polyline> { region.Clone() };
        }

        /***************************************************/
    }
}
