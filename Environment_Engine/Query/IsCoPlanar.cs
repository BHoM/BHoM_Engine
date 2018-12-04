using System;
using System.Collections.Generic;
using BH.oM.Environment.Elements;

using BH.oM.Base;

using System.Linq;

using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsCoPlanar(this BuildingElement element, BuildingElement compare)
        {
            Polyline pLine1 = element.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle);
            Polyline pLine2 = compare.PanelCurve.ICollapseToPolyline(BH.oM.Geometry.Tolerance.Angle);

            return pLine1.IsCoplanar(pLine2);
        }
    }
}