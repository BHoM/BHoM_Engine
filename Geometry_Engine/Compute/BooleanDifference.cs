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
        
        public static List<Line> BooleanDifference(this Line line, Line refLine)
        {
            if (refLine.Length() <= Tolerance.Distance) return new List<Line> { line.Clone() };
            if (line.IsColinear(refLine))
            {
                List<Line> splitLine = line.SplitAtPoints(refLine.ControlPoints());
                if (splitLine.Count == 3)
                {
                    splitLine.RemoveAt(1);
                }
                else if (splitLine.Count == 2)
                {
                    Point aPt = refLine.ControlPoints().Average();
                    if (line.Start.SquareDistance(aPt) < line.End.SquareDistance(aPt)) splitLine.RemoveAt(0);
                    else splitLine.RemoveAt(1);
                }
                else
                {
                    Point aPt = splitLine[0].ControlPoints().Average();
                    Point aRPt = refLine.ControlPoints().Average();
                    if (aRPt.SquareDistance(splitLine[0]) > Tolerance.SqrtDist && aPt.SquareDistance(refLine) > Tolerance.SqrtDist) splitLine = new List<Line> { line.Clone() };
                    else splitLine = new List<Line>();
                }
                return splitLine;
            }
            return new List<Line> { line.Clone() };
        }

        /***************************************************/

        public static List<Line> BooleanDifference(this List<Line> lines, List<Line> refLines)
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
                        if (split) break;
                        for (int j = 0; j < splitLine.Count; j++)
                        {
                            Line l = splitLine[j];
                            List<Line> bd = l.BooleanDifference(refLines[i]);
                            if (bd.Count == 0)
                            {
                                k = refLines.Count;
                                splitLine = new List<Line>();
                                split = true;
                                break;
                            }
                            else if (bd.Count > 1 || bd[0].Length() != l.Length())
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

        public static List<Polyline> BooleanDifference(this Polyline region, Polyline refRegion)
        {
            if (region.IsCoplanar(refRegion))
            {
                List<Polyline> result = new List<Polyline>();
                List<Point> iPts = region.LineIntersections(refRegion);
                List<Polyline> splitRegion1 = region.SplitAtPoints(iPts);
                List<Polyline> splitRegion2 = refRegion.SplitAtPoints(iPts);
                foreach (Polyline segment in splitRegion1)
                {
                    List<Point> cPts = segment.SubParts().Select(s => s.ControlPoints().Average()).ToList();
                    cPts.AddRange(segment.ControlPoints);
                    if (!refRegion.IsContaining(cPts, false))
                    {
                        foreach (Point pt in cPts)
                        {
                            if (region.ClosestPoint(pt).SquareDistance(pt) > Tolerance.SqrtDist)
                            {
                                continue;
                            }
                        }
                        result.Add(segment);
                    }
                }
                foreach (Polyline segment in splitRegion2)
                {
                    List<Point> cPts = segment.SubParts().Select(s => s.ControlPoints().Average()).ToList();
                    cPts.AddRange(segment.ControlPoints);
                    if (region.IsContaining(cPts, true))
                    {
                        foreach (Point pt in cPts)
                        {
                            if (region.ClosestPoint(pt).SquareDistance(pt) > Tolerance.SqrtDist)
                            {
                                result.Add(segment);
                                break;
                            }
                        }
                    }

                }
                return result.Join();
            }

            return new List<Polyline> { region };
        }

        /***************************************************/

        public static List<Polyline> BooleanDifference(this List<Polyline> regions, List<Polyline> refRegions)
        {
            List<Polyline> result = new List<Polyline>();

            foreach (Polyline region in regions)
            {
                List<Polyline> splitRegion = new List<Polyline> { region.Clone() };
                int k = 0;
                bool split = false;
                do
                {
                    split = false;
                    for (int i = k; i < refRegions.Count; i++)
                    {
                        if (split) break;
                        for (int j = 0; j < splitRegion.Count; j++)
                        {
                            Polyline r = splitRegion[j];
                            List<Polyline> bd = r.BooleanDifference(refRegions[i]);
                            if (bd.Count == 0)
                            {
                                k = refRegions.Count;
                                splitRegion = new List<Polyline>();
                                split = true;
                                break;
                            }
                            else if (bd.Count > 1 || bd[0].Area() != r.Area())
                            {
                                k = i + 1;
                                split = true;
                                splitRegion.RemoveAt(j);
                                splitRegion.AddRange(bd);
                                break;
                            }
                        }
                    }
                } while (split);
                result.AddRange(splitRegion);
            }
            return result;
        }
    }
}
