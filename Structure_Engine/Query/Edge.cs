using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<ICurve> Edges(this PanelFreeForm contour)
        {
            if (contour.Surface != null)
                return contour.Surface.IExternalEdges();
            else
                return new List<ICurve>();
        }

        /***************************************************/

        public static List<ICurve> InternalEdgeCurves(this PanelPlanar panel)
        {
            // Todo:
            // - return the edges as polycurves -> curve.Join needed

            List<ICurve> edges = new List<ICurve>();
            foreach (Opening o in panel.Openings)
            {
                edges.AddRange(o.Edges.Select(e => e.Curve).ToList());
            }
            return edges;
        }

        /***************************************************/

        public static List<ICurve> ExternalEdgeCurves(this PanelPlanar panel)
        {
            return panel.ExternalEdges.Select(x => x.Curve).ToList();
        }

        /***************************************************/

        public static List<ICurve> AllEdgeCurves(this PanelPlanar panel)
        {
            List<ICurve> result = panel.ExternalEdgeCurves();
            result.AddRange(panel.InternalEdgeCurves());
            return result;
        }

        /***************************************************/
    }

}
