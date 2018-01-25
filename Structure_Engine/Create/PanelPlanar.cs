using BH.oM.Structural.Elements;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using BHE = BH.Engine.Geometry;
using BHS = BH.Engine.Structure;

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
    }
}
