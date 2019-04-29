/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

        public static List<Line> BooleanUnion(this Line line, Line refLine, double tolerance = Tolerance.Distance)
        {
            if (line.IsCollinear(refLine, tolerance))
            {
                double sqTol = tolerance * tolerance;

                List<Point> cPts = line.ControlPoints();
                foreach (Point cPt in cPts)
                {
                    if (cPt.SquareDistance(refLine) <= sqTol)
                    {
                        cPts.AddRange(refLine.ControlPoints());
                        cPts = cPts.SortCollinear(tolerance);
                        return new List<Line> { new Line { Start = cPts[0], End = cPts.Last() } };
                    }
                }

                cPts = refLine.ControlPoints();
                foreach (Point cPt in cPts)
                {
                    if (cPt.SquareDistance(line) <= sqTol)
                    {
                        cPts.AddRange(line.ControlPoints());
                        cPts = cPts.SortCollinear(tolerance);
                        return new List<Line> { new Line { Start = cPts[0], End = cPts.Last() } };
                    }
                }
            }

            return new List<Line> { line.Clone(), refLine.Clone() };
        }

        /***************************************************/

        public static List<Line> BooleanUnion(this List<Line> lines, double tolerance = Tolerance.Distance)
        {
            List<Line> result = lines.Select(l => l.Clone()).ToList();
            bool union;
            do
            {
                union = false;
                for (int i = 0; i < result.Count - 1; i++)
                {
                    if (union)
                        break;

                    for (int j = i + 1; j < result.Count; j++)
                    {
                        List<Line> uLines = result[i].BooleanUnion(result[j], tolerance);
                        if (uLines.Count == 1)
                        {
                            result.RemoveAt(j);
                            result.RemoveAt(i);
                            result.Insert(i, uLines[0]);
                            union = true;
                            break;
                        }
                    }
                }
            } while (union);

            return result;
        }


        /***************************************************/
        /****         public Methods - Regions          ****/
        /***************************************************/

        public static List<Polyline> NewBooleanUnion(this List<Polyline> regions, double tolerance = Tolerance.Distance)
        {
            foreach (Polyline region in regions)
            {
                if (!region.IsClosed(tolerance))
                {
                    Reflection.Compute.RecordError("Boolean Union works only on closed curves.");
                    return regions;
                }
            }

            List<Polyline> result = new List<Polyline>();
            double sqTolerance = tolerance * tolerance;
            
            foreach (List<Polyline> regionCluster in regions.ClusterCoplanar(tolerance))
            {
                List<Polyline> tmpResult = new List<Polyline>();
                
                List<BoundingBox> regionBounds = regionCluster.Select(x => x.Bounds()).ToList();
                List<List<Line>> curveSegments = regionCluster.Select(x => x.SubParts()).ToList();

                List<Point>[] iPts = new List<Point>[regionCluster.Count];
                for (int i = 0; i < iPts.Length; i++)
                {
                    iPts[i] = new List<Point>();
                }
                
                for (int j = 0; j < regionCluster.Count - 1; j++)
                {
                    for (int k = j + 1; k < regionCluster.Count; k++)
                    {
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
                    if (iPts[i].Count > 0)
                    {
                        List<Polyline> splReg = regionCluster[i].SplitAtPoints(iPts[i]);
                        for (int k = 0; k < splReg.Count; k++)
                        {
                            Boolean flag = true;
                            List<Point> mPts = new List<Point> { splReg[k].PointAtParameter(0.5) };

                            for (int j = 0; j < regionCluster.Count; j++)
                            {

                                if (i != j && regionBounds[j].IsContaining(mPts) && regionCluster[j].IsContaining(mPts, true, tolerance))
                                    {
                                        flag = false;
                                        break;
                                    }
                            }
                            if (flag)
                                tmpResult.Add(splReg[k]);
                        }
                    }
                    else
                        result.Add(regionCluster[i]);
                }
                
                for (int i = 0; i < tmpResult.Count - 1; i++)
                {
                    for (int j = i + 1; j < tmpResult.Count; j++)
                    {
                        if (tmpResult[i].IsEqual(tmpResult[j]) || tmpResult[i].IsEqual(tmpResult[j].Flip()))
                            tmpResult.RemoveAt(j);
                    }
                }

                result.AddRange(tmpResult.Join(tolerance));
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

        public static List<Polyline> BooleanUnion(this List<Polyline> regions, double tolerance = Tolerance.Distance)
        {
            double sqTol = tolerance * tolerance;
            List<Polyline> cutRegions = regions.Where(r => r.Area() > sqTol).Select(r => r.Clone()).ToList();
            List<Polyline> result = new List<Polyline>();
            List<Polyline> openings = new List<Polyline>();
            bool union;

            while (cutRegions.Count > 0)
            {
                Polyline uRegion = cutRegions[0].Clone();
                cutRegions.RemoveAt(0);
                do
                {
                    List<Polyline> uRegions;
                    union = false;
                    for (int i = 0; i < cutRegions.Count; i++)
                    {
                        union = cutRegions[i].BooleanUnion(uRegion, out uRegions, tolerance);
                        if (union)
                        {
                            cutRegions.RemoveAt(i);

                            uRegions.Sort(delegate (Polyline p1, Polyline p2)
                            {
                                return p1.Area().CompareTo(p2.Area());
                            });
                            uRegions.Reverse();

                            uRegion = uRegions[0];
                            openings.AddRange(uRegions.Skip(1));
                            break;
                        }
                    }
                } while (union);

                result.Add(uRegion);
            }

            result.AddRange(openings.BooleanDifference(regions, tolerance));
            return result;
        }

        /***************************************************/

        public static List<PolyCurve> BooleanUnion(this List<PolyCurve> regions, double tolerance = Tolerance.Distance)
        {
            Boolean coplanar = true;
            for (int i = 0; i < regions.Count - 1; i++)
            {
                if (!regions[i].IsCoplanar(regions[i + 1], tolerance))
                    coplanar = false;
            }

            if (coplanar)
            {
                double sqTol = tolerance * tolerance;
                List<PolyCurve> tmpResult = new List<PolyCurve>();
                List<PolyCurve> result = new List<PolyCurve>();
                List<Point>[] iPts = new List<Point>[regions.Count];
                List<Point> tmpPts = new List<Point>();

                for (int j = 0; j < regions.Count - 1; j++)
                {
                    for (int k = j + 1; k < regions.Count; k++)
                    {
                        if (regions[j].Bounds().IsInRange(regions[k].Bounds()))
                            tmpPts.AddRange(regions[j].CurveIntersections(regions[k]));
                        if (tmpPts.Count > 0)
                        {
                            iPts[j] = tmpPts;
                            iPts[k] = tmpPts;
                        }
                        else
                        {
                            iPts[j] = new List<Point>();
                            iPts[k] = new List<Point>();
                        }
                    }
                }


                for (int i = 0; i < regions.Count; i++)
                {
                    if (iPts[i].Count() > 0)
                    {
                        List<PolyCurve> splReg = regions[i].SplitAtPoints(iPts[i]);
                        for (int k = 0; k < splReg.Count; k++)
                        {
                            Boolean flag = true;
                            List<Point> mPts = new List<Point> { splReg[k].PointAtParameter(0.5) };

                            for (int j = 0; j < regions.Count; j++)
                            {

                                if (regions[j].Bounds().IsContaining(mPts))
                                    if (i != j && regions[j].IsContaining(mPts, false, tolerance))
                                    {
                                        flag = false;
                                        break;
                                    }
                            }
                            if (flag)
                            {
                                tmpResult.Add(splReg[k]);
                                k++;
                            }
                        }
                    }
                    else
                        tmpResult.Add(regions[i]);
                }

                for (int i = 0; i < tmpResult.Count - 1; i++)
                {
                    for (int j = i + 1; j < tmpResult.Count; j++)
                    {
                        if (tmpResult[i].IIsEqual(tmpResult[j]) || tmpResult[i].IIsEqual(tmpResult[j].IFlip()))
                            tmpResult.RemoveAt(j);
                    }
                }

                result = tmpResult.Join(tolerance);
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
            return new List<PolyCurve>();
        }

    
        /***************************************************/
        /****              Private methods              ****/
        /***************************************************/

        private static bool BooleanUnion(this Polyline region1, Polyline region2, out List<Polyline> result, double tolerance = Tolerance.Distance)
        {
            result = new List<Polyline>();

            if (region1.IsCoplanar(region2, tolerance))
            {
                Plane p1 = region1.FitPlane(tolerance);

                if (!region1.IsClockwise(p1.Normal))
                    region1 = region1.Clone().Flip();
                if (!region2.IsClockwise(p1.Normal))
                    region2 = region2.Clone().Flip();

                List<Point> cPts1 = region1.SubParts().Select(s => s.ControlPoints().Average()).ToList();
                cPts1.AddRange(region1.ControlPoints);
                List<Point> cPts2 = region2.SubParts().Select(s => s.ControlPoints().Average()).ToList();
                cPts2.AddRange(region2.ControlPoints);

                if (region1.IsContaining(cPts2, true, tolerance))
                    result.Add(region1.Clone());
                else if (region2.IsContaining(cPts1, true, tolerance))
                    result.Add(region2.Clone());
                else
                {
                    double sqTol = tolerance * tolerance;
                    List<Point> iPts = region1.LineIntersections(region2, tolerance);
                    List<Polyline> splitRegion1 = region1.SplitAtPoints(iPts, tolerance);
                    List<Polyline> splitRegion2 = region2.SplitAtPoints(iPts, tolerance);

                    if (splitRegion1.Count == 1 && splitRegion2.Count == 1)
                    {
                        result = new List<Polyline> { region1.Clone(), region2.Clone() };
                        return false;
                    }

                    foreach (Polyline segment in splitRegion1)
                    {
                        List<Line> subparts = segment.SubParts();
                        List<Point> cPts = subparts.Select(s => s.ControlPoints().Average()).ToList();
                        cPts.AddRange(segment.ControlPoints);

                        if (!region2.IsContaining(cPts, true, tolerance))
                            result.Add(segment);
                        else if (!region2.IsContaining(cPts, false, tolerance))
                        {
                            foreach (Polyline r in splitRegion2)
                            {
                                bool found = false;

                                if (segment.ControlPoints.Count == r.ControlPoints.Count && segment.ControlPoints[0].SquareDistance(r.ControlPoints[0]) <= sqTol)
                                {
                                    found = true;
                                    for (int i = 0; i < segment.ControlPoints.Count; i++)
                                    {
                                        if (segment.ControlPoints[i].SquareDistance(r.ControlPoints[i]) > sqTol)
                                        {
                                            found = false;
                                            break;
                                        }
                                    }
                                }

                                if (found)
                                {
                                    result.Add(segment);
                                    break;
                                }
                            }
                        }
                    }

                    foreach (Polyline segment in splitRegion2)
                    {
                        List<Point> cPts = segment.SubParts().Select(s => s.ControlPoints().Average()).ToList();
                        cPts.AddRange(segment.ControlPoints);

                        if (!region1.IsContaining(cPts, true, tolerance))
                            result.Add(segment);
                    }

                    result = result.Join(tolerance);
                }
                return true;
            }

            result = new List<Polyline> { region1.Clone(), region2.Clone() };
            return false;
        }
        
        /***************************************************/
    }
}