using BH.oM.Geometry;
using BH.oM.Environment.Elements;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Base;

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
            o.OpeningCurve = new PolyCurve { Curves = outline.Select(e => e as ICurve).ToList() };
            return o;
        }

        /***************************************************/

        public static Panel SetOutline(this Panel panel, List<IElement1D> outline, bool cloneOpenings = true)
        {
            Panel pp = panel.GetShallowClone() as Panel;
            pp.PanelCurve = new PolyCurve { Curves = outline.Select(e => e as ICurve).ToList() };

            if (cloneOpenings)
            {
                pp.Openings = new List<Opening>(pp.Openings);
                for (int i = 0; i < pp.Openings.Count; i++)
                {
                    pp.Openings[i] = pp.Openings[i].GetShallowClone() as Opening;
                }
            }

            return pp;
        }

        /***************************************************/

        public static Panel SetOutline(this Panel panel, List<IElement1D> outline)
        {
            return panel.SetOutline(outline, false);
        }

        /***************************************************/
    }
}
