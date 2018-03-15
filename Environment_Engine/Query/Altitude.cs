using System;
using System.Collections.Generic;
using System.Linq;
using BHEE = BH.oM.Environmental.Elements;
using BH.Engine.Geometry;
using BHEI = BH.oM.Environmental.Interface;
using BHG = BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double Altitude(this BHEI.IBuildingElementGeometry buildingElementGeometry)
        {
            BHG.BoundingBox panelBoundingBox = BH.Engine.Geometry.Query.IBounds(buildingElementGeometry.ICurve());
            double altitude = panelBoundingBox.Min.Z;

            return altitude;

        }

        /***************************************************/
    }
}
