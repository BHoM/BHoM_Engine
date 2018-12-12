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
                    return aRPt.SquareDistance(splitLine[0]) > sqTol && aPt.SquareDistance(refLine) > sqTol ? null : line.Clone();
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

            Line result = lines[0].Clone();
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
            if (region.IsCoplanar(refRegion, tolerance))
            {
                double sqTol = tolerance * tolerance;
                List<Polyline> result = new List<Polyline>();
                List<Point> iPts = region.LineIntersections(refRegion, tolerance);
                List<Polyline> splitRegion1 = region.SplitAtPoints(iPts, tolerance);
                List<Polyline> splitRegion2 = refRegion.SplitAtPoints(iPts, tolerance);

                foreach (Polyline segment in splitRegion1)
                {
                    List<Point> cPts = segment.SubParts().Select(s => s.ControlPoints().Average()).ToList();
                    cPts.AddRange(segment.ControlPoints);

                    if (refRegion.IsContaining(cPts, true, tolerance))
                        result.Add(segment);
                }

                foreach (Polyline segment in splitRegion2)
                {
                    List<Point> cPts = segment.SubParts().Select(s => s.ControlPoints().Average()).ToList();
                    cPts.AddRange(segment.ControlPoints);

                    if (region.IsContaining(cPts, true, tolerance))
                    {
                        foreach (Point cPt in cPts)
                        {
                            if (cPt.SquareDistance(region.ClosestPoint(cPt)) > sqTol)
                            {
                                result.Add(segment);
                                break;
                            }
                        }
                    }
                }

                result = result.Join(tolerance);
                int i = 0;
                while (i < result.Count)
                {
                    if (result[i].Area() <= sqTol)
                        result.RemoveAt(i);
                    else
                        i++;
                }

                return result;

            }

            return new List<Polyline>();
        }

        /***************************************************/

        public static List<Polyline> BooleanIntersection(this List<Polyline> regions, double tolerance = Tolerance.Distance)
        {
            List<Polyline> result = new List<Polyline>();
            if (regions[0].Length() <= tolerance)
                return result;

            result.Add(regions[0].Clone());
            for (int i = 1; i < regions.Count; i++)
            {
                List<Polyline> newResult = new List<Polyline>();
                result.ForEach(r => newResult.AddRange(r.BooleanIntersection(regions[i], tolerance)));
                result = newResult;
            }

            return result;
        }

        /***************************************************/
    }
}
