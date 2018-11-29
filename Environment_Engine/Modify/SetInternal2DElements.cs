using BH.oM.Common;
using BH.oM.Environment.Elements;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Environment
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

        public static Panel SetInternal2DElements(this Panel panel, List<IElement2D> internal2DElements)
        {
            Panel pp = panel.GetShallowClone() as Panel;
            pp.Openings = new List<Opening>(internal2DElements.Cast<Opening>().ToList());
            return pp;
        }

        /***************************************************/
    }
}
