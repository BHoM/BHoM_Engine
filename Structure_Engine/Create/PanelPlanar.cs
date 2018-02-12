using BH.oM.Structural.Elements;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using BHE = BH.Engine.Geometry;
using BHS = BH.Engine.Structure;
using ML = BH.Engine.ModelLaundry;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static PanelPlanar PanelPlanar(ICurve outline, List<Opening> openings = null)
        {
            if (!BHE.Query.IIsClosed(outline)) return null;
            PanelPlanar panel = new PanelPlanar();
            panel.ExternalEdges = BHE.Query.ISubParts(outline).Select(x => new Edge { Curve = x }).ToList();
            panel.Openings = openings;
            return panel;
        }

        /***************************************************/

        public static PanelPlanar PanelPlanar(ICurve outline, List<ICurve> openings = null)
        {
            if (!BHE.Query.IIsClosed(outline)) return null;
            List<Opening> pOpenings = openings != null ? openings.Select(o => Create.Opening(o)).ToList() : new List<Opening>();
            PanelPlanar panel = new PanelPlanar();
            panel.ExternalEdges = BHE.Query.ISubParts(outline).Select(x => new Edge { Curve = x }).ToList();
            panel.Openings = pOpenings;
            return panel;
        }

        /***************************************************/

        public static List<PanelPlanar> PanelPlanar(List<ICurve> outlines)
        {
            List<PanelPlanar> result = new List<PanelPlanar>();
            List<Polyline> pOutlines = outlines.Select(o => (Polyline)o).ToList();
            List<List<Polyline>> sortedOutlines = ML.Compute.DistributeOutlines(pOutlines);
            foreach (List<Polyline> panelOutlines in sortedOutlines)
            {
                List<Edge> externalEdges = BHE.Query.SubParts(panelOutlines[0]).Select(o => new Edge { Curve = o }).ToList();
                List<Opening> openings = new List<Opening>();
                foreach (Polyline p in panelOutlines.Skip(1))
                {
                    List<Edge> openingEdges = BHE.Query.SubParts(p).Select(o => new Edge { Curve = o }).ToList();
                    openings.Add(new Opening { Edges = openingEdges });
                }
                result.Add(new PanelPlanar { ExternalEdges = externalEdges, Openings = openings });
            }
            return result;
        }
    }
}
