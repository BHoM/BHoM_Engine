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

        public static Line BooleanIntersection(this Line line, Line refLine)
        {
            if (line == null || line.Length() <= Tolerance.Distance || refLine.Length() <= Tolerance.Distance) return null;
            if (line.IsColinear(refLine))
            {
                List<Line> splitLine = line.SplitAtPoints(refLine.ControlPoints());
                if (splitLine.Count == 3)
                {
                    return splitLine[1];
                }
                else if (splitLine.Count == 2)
                {
                    Point aPt = refLine.ControlPoints().Average();
                    if (line.Start.SquareDistance(aPt) < line.End.SquareDistance(aPt)) return splitLine[0];
                    return splitLine[1];
                }
                else
                {
                    Point aPt = splitLine[0].ControlPoints().Average();
                    Point aRPt = refLine.ControlPoints().Average();
                    if (aRPt.SquareDistance(splitLine[0]) > Tolerance.SqrtDist && aPt.SquareDistance(refLine) > Tolerance.SqrtDist) return null;
                    return line.Clone();
                }
            }
            return null;
        }

        /***************************************************/

        public static List<Line> BooleanIntersection(this Line line, List<Line> refLines)
        {
            List<Line> result = new List<Line>();
            if (line.Length() <= Tolerance.Distance) return result;
            foreach (Line l in refLines)
            {
                Line intersection = line.BooleanIntersection(l);
                if (intersection != null) result.Add(intersection);
            }
            result = result.BooleanUnion();
            return result;
        }

        /***************************************************/

        public static Line BooleanIntersection(this List<Line> lines)
        {
            if (lines[0].Length() <= Tolerance.Distance) return null;
            Line result = lines[0].Clone();
            for (int i = 1; i < lines.Count; i++)
            {
                result = result.BooleanIntersection(lines[i]);
            }
            return result;
        }


        /***************************************************/
        /****         public Methods - Regions          ****/
        /***************************************************/

        public static List<Polyline> BooleanIntersection(this Polyline region1, Polyline refRegion)
        {
            if (region1.IsCoplanar(refRegion))
            {
                List<Polyline> result = new List<Polyline>();
                List<Point> iPts = region1.LineIntersections(refRegion);
                List<Polyline> splitRegion1 = region1.SplitAtPoints(iPts);
                List<Polyline> splitRegion2 = refRegion.SplitAtPoints(iPts);
                foreach (Polyline segment in splitRegion1)
                {
                    List<Point> cPts = segment.SubParts().Select(s => s.ControlPoints().Average()).ToList();
                    cPts.AddRange(segment.ControlPoints);
                    if (refRegion.IsContaining(cPts, true)) result.Add(segment);
                }
                foreach (Polyline segment in splitRegion2)
                {
                    List<Point> cPts = segment.SubParts().Select(s => s.ControlPoints().Average()).ToList();
                    cPts.AddRange(segment.ControlPoints);
                    if (region1.IsContaining(cPts, true))
                    {
                        foreach (Point cPt in cPts)
                        {
                            if (cPt.SquareDistance(region1.ClosestPoint(cPt)) > Tolerance.SqrtDist)
                            {
                                result.Add(segment);
                                break;
                            }
                        }
                    }
                }
                result = result.Join();
                int i = 0;
                while (i < result.Count)
                {
                    if (result[i].Area() <= Tolerance.SqrtDist) result.RemoveAt(i);
                    else i++;
                }
                return result;

            }

            return new List<Polyline>();
        }

        /***************************************************/

        public static List<Polyline> BooleanIntersection(this List<Polyline> regions)
        {
            List<Polyline> result = new List<Polyline>();
            if (regions[0].Length() <= Tolerance.Distance) return result;
            result.Add(regions[0].Clone());
            for (int i = 1; i < regions.Count; i++)
            {
                List<Polyline> newResult = new List<Polyline>();
                foreach(Polyline r in result)
                {
                    newResult.AddRange(r.BooleanIntersection(regions[i]));
                }
                result = newResult;
            }
            return result;
        }
    }
}
