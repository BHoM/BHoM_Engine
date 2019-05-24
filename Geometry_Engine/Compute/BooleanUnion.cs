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

        public static List<Polyline> BooleanUnion(this List<Polyline> regions, double tolerance = Tolerance.Distance)
        {
            if (regions.Count < 2)
                return regions;

            if (regions.Any(x => !x.IsClosed(tolerance)))
            {
                Reflection.Compute.RecordError("Boolean Union works on closed regions.");
                return regions;
            }

            List<Polyline> result = new List<Polyline>();
            double sqTolerance = tolerance * tolerance;

            foreach (List<Polyline> regionCluster in regions.ClusterCoplanar(tolerance))
            {
                List<Polyline> tmpResult = new List<Polyline>();

                List<Point>[] iPts = new List<Point>[regionCluster.Count];
                for (int i = 0; i < iPts.Length; i++)
                {
                    iPts[i] = new List<Point>();
                }
                
                List<BoundingBox> regionBounds = regionCluster.Select(x => x.Bounds()).ToList();
                for (int j = 0; j < regionCluster.Count - 1; j++)
                {
                    for (int k = j + 1; k < regionCluster.Count; k++)
                    {
                        if (regionBounds[j].IsInRange(regionBounds[k]))
                        {
                            List<Point> tmpPts = regionCluster[j].LineIntersections(regionCluster[k], tolerance);
                            iPts[j].AddRange(tmpPts);
                            iPts[k].AddRange(tmpPts);
                        }
                    }
                }

                for (int i = 0; i < regionCluster.Count; i++)
                {
                    if (iPts[i].Count > 0)
                    {
                        List<Polyline> splReg = regionCluster[i].SplitAtPoints(iPts[i]);
                        foreach(Polyline segment in splReg)
                        {
                            bool flag = true;
                            List<Point> mPts = new List<Point> { segment.PointAtParameter(0.5) };

                            for (int j = 0; j < regionCluster.Count; j++)
                            {

                                if (i != j && regionBounds[j].IsContaining(mPts) && regionCluster[j].IsContaining(mPts, true, tolerance))
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

                for (int i = 0; i < tmpResult.Count - 1; i++)
                {
                    for (int j = i + 1; j < tmpResult.Count; j++)
                    {
                        if (tmpResult[i].IsEqual(tmpResult[j]) || tmpResult[i].IsEqual(tmpResult[j].Flip()))
                            tmpResult.RemoveAt(j);
                    }
                }

                result.AddRange(BH.Engine.Geometry.Compute.Join(tmpResult, tolerance));
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

        public static List<PolyCurve> BooleanUnion(this List<PolyCurve> regions, double tolerance = Tolerance.Distance)
        {
            if (regions.Count < 2)
                return regions;

            if (regions.Any(x => !x.IsClosed(tolerance)))
            {
                Reflection.Compute.RecordError("Boolean Union works on closed regions.");
                return regions;
            }

            List<PolyCurve> result = new List<PolyCurve>();
            double sqTolerance = tolerance * tolerance;

            foreach (List<PolyCurve> regionCluster in regions.ClusterCoplanar(tolerance))
            {
                List<PolyCurve> tmpResult = new List<PolyCurve>();

                List<Point>[] iPts = new List<Point>[regionCluster.Count];
                for (int i = 0; i < iPts.Length; i++)
                {
                    iPts[i] = new List<Point>();
                }

                List<BoundingBox> regionBounds = regionCluster.Select(x => x.Bounds()).ToList();
                for (int j = 0; j < regionCluster.Count - 1; j++)
                {
                    for (int k = j + 1; k < regionCluster.Count; k++)
                    {
                        if (regionBounds[j].IsInRange(regionBounds[k]))
                        {
                            List<Point> intPts = regionCluster[j].CurveIntersections(regionCluster[k], tolerance);
                            iPts[j].AddRange(intPts);
                            iPts[k].AddRange(intPts);
                        }
                    }
                }
                
                for (int i = 0; i < regionCluster.Count; i++)
                {
                    if (iPts[i].Count() > 0)
                    {
                        List<PolyCurve> splReg = regionCluster[i].SplitAtPoints(iPts[i]);
                        foreach (PolyCurve segment in splReg)
                        {
                            bool flag = true;
                            List<Point> mPts = new List<Point> { segment.PointAtParameter(0.5) };

                            for (int j = 0; j < regionCluster.Count; j++)
                            {
                                if (i != j && regionBounds[j].IsContaining(mPts) && regionCluster[j].IsContaining(mPts, true, tolerance))
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

                for (int i = 0; i < tmpResult.Count - 1; i++)
                {
                    for (int j = i + 1; j < tmpResult.Count; j++)
                    {
                        if (tmpResult[i].IIsEqual(tmpResult[j]) || tmpResult[i].IIsEqual(tmpResult[j].IFlip()))
                            tmpResult.RemoveAt(j);
                    }
                }

                result.AddRange(BH.Engine.Geometry.Compute.Join(tmpResult, tolerance));
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
    }
}