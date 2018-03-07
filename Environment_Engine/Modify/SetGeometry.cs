using BH.oM.Environmental.Elements;
using BH.oM.Environmental.Interface;
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

        public static IBuildingElementGeometry ISetGeometry(this IBuildingElementGeometry buildingElementGeometry, ICurve curve)
        {
            return SetGeometry(buildingElementGeometry as dynamic, curve as dynamic);
        }

        /***************************************************/

        public static BuildingElementPanel SetGeometry(this BuildingElementPanel buildingElementPanel, PolyCurve polyCurve)
        {
            BuildingElementPanel aBuildingElementPanel = buildingElementPanel.GetShallowClone() as BuildingElementPanel;
            aBuildingElementPanel.PolyCurve = polyCurve;
            return aBuildingElementPanel;
        }

        /***************************************************/

        public static BuildingElementCurve SetGeometry(this BuildingElementCurve buildingElementCurve, ICurve curve)
        {
            BuildingElementCurve aBuildingElementCurve = buildingElementCurve.GetShallowClone() as BuildingElementCurve;
            aBuildingElementCurve.Curve = curve;
            return aBuildingElementCurve;
        }

        /***************************************************/
    }
}
