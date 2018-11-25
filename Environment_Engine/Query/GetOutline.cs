using BH.Engine.Geometry;
using BH.oM.Common;
using BH.oM.Environment.Elements;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        public static List<IElement1D> GetOutline(this Opening opening)
        {
            return opening.OpeningCurve.ISubParts().Select(e => e as IElement1D).ToList();
        }

        /***************************************************/

        public static List<IElement1D> GetOutline(this Panel panel)
        {
            return panel.PanelCurve.ISubParts().Select(e => e as IElement1D).ToList();
        }

        /***************************************************/
    }
}
