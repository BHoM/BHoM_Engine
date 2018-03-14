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

        public static double IAltitudeRange(this BHEI.IBuildingElementGeometry buildingElementGeometry)
        {
            return AltitudeRange(buildingElementGeometry as dynamic);
        }

        /***************************************************/


        public static double AltitudeRange(this BHEE.BuildingElementPanel panel)
        {
            BHG.BoundingBox panelBoundingBox = BH.Engine.Geometry.Query.IBounds(panel.ICurve());
            double altitudeRange = panelBoundingBox.Max.Z - panelBoundingBox.Min.Z;

            return altitudeRange;

        }

        /***************************************************/
    }
}
