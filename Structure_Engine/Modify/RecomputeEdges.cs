using System.Collections.Generic;
using System.Linq;
using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<PanelPlanar> RecomputeEdges(this PanelPlanar panel)
        {
            List<PanelPlanar> result = new List<PanelPlanar>();

            List<Polyline> removedDuplicates = panel.Outline().RemoveDuplicateEdges();
            foreach (Opening o in panel.Openings)
            {
                removedDuplicates.AddRange(o.Outline().RemoveDuplicateEdges());
            }
            List<List<Polyline>> distributedOutlines = removedDuplicates.DistributeOutlines();
            
            for(int i = distributedOutlines.Count - 1; i >= 0; i--)
            {
                List<Polyline> outlines = distributedOutlines[i];
                List<Polyline> newOutlines = outlines.Take(1).ToList().BooleanDifference(outlines.Skip(1).ToList());
                distributedOutlines.AddRange(newOutlines.DistributeOutlines());
                distributedOutlines.RemoveAt(i);
            }

            foreach (List<Polyline> panelOutlines in distributedOutlines)
            {
                PanelPlanar pp = (PanelPlanar)panel.GetShallowClone(true);
                pp.ExternalEdges = new List<Edge>();
                pp.Openings = new List<Opening>();
                List<Edge> externalEdges = new List<Edge>();
                foreach (Line l in panelOutlines[0].SubParts())
                {
                    Edge edge = null;
                    bool edgeFound = false;
                    foreach (Opening o in panel.Openings)
                    {
                        if (edgeFound) break;
                        edge = l.AssignEdgeProperties(o.Edges, out edgeFound);
                    }
                    if (!edgeFound)
                    {
                        edge = l.AssignEdgeProperties(panel.ExternalEdges, out edgeFound);
                    }

                    if (edgeFound) externalEdges.Add(edge);
                    else externalEdges.Add(new Edge { Curve = l });
                }
                pp.ExternalEdges = externalEdges;

                foreach (Polyline p in panelOutlines.Skip(1).ToList().BooleanUnion())
                {
                    List<Edge> oEdges = new List<Edge>();
                    Edge edge = null;
                    foreach (Line l in p.SubParts())
                    {
                        bool edgeFound = false;
                        foreach (Opening o in panel.Openings)
                        {
                            if (edgeFound) break;
                            edge = l.AssignEdgeProperties(o.Edges, out edgeFound);
                        }

                        if (edgeFound) oEdges.Add(edge);
                        else oEdges.Add(new Edge { Curve = l });
                    }
                    pp.Openings.Add(new Opening { Edges = oEdges });
                }

                result.Add(pp);
            }

            return result;
        }


        /******************************************/
        /***          Private methods           ***/
        /******************************************/

        private static Edge AssignEdgeProperties(this Line line, List<Edge> refEdges, out bool edgeFound)
        {
            edgeFound = false;
            foreach (Edge e in refEdges)
            {
                Line el = (Line)e.Curve;
                if (el.ClosestPoint(line.Start).SquareDistance(line.Start) <= Tolerance.SqrtDist && el.ClosestPoint(line.End).SquareDistance(line.End) <= Tolerance.SqrtDist)
                {
                    Edge ce = (Edge)e.GetShallowClone(true);
                    ce.Curve = line;
                    edgeFound = true;
                    return ce;
                }
            }
            return null;
        }

        /***************************************************/

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
