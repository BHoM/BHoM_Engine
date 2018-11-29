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

        public static Opening SetInternal2DElements(this Opening opening, List<IElement2D> internal2DElements)
        {
            if (internal2DElements.Count != 0)
            {
                Reflection.Compute.RecordError("Cannot set internal 2D elements to an opening.");
                return null;
            }

            return opening.GetShallowClone() as Opening;
        }

        /***************************************************/

        public static PanelPlanar SetInternal2DElements(this PanelPlanar panelPlanar, List<IElement2D> internal2DElements)
        {
            PanelPlanar pp = panelPlanar.GetShallowClone() as PanelPlanar;
            pp.Openings = new List<Opening>(internal2DElements.Cast<Opening>().ToList());
            return pp;
        }

        /***************************************************/
    }
}
