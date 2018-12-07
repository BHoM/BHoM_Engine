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

        public static Opening SetInternalElements2D(this Opening opening, List<IElement2D> internalElements2D)
        {
            if (internalElements2D.Count != 0)
            {
                Reflection.Compute.RecordError("Cannot set internal 2D elements to an opening.");
                return null;
            }

            return opening.GetShallowClone() as Opening;
        }

        /***************************************************/

        public static PanelPlanar SetInternalElements2D(this PanelPlanar panelPlanar, List<IElement2D> internalElements2D)
        {
            PanelPlanar pp = panelPlanar.GetShallowClone() as PanelPlanar;
            pp.Openings = new List<Opening>(internalElements2D.Cast<Opening>().ToList());
            return pp;
        }

        /***************************************************/
    }
}
