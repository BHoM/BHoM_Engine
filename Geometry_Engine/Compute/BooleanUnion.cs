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

            return new List<Line> { line.DeepClone(), refLine.DeepClone() };
        }

        /***************************************************/

        public static List<Line> BooleanUnion(this List<Line> lines, double tolerance = Tolerance.Distance)
        {
            List<Line> result = lines.Select(l => l.DeepClone()).ToList();
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
    }
}





