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

        public static ICurve Curve(this BuildingElementCurve buildingElementCurve)
        {
            return buildingElementCurve.Curve;
        }

        public static ICurve Curve(this BuildingElementPanel buildingElementPanel)
        {
            return buildingElementPanel.PolyCurve;
        }

        public static ICurve ICurve(this IBuildingElementGeometry buildingElementGeometry)
        {
            return Curve(buildingElementGeometry as dynamic);
        }

        /***************************************************/
    }
}
