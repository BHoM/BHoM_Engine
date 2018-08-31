using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Structure.Elements;
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

        public static List<ICurve> EdgeCurves(this Opening opening)
        {
            return opening.Edges.Select(e => e.Curve).ToList();
        }

        /***************************************************/

        public static List<Line> Edges(this MeshFace face)
        {
            List<Line> edges = new List<Line>();

            for (int i = 0; i < face.Nodes.Count -1; i++)
            {
                edges.Add(new Line { Start = face.Nodes[i].Position, End = face.Nodes[i + 1].Position });
            }
            edges.Add(new Line { Start = face.Nodes.Last().Position, End = face.Nodes.First().Position });
            return edges;
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IEnumerable<ICurve> IEdges(this IAreaElement element)
        {
            if (element is PanelPlanar)
                return (AllEdgeCurves(element as PanelPlanar));
            else
                return Edges(element as dynamic);
        }

        /***************************************************/
    }

}
