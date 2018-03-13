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

        public static double AltitudeRange(BHEI.IBuildingElementGeometry bHoMBuildingElementPanel)
        {
            BHG.BoundingBox panelBoundingBox = BH.Engine.Geometry.Query.IBounds(bHoMBuildingElementPanel.ICurve());
            float altitudeRange = (float)panelBoundingBox.Max.Z - (float)panelBoundingBox.Min.Z;

            return altitudeRange;

        }

        /***************************************************/
    }
}
