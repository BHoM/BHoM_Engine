using System.Collections.Generic;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /******************************************/
        /****          Panel outline           ****/
        /******************************************/

        public static PolyCurve Outline(this PanelPlanar panel)
        {
            return new PolyCurve { Curves = panel.ExternalEdgeCurves() };
        }


        /******************************************/
        /****         Opening outline          ****/
        /******************************************/

        public static PolyCurve Outline(this Opening opening)
        {
            return new PolyCurve { Curves = opening.EdgeCurves() };
        }

        /******************************************/

        public static PolyCurve Outline(this List<Edge> edges)
        {
            return new PolyCurve { Curves = edges.Select(e => e.Curve).ToList() };
        }

        /******************************************/
    }
}
