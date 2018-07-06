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
            if (line.IsCollinear(refLine))
            {
                List<Point> cPts = line.ControlPoints();
                cPts.AddRange(refLine.ControlPoints());
                cPts = cPts.SortCollinear(tolerance);
                return new List<Line> { new Line { Start = cPts[0], End = cPts.Last() } };
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
                    if (union) break;
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
        /****              Private methods              ****/
        /***************************************************/

        private static bool BooleanUnion(this Polyline region1, Polyline region2, out List<Polyline> result, double tolerance = Tolerance.Distance)
        {
            result = new List<Polyline>();

            if (region1.IsCoplanar(region2, tolerance))
            {
                Plane p1 = region1.FitPlane(tolerance);
                if (!region1.IsClockwise(p1.Normal)) region1 = region1.Clone().Flip();
                if (!region2.IsClockwise(p1.Normal)) region2 = region2.Clone().Flip();

                List<Point> cPts1 = region1.SubParts().Select(s => s.ControlPoints().Average()).ToList();
                cPts1.AddRange(region1.ControlPoints);
                List<Point> cPts2 = region2.SubParts().Select(s => s.ControlPoints().Average()).ToList();
                cPts2.AddRange(region2.ControlPoints);
                if (region1.IsContaining(cPts2, true, tolerance))
                {
                    result.Add(region1.Clone());
                }
                else if (region2.IsContaining(cPts1, true, tolerance))
                {
                    result.Add(region2.Clone());
                }
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
                        if (!region2.IsContaining(cPts, true, tolerance)) result.Add(segment);
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
                        if (!region1.IsContaining(cPts, true, tolerance)) result.Add(segment);
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
