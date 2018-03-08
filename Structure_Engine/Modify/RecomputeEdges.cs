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
            List<Polyline> panelOutline = new List<Polyline> { panel.Outline() };
            List<Polyline> openingOutlines = panel.Openings.Select(o => o.Outline()).ToList();
            openingOutlines = openingOutlines.BooleanUnion();

            List<Polyline> newOutlines = panelOutline.BooleanDifference(openingOutlines);
            List<List<Polyline>> distributedOutlines = newOutlines.DistributeOutlines();

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

                foreach (Polyline p in panelOutlines.Skip(1))
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

        /******************************************/
    }
}
