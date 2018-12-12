using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        public static Opening SetOutlineElements1D(this Opening opening, List<IElement1D> outlineElements1D)
        {
            Opening o = opening.GetShallowClone() as Opening;
            o.Edges = new List<Edge>(outlineElements1D.Cast<Edge>());
            return o;
        }

        /***************************************************/

        public static PanelPlanar SetOutlineElements1D(this PanelPlanar panelPlanar, List<IElement1D> outlineElements1D)
        {
            PanelPlanar pp = panelPlanar.GetShallowClone() as PanelPlanar;
            pp.ExternalEdges = new List<Edge>(outlineElements1D.Cast<Edge>());
            return pp;
        }

        /***************************************************/
    }
}
