using BH.oM.Geometry;
using BH.oM.Environment.Interface;
using BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ICurve Curve(this Panel buildingElementPanel)
        {
            return buildingElementPanel.PanelCurve;
        }

        public static ICurve Curve(this BuildingElement buildingElement)
        {
            return buildingElement.PanelCurve;
        }

        public static ICurve Curve(this Opening opening)
        {
            return opening.OpeningCurve;
        }

        public static ICurve ICurve(this IBuildingObject buildingObject)
        {
            return Curve(buildingObject as dynamic);
        }

        /***************************************************/
    }
}
