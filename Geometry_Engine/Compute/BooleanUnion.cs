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

        public static List<Line> BooleanUnion(this Line line, Line refLine)
        {
            if (line.IsColinear(refLine))
            {
                List<Point> cPts = line.ControlPoints();
                cPts.AddRange(refLine.ControlPoints());
                cPts = cPts.SortCollinear();
                return new List<Line> { new Line { Start = cPts[0], End = cPts.Last() } };
            }

            return new List<Line> { line.Clone(), refLine.Clone() };
        }

        /***************************************************/

        public static List<Line> BooleanUnion(this List<Line> lines)
        {
            List<Line> result = lines.Select(l => l.Clone()).ToList();
            bool union;
            do
            {
                union = false;
                for (int i = 0; i < result.Count - 1; i++)
                {
                    if (union) break;
                    for (int j = i + 1; j < result.Count; j++)
                    {
                        List<Line> uLines = result[i].BooleanUnion(result[j]);
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

        //private static List<Polyline> BooleanUnion(this Polyline region1, Polyline region2)
        //{
        //    if (region1.IsCoplanar(region2))
        //    {
        //        List<Polyline> result = new List<Polyline>();
        //        List<Point> iPts = region1.LineIntersections(region2);
        //        List<Polyline> splitRegion1 = region1.SplitAtPoints(iPts);
        //        List<Polyline> splitRegion2 = region2.SplitAtPoints(iPts);
        //        foreach (Polyline segment in splitRegion1)
        //        {
        //            List<Point> cPts = segment.SubParts().Select(s => s.ControlPoints().Average()).ToList();
        //            cPts.AddRange(segment.ControlPoints);
        //            if (!region2.IsContaining(cPts, true)) result.Add(segment);
        //        }
        //        foreach (Polyline segment in splitRegion2)
        //        {
        //            List<Point> cPts = segment.SubParts().Select(s => s.ControlPoints().Average()).ToList();
        //            cPts.AddRange(segment.ControlPoints);
        //            if (!region1.IsContaining(cPts, true)) result.Add(segment);
        //        }
        //        return result.Join();
        //    }

        //    return new List<Polyline> { region1.Clone(), region2.Clone() };
        //}

        /***************************************************/

        private static bool BooleanUnion(this Polyline region1, Polyline region2, out List<Polyline> result)
        {
            result = new List<Polyline>();
            if (region1.IsCoplanar(region2))
            {
                if (region1.IsContaining(region2.ControlPoints, true))
                {
                    result.Add(region1.Clone());
                }
                else if (region2.IsContaining(region1.ControlPoints, true))
                {
                    result.Add(region2.Clone());
                }
                else
                {
                    List<Point> iPts = region1.LineIntersections(region2);
                    List<Polyline> splitRegion1 = region1.SplitAtPoints(iPts);
                    List<Polyline> splitRegion2 = region2.SplitAtPoints(iPts);
                    if (splitRegion1.Count==1 && splitRegion2.Count == 1)
                    {
                        result = new List<Polyline> { region1.Clone(), region2.Clone() };
                        return false;
                    }

                    foreach (Polyline segment in splitRegion1)
                    {
                        List<Point> cPts = segment.SubParts().Select(s => s.ControlPoints().Average()).ToList();
                        cPts.AddRange(segment.ControlPoints);
                        if (!region2.IsContaining(cPts, true)) result.Add(segment);
                    }
                    foreach (Polyline segment in splitRegion2)
                    {
                        List<Point> cPts = segment.SubParts().Select(s => s.ControlPoints().Average()).ToList();
                        cPts.AddRange(segment.ControlPoints);
                        if (!region1.IsContaining(cPts, true)) result.Add(segment);
                    }
                    result = result.Join();
                }
                return true;
            }
            
            result = new List<Polyline> { region1.Clone(), region2.Clone() };
            return false;
        }

        /***************************************************/

        public static List<Polyline> BooleanUnion(this List<Polyline> regions)
        {
            List<Polyline> result = new List<Polyline>();

            // write this more efficiently! this is retarded?
            List<Point> iPts = new List<Point>();
            List<Line> cutlines = new List<Line>();
            for (int i = 0; i < regions.Count - 1; i++)
            {
                for (int j = i + 1; j < regions.Count; j++)
                {
                    iPts.AddRange(regions[i].LineIntersections(regions[j]));
                }
            }
            foreach (Polyline r in regions)
            {
                foreach (Polyline p in r.SplitAtPoints(iPts))
                {
                    cutlines.AddRange(p.SubParts());
                }
            }
            foreach (Polyline r in regions)
            {
                result.AddRange(r.Split(cutlines));
            }

            List<Polyline> uRegions;
            List<Polyline> openings = new List<Polyline>();
            bool union;
            do
            {
                union = false;
                for (int i = 0; i < result.Count - 1; i++)
                {
                    if (union) break;
                    for (int j = i + 1; j < result.Count; j++)
                    {
                        union = result[i].BooleanUnion(result[j], out uRegions);
                        if (union)
                        {
                            result.RemoveAt(j);
                            result.RemoveAt(i);

                            // make sure that area works!
                            uRegions.Sort(delegate (Polyline p1, Polyline p2)
                            {
                                return p1.Area().CompareTo(p2.Area());
                            });
                            uRegions.Reverse();
                            result.Add(uRegions[0]);
                            openings.AddRange(uRegions.Skip(1));
                            break;
                        }
                    }
                }
            } while (union);
            
            result.AddRange(openings.BooleanDifference(regions));
            return result;
        }
    }
}
