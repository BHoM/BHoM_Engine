/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /****      public Methods - Bounding Boxes      ****/
        /***************************************************/

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

        public static Line BooleanIntersection(this Line line, Line refLine, double tolerance = Tolerance.Distance)
        {
            if (line == null || line.Length() <= tolerance || refLine.Length() <= tolerance)
                return null;

            if (line.IsCollinear(refLine, tolerance))
            {
                List<Line> splitLine = line.SplitAtPoints(refLine.ControlPoints(), tolerance);
                if (splitLine.Count == 3)
                    return splitLine[1];
                else if (splitLine.Count == 2)
                {
                    Point aPt = refLine.ControlPoints().Average();
                    return line.Start.SquareDistance(aPt) < line.End.SquareDistance(aPt) ? splitLine[0] : splitLine[1];
                }
                else
                {
                    double sqTol = tolerance * tolerance;
                    Point aPt = splitLine[0].ControlPoints().Average();
                    Point aRPt = refLine.ControlPoints().Average();
                    return aRPt.SquareDistance(splitLine[0]) > sqTol && aPt.SquareDistance(refLine) > sqTol ? null : line.DeepClone();
                }
            }

            return null;
        }

        /***************************************************/

        public static List<Line> BooleanIntersection(this Line line, List<Line> refLines, double tolerance = Tolerance.Distance)
        {
            List<Line> result = new List<Line>();
            if (line.Length() <= tolerance)
                return result;

            foreach (Line l in refLines.BooleanUnion(tolerance))
            {
                Line intersection = line.BooleanIntersection(l, tolerance);
                if (intersection != null)
                    result.Add(intersection);
            }

            return result;
        }

        /***************************************************/

        public static Line BooleanIntersection(this List<Line> lines, double tolerance = Tolerance.Distance)
        {
            if (lines[0].Length() <= tolerance)
                return null;

            Line result = lines[0].DeepClone();
            for (int i = 1; i < lines.Count; i++)
            {
                result = result.BooleanIntersection(lines[i], tolerance);
            }

            return result;
        }


        /***************************************************/
        /****         public Methods - Regions          ****/
        /***************************************************/

        public static List<Polyline> BooleanIntersection(this Polyline region, Polyline refRegion, double tolerance = Tolerance.Distance)
        {
            if (!region.IsClosed(tolerance) || !refRegion.IsClosed(tolerance))
            {
                Reflection.Compute.RecordError("Boolean Union works on closed regions.");
                return new List<Polyline>();
            }

            if (!region.IsCoplanar(refRegion, tolerance))
                return new List<Polyline>();

            double sqTol = tolerance * tolerance;
            List<Polyline> tmpResult = new List<Polyline>();
            List<Point> iPts = region.LineIntersections(refRegion, tolerance);

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

            List<Polyline> result = Join(tmpResult, tolerance);

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

        public static List<PolyCurve> BooleanIntersection(this ICurve region, ICurve refRegion, double tolerance = Tolerance.Distance)
        {
            if (region is NurbsCurve || region is Ellipse || refRegion is NurbsCurve || refRegion is Ellipse)
            {
                Reflection.Compute.RecordError("NurbsCurves and ellipses are not implemented for BooleanIntersection.");
                return null;
            }

            if (!region.IIsClosed(tolerance) || !refRegion.IIsClosed(tolerance))
            {
                Reflection.Compute.RecordError("Boolean Intersection works on closed regions.");
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

            List<PolyCurve> result = IJoin(tmpResult, tolerance).ToList();

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
                List<PolyCurve> newResult = new List<PolyCurve>();
                result.ForEach(r => newResult.AddRange(r.BooleanIntersection((ICurve)regionListPolyCurve[i], tolerance)));
                result = newResult;
            }

            return result;
        }

        /**************************************************
        /***          Private methods                    ***/
        /***************************************************/

        private static Boolean IsSimilarSegment(this ICurve curve, ICurve refCurve, double tolerance = Tolerance.Distance)
        {
            double sqTol = tolerance * tolerance;
            Point sp = curve.IStartPoint();
            Point rsp = refCurve.IStartPoint();
            Point mp = curve.IPointAtParameter(0.5);
            Point rmp = refCurve.IPointAtParameter(0.5);
            Point ep = curve.IEndPoint();
            Point rep = refCurve.IEndPoint();

            return mp.SquareDistance(rmp) <= sqTol && 
                   ((sp.SquareDistance(rsp) <= sqTol && ep.SquareDistance(rep) <= sqTol) ||
                   (sp.SquareDistance(rep) <= sqTol && ep.SquareDistance(rsp) <= sqTol));
        }

        /***************************************************/      

    }
}


