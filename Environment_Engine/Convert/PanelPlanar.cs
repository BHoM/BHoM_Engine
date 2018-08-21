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

        public static PanelPlanar ToPanelPlanar(this BuildingElementPanel buildingElementPanel)
        {
            return Structure.Create.PanelPlanar(buildingElementPanel.PolyCurve, new List<Opening>());
        }

        /***************************************************/
    }
}

