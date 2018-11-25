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

        public static Opening SetInternal2DElements(this Opening opening, List<IElement2D> internal2DElements, bool cloneEdges = true)
        {
            if (internal2DElements.Count != 0)
            {
                Reflection.Compute.RecordError("Cannot set internal 2D elements to an opening.");
                return null;
            }

            Opening o = opening.GetShallowClone() as Opening;
            if (cloneEdges)
            {
                o.Edges = new List<Edge>(o.Edges);
                for (int i = 0; i < o.Edges.Count; i++)
                {
                    o.Edges[i] = o.Edges[i].GetShallowClone() as Edge;
                }
            }

            return o;
        }

        /***************************************************/

        public static Opening SetInternal2DElements(this Opening opening, List<IElement2D> internal2DElements)
        {
            return opening.SetInternal2DElements(internal2DElements, false);
        }

        /***************************************************/

        public static PanelPlanar SetInternal2DElements(this PanelPlanar panelPlanar, List<IElement2D> internal2DElements, bool cloneExternalEdges = true)
        {
            PanelPlanar pp = panelPlanar.GetShallowClone() as PanelPlanar;
            pp.Openings = new List<Opening>(internal2DElements.Select(o => o as Opening).ToList());

            if (cloneExternalEdges)
            {
                pp.ExternalEdges = new List<Edge>(pp.ExternalEdges);
                for (int i = 0; i < pp.ExternalEdges.Count; i++)
                {
                    pp.ExternalEdges[i] = pp.ExternalEdges[i].GetShallowClone() as Edge;
                }
            }

            return pp;
        }

        /***************************************************/

        public static PanelPlanar SetInternal2DElements(this PanelPlanar panelPlanar, List<IElement2D> internal2DElements)
        {
            return panelPlanar.SetInternal2DElements(internal2DElements, false);
        }

        /***************************************************/
    }
}
