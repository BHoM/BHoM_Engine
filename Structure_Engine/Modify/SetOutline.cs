using BH.oM.Common;
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

        public static Opening SetOutline(this Opening opening, List<IElement1D> outline)
        {
            Opening o = opening.GetShallowClone() as Opening;
            o.Edges = new List<Edge>(outline.Cast<Edge>());
            return o;
        }

        /***************************************************/

        public static PanelPlanar SetOutline(this PanelPlanar panelPlanar, List<IElement1D> outline)
        {
            PanelPlanar pp = panelPlanar.GetShallowClone() as PanelPlanar;
            pp.ExternalEdges = new List<Edge>(outline.Cast<Edge>());
            return pp;
        }

        /***************************************************/
    }
}
