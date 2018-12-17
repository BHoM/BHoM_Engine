using System;
using System.Collections.Generic;
using BHG = BH.oM.Geometry;
using BHEI = BH.oM.Environment.Interface;
using BHE = BH.oM.Environment;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double Tilt(this BHEI.IBuildingObject buildingElementGeometry)
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

            if (pts.Count < 3 || !BH.Engine.Geometry.Query.IsClosed(pline)) return -1; //Error protection on pts having less than 3 elements to create a plane or pLine not being closed

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
