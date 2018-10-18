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

        public static IBuildingObject ISetGeometry(this IBuildingObject buildingObject, ICurve curve)
        { 
            return SetGeometry(buildingObject as dynamic, curve as dynamic);
        }

        /***************************************************/

        public static Panel SetGeometry(this Panel buildingElementPanel, ICurve curve)
        {
            Panel aBuildingElementPanel = buildingElementPanel.GetShallowClone() as Panel;
            aBuildingElementPanel.PanelCurve = curve;
            return aBuildingElementPanel;
        }

        /***************************************************/

        public static Opening SetGeometry(this Opening opening, ICurve curve)
        {
            Opening aOpening = opening.GetShallowClone() as Opening;
            aOpening.OpeningCurve = curve;
            return aOpening;
        }

        /***************************************************/

        public static BuildingElement SetGeometry(this BuildingElement element, ICurve curve)
        {
            BuildingElement aElement = element.GetShallowClone() as BuildingElement;
            aElement.PanelCurve = curve;
            return aElement;
        }
    }
}
