using BH.oM.Structural.Elements;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static PanelPlanar PanelPlanar(ICurve outline, List<Opening> openings = null)
        {
            if (!outline.IIsClosed()) return null;
            PanelPlanar panel = new PanelPlanar();
            panel.ExternalEdges = outline.ISubParts().Select(x => new Edge { Curve = x }).ToList();
            panel.Openings = openings;
            return panel;
        }

        /***************************************************/

        public static PanelPlanar PanelPlanar(ICurve outline, List<ICurve> openings = null)
        {
            if (!outline.IIsClosed()) return null;
            List<Opening> pOpenings = openings != null ? openings.Select(o => Create.Opening(o)).ToList() : new List<Opening>();
            PanelPlanar panel = new PanelPlanar();
            panel.ExternalEdges = outline.ISubParts().Select(x => new Edge { Curve = x }).ToList();
            panel.Openings = pOpenings;
            return panel;
        }

        /***************************************************/

        public static List<PanelPlanar> PanelPlanar(List<ICurve> outlines)
        {
            List<PanelPlanar> result = new List<PanelPlanar>();
            List<Polyline> pOutlines = outlines.Select(o => (Polyline)o).ToList();
            List<List<Polyline>> sortedOutlines = pOutlines.DistributeOutlines();
            foreach (List<Polyline> panelOutlines in sortedOutlines)
            {
                List<Edge> externalEdges = panelOutlines[0].SubParts().Select(o => new Edge { Curve = o }).ToList();
                List<Opening> openings = new List<Opening>();
                foreach (Polyline p in panelOutlines.Skip(1))
                {
                    List<Edge> openingEdges = p.SubParts().Select(o => new Edge { Curve = o }).ToList();
                    openings.Add(new Opening { Edges = openingEdges });
                }
                result.AddRange((new PanelPlanar { ExternalEdges = externalEdges, Openings = openings }).RecomputeEdges());
            }
            return result;
        }

        /***************************************************/
    }
}
