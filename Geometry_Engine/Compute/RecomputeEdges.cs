using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<List<Polyline>> RecomputeEdges(Polyline panelOutline, List<Polyline> openingOutlines)
        {
            List<Polyline> removedDuplicates = panelOutline.RemoveDuplicateEdges();
            foreach (Polyline o in openingOutlines)
            {
                removedDuplicates.AddRange(o.RemoveDuplicateEdges());
            }
            List<List<Polyline>> result = removedDuplicates.DistributeOutlines();

            for (int i = result.Count - 1; i >= 0; i--)
            {
                List<Polyline> outlines = result[i];
                List<Polyline> newOutlines = outlines.Take(1).ToList().BooleanDifference(outlines.Skip(1).ToList());
                foreach (List<Polyline> lp in newOutlines.DistributeOutlines())
                {
                    result.Add(new List<Polyline> { lp[0] }.Concat(lp.Skip(1).ToList().BooleanUnion()).ToList());
                }
                result.RemoveAt(i);
            }
            return result;
        }

        /******************************************/
        /***          Private methods           ***/
        /******************************************/

        private static List<Polyline> RemoveDuplicateEdges(this Polyline outline)
        {
            List<Point> intpts = outline.SubParts().LineIntersections();
            List<Line> edgeLines = new List<Line>();
            foreach (Polyline p in outline.SplitAtPoints(intpts))
            {
                edgeLines.AddRange(p.SubParts());
            }
            int ec = edgeLines.Count - 1;
            for (int i = 0; i < ec; i++)
            {
                Line l1 = edgeLines[i];
                for (int j = i + 1; j < edgeLines.Count; j++)
                {
                    Line l2 = edgeLines[j];
                    if (l1.Start.SquareDistance(l2.End) <= Tolerance.SqrtDist && l1.End.SquareDistance(l2.Start) <= Tolerance.SqrtDist)
                    {
                        edgeLines.RemoveAt(j);
                        edgeLines.RemoveAt(i);
                        i--;
                        ec -= 2;
                        break;
                    }
                }
            }
            return edgeLines.Join();
        }

        /******************************************/
    }
}
