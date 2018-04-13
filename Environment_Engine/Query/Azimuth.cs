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

        public static double Azimuth(this BHEI.IBuildingElementGeometry buildingElementGeometry, BHG.Vector refVector)
        {
            BHG.Polyline pline = new BHG.Polyline { ControlPoints = BH.Engine.Geometry.Query.IControlPoints(buildingElementGeometry.ICurve()) };

            double azimuth = Azimuth(pline, refVector);
            return azimuth;
        }

        /***************************************************/

        public static double Azimuth(this BHG.Polyline pline, BHG.Vector refVector)
        {
            double azimuth;

            List<BHG.Point> pts = BH.Engine.Geometry.Query.DiscontinuityPoints(pline);
            BHG.Plane plane = BH.Engine.Geometry.Create.Plane(pts[0], pts[1], pts[2]);

            if (Geometry.Modify.Normalise(plane.Normal).Z == 1)
                azimuth = 0;
            else if (Geometry.Modify.Normalise(plane.Normal).Z == -1)
                azimuth = 180;
            else
            {
                BHG.Vector v1 = Geometry.Modify.Project(plane.Normal, BHG.Plane.XY);
                BHG.Vector v2 = (Geometry.Modify.Project(refVector, BHG.Plane.XY));
               
                azimuth = (BH.Engine.Geometry.Query.SignedAngle(v1, v2, BHG.Vector.ZAxis) * (180 / Math.PI));
                if (azimuth < 0)
                    azimuth = 360 + azimuth;

            }
            return azimuth;
        }

        /***************************************************/
    }


}
