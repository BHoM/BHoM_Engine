using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Geometry;
using BH.oM.Environmental.Interface;
using BH.oM.Environmental.Elements;

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
