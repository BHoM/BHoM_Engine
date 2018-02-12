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

        public static bool BooleanDifference(this Polyline region, Polyline refRegion, out List<Polyline> result)
        {
            List<Polyline> cutRegions = region.BooleanIntersection(refRegion);
            result = new List<Polyline>();

            if (region.IsCoplanar(refRegion))
            {
                List<Point> iPts = new List<Point>();
                foreach (Polyline cr in cutRegions)
                {
                    iPts.AddRange(region.LineIntersections(cr));
                }
                List<Polyline> splitRegion = region.SplitAtPoints(iPts);
                if (splitRegion.Count == 1)
                {
                    result.Add(region.Clone());
                    foreach (Point cPt in refRegion.ControlPoints)
                    {
                        if (!region.IsContaining(new List<Point> { cPt }))
                        {
                            return false;
                        }
                    }
                    result.Add(refRegion.Clone());
                    return false;
                }
                foreach (Polyline segment in splitRegion)
                {
                    List<Point> cPts = segment.SubParts().Select(s => s.ControlPoints().Average()).ToList();
                    cPts.AddRange(segment.ControlPoints);
                    bool isInRegion = false;
                    foreach (Polyline cr in cutRegions)
                    {
                        if (cr.IsContaining(cPts, true))
                        {
                            isInRegion = true;
                            break;
                        }
                    }
                    if (!isInRegion) result.Add(segment);
                }
                foreach (Polyline cr in cutRegions)
                {
                    splitRegion = cr.SplitAtPoints(iPts);
                    foreach (Polyline segment in splitRegion)
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
                }
                result = result.Join();
                return true;
            }

            result.Add(region.Clone());
            return false;
        }

        /***************************************************/

        public static List<Polyline> BooleanDifference(this List<Polyline> regions, List<Polyline> refRegions)
        {
            List<Polyline> result = new List<Polyline>();
            List<Polyline> openings = new List<Polyline>();

            foreach (Polyline region in regions)
            {
                List<Polyline> splitRegion = new List<Polyline> { region.Clone() };
                int k = 0;
                bool split;
                do
                {
                    split = false;
                    for (int i = k; i < refRegions.Count; i++)
                    {
                        if (split) break;
                        for (int j = 0; j < splitRegion.Count; j++)
                        {
                            List<Polyline> bd;
                            split = splitRegion[j].BooleanDifference(refRegions[i], out bd);

                            if (split && bd.Count == 0)
                            {
                                k = refRegions.Count;
                                splitRegion = new List<Polyline>();
                                break;
                            }
                            else if (split)
                            {
                                k = i + 1;
                                splitRegion.RemoveAt(j);
                                splitRegion.AddRange(bd);
                                break;
                            }
                            else if (!split && bd.Count > 1)
                            {
                                openings.AddRange(bd.Skip(1));
                            }
                        }
                    }
                } while (split);
                result.AddRange(splitRegion);
            }
            result.AddRange(openings);
            return result;
        }
    }
}
