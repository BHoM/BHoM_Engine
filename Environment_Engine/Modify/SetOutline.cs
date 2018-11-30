using BH.oM.Common;
using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        public static Opening SetOutline(this Opening opening, List<IElement1D> outline)
        {
            Opening o = opening.GetShallowClone() as Opening;
            o.OpeningCurve = new PolyCurve { Curves = outline.Cast<ICurve>().ToList() };
            return o;
        }

        /***************************************************/

        public static Panel SetOutline(this Panel panel, List<IElement1D> outline)
        {
            Panel pp = panel.GetShallowClone() as Panel;
            pp.PanelCurve = new PolyCurve { Curves = outline.Cast<ICurve>().ToList() };
            return pp;
        }

        /***************************************************/

        public static BuildingElement SetOutline(this BuildingElement element, List<IElement1D> outline)
        {
            BuildingElement be = element.GetShallowClone() as BuildingElement;
            element.PanelCurve = new PolyCurve { Curves = outline.Cast<ICurve>().ToList() };
            return be;
        }

        /***************************************************/
    }
}
