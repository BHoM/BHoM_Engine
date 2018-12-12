using BH.oM.Geometry;
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

        public static Panel SetInternalElements2D(this Panel panel, List<IElement2D> internalElements2D)
        {
            Panel pp = panel.GetShallowClone() as Panel;
            pp.Openings = new List<Opening>(internalElements2D.Cast<Opening>().ToList());
            return pp;
        }

        /***************************************************/

        public static BuildingElement SetInternalElements2D(this BuildingElement panel, List<IElement2D> internalElements2D)
        {
            BuildingElement pp = panel.GetShallowClone() as BuildingElement;
            pp.Openings = new List<Opening>(internalElements2D.Cast<Opening>().ToList());
            return pp;
        }

        /***************************************************/
    }
}
