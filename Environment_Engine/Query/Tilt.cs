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

        public static double Tilt(this BHEI.IBuildingElementGeometry buildingElementGeometry)
        {
            double tilt;
            BHG.Polyline pline = new BHG.Polyline { ControlPoints = BH.Engine.Geometry.Query.IControlPoints(buildingElementGeometry.ICurve()) };

            tilt = Tilt(pline);
            return tilt;
        }

        /***************************************************/

        public static double Tilt(this BHG.Polyline pline)
        {
            double tilt;

            List<BHG.Point> pts = BH.Engine.Geometry.Query.DiscontinuityPoints(pline);

            BHG.Plane plane = BH.Engine.Geometry.Create.Plane(pts[0], pts[1], pts[2]);

            //The polyline can be locally concave. Check if the polyline is clockwise.
            if (!BH.Engine.Geometry.Query.IsClockwise(pline, plane.Normal))
                plane.Normal = -plane.Normal;

            tilt = BH.Engine.Geometry.Query.Angle(plane.Normal, BHG.Plane.XY.Normal) * (180 / Math.PI);

            return tilt;
        }

        /***************************************************/
    }
}
