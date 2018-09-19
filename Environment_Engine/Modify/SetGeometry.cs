using BH.oM.Environment.Elements;
using BH.oM.Environment.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IBuildingElementGeometry ISetGeometry(this IBuildingObject buildingObject, ICurve curve)
        {
            return SetGeometry(buildingObject as dynamic, curve as dynamic);
        }

        /***************************************************/

        public static Panel SetGeometry(this Panel buildingElementPanel, PolyCurve polyCurve)
        {
            Panel aBuildingElementPanel = buildingElementPanel.GetShallowClone() as Panel;
            aBuildingElementPanel.PanelCurve = polyCurve;
            return aBuildingElementPanel;
        }

        /***************************************************/

        public static Opening SetGeometry(this Opening opening, PolyCurve polyCurve)
        {
            Opening aOpening = opening.GetShallowClone() as Opening;
            aOpening.OpeningCurve = polyCurve;
            return aOpening;
        }

        /***************************************************/

        public static BuildingElement SetGeometry(this BuildingElement element, PolyCurve polyCurve)
        {
            BuildingElement aElement = element.GetShallowClone() as BuildingElement;
            aElement.PanelCurve = polyCurve;
            return aElement;
        }
    }
}
