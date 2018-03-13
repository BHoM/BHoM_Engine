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

        public static double Orientation(BHEI.IBuildingElementGeometry bHoMPanel)
        {

            float orientation;
            BHE.Elements.BuildingElementPanel panel = bHoMPanel as BHE.Elements.BuildingElementPanel;
            BHG.Polyline pline = new BHG.Polyline { ControlPoints = BH.Engine.Geometry.Query.IControlPoints(panel.PolyCurve) };


            List<BHG.Point> pts = BH.Engine.Geometry.Query.DiscontinuityPoints(pline);

            BHG.Plane plane = BH.Engine.Geometry.Create.Plane(pts[0], pts[1], pts[2]);

            BHG.Vector xyNormal = BH.Engine.Geometry.Create.Vector(0, 1, 0);
            orientation = (float)(BH.Engine.Geometry.Query.Angle(plane.Normal, xyNormal) * (180 / Math.PI));


            return orientation;
        }

        /***************************************************/
    }


}
