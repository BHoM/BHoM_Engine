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

        public static double IInclination(this BHEI.IBuildingElementGeometry buildingElementGeometry)
        {
            return Inclination(buildingElementGeometry as dynamic);
        }

        /***************************************************/

        public static double Inclination(this BHE.Elements.BuildingElementPanel panel)
        {
            double inclination;
          
            BHG.Polyline pline = new BHG.Polyline { ControlPoints = BH.Engine.Geometry.Query.IControlPoints(panel.PolyCurve) };

            List<BHG.Point> pts = BH.Engine.Geometry.Query.DiscontinuityPoints(pline);
            BHG.Plane plane = BH.Engine.Geometry.Create.Plane(pts[0], pts[1], pts[2]);

            inclination = BH.Engine.Geometry.Query.Angle(plane.Normal, BHG.Plane.XY.Normal) * (180 / Math.PI);

            return inclination;
        }

        /***************************************************/
    }
}
