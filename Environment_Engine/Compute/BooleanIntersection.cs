using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Environment.Elements;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        /***************************************************/
        /****          public Methods - Lines           ****/
        /***************************************************/

        public static bool DoesBooleanIntersect(this BuildingElement element, BuildingElement elementToCompare)
        {
            Polyline be1P = element.PanelCurve.ICollapseToPolyline(Tolerance.Angle);
            Polyline be2P = elementToCompare.PanelCurve.ICollapseToPolyline(Tolerance.Angle);

            return be1P.BooleanIntersection(be2P).Count > 0;
        }
    }
}
