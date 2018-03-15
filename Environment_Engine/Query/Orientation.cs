using System;
using System.Collections.Generic;
using BHG = BH.oM.Geometry;
using BHEI = BH.oM.Environmental.Interface;
using BHE = BH.oM.Environmental;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double Orientation(this BHEI.IBuildingElementGeometry buildingElementGeometry)
        {

            double orientation;
            BHG.Polyline pline = new BHG.Polyline { ControlPoints = BH.Engine.Geometry.Query.IControlPoints(buildingElementGeometry.ICurve()) };

            List<BHG.Point> pts = BH.Engine.Geometry.Query.DiscontinuityPoints(pline);
            BHG.Plane plane = BH.Engine.Geometry.Create.Plane(pts[0], pts[1], pts[2]);

            orientation = (BH.Engine.Geometry.Query.Angle(plane.Normal, BHG.Plane.XZ.Normal) * (180 / Math.PI));


            return orientation;
        }

        /***************************************************/
    }


}
