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

        public static Opening SetOutlineElements1D(this Opening opening, List<IElement1D> outlineElements1D)
        {
            Opening o = opening.GetShallowClone() as Opening;
            o.OpeningCurve = new PolyCurve { Curves = outlineElements1D.Cast<ICurve>().ToList() };
            return o;
        }

        /***************************************************/

        public static Panel SetOutlineElements1D(this Panel panel, List<IElement1D> outlineElements1D)
        {
            Panel pp = panel.GetShallowClone() as Panel;
            pp.PanelCurve = new PolyCurve { Curves = outlineElements1D.Cast<ICurve>().ToList() };
            return pp;
        }

        /***************************************************/

        public static BuildingElement SetOutlineElements1D(this BuildingElement element, List<IElement1D> outlineElements1D)
        {
            BuildingElement be = element.GetShallowClone() as BuildingElement;
            element.PanelCurve = new PolyCurve { Curves = outlineElements1D.Cast<ICurve>().ToList() };
            return be;
        }

        /***************************************************/
    }
}
