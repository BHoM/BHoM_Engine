using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Structure.Elements;
using BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static PanelPlanar ToPanelPlanar(this Panel Panel)
        {
            return Structure.Create.PanelPlanar(Panel.PanelCurve, new List<BH.oM.Structure.Elements.Opening>());
        }

        /***************************************************/

        public static PanelPlanar ToPanelPlanar(this BuildingElement element)
        {
            return Structure.Create.PanelPlanar(element.PanelCurve, new List<oM.Structure.Elements.Opening>());
        }
    }
}

