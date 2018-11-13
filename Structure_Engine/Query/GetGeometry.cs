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

        public static Point GetGeometry(this Node node)
        {
            return node.Position;
        }

        /***************************************************/

        public static ICurve GetGeometry(this Bar bar)
        {
            return bar.Centreline();
        }

        /***************************************************/

        public static ICurve GetGeometry(this Edge edge)
        {
            return edge.Curve;
        }

        /***************************************************/

        //public static List<PolyCurve> GetGeometry(this Opening opening)
        //{
        //    return new List<PolyCurve> { new PolyCurve { Curves = opening.Edges.Select(e => e.Curve).ToList() } };
        //}

        ///***************************************************/

        //public static List<PolyCurve> GetGeometry(this PanelPlanar panelPlanar)
        //{
        //    List<PolyCurve> result = new List<PolyCurve> { new PolyCurve { Curves = panelPlanar.ExternalEdges.Select(e => e.Curve).ToList() } };

        //    foreach (Opening opening in panelPlanar.Openings)
        //    {
        //        result.AddRange(opening.GetGeometry());
        //    }

        //    return result;
        //}

        /***************************************************/
    }
}
