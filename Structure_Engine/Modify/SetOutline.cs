using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Base;

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
            o.Edges = new List<Edge>(outline.Select(e => e as Edge));
            return o;
        }

        /***************************************************/

        public static PanelPlanar SetOutline(this PanelPlanar panelPlanar, List<IElement1D> outline, bool cloneOpenings = true)
        {
            PanelPlanar pp = panelPlanar.GetShallowClone() as PanelPlanar;
            pp.ExternalEdges = new List<Edge>(outline.Select(e => e as Edge));

            if (cloneOpenings)
            {
                pp.Openings = new List<Opening>(pp.Openings);
                for (int i = 0; i < pp.Openings.Count; i++)
                {
                    pp.Openings[i] = pp.Openings[i].SetOutline(pp.Openings[i].Edges.Select(e => e.GetShallowClone() as IElement1D).ToList());
                }
            }

            return pp;
        }

        /***************************************************/

        public static PanelPlanar SetOutline(this PanelPlanar panelPlanar, List<IElement1D> outline)
        {
            return panelPlanar.SetOutline(outline, false);
        }

        /***************************************************/
    }
}
