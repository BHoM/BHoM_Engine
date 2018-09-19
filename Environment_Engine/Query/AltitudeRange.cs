using System;
using System.Collections.Generic;
using System.Linq;
using BHEE = BH.oM.Environment.Elements;
using BH.Engine.Geometry;
using BHEI = BH.oM.Environment.Interface;
using BHG = BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double AltitudeRange(this BHEI.IBuildingObject buildingElementGeometry)
        {
            BHG.BoundingBox panelBoundingBox = BH.Engine.Geometry.Query.IBounds(buildingElementGeometry.ICurve());
            double altitudeRange = panelBoundingBox.Max.Z - panelBoundingBox.Min.Z;

            return altitudeRange;
        }

        /***************************************************/
    }
}
