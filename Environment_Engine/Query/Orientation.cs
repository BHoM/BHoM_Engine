using System;
using System.Collections.Generic;
using System.Linq;
using BH.oM.Environment.Elements;
using BH.Engine.Geometry;
using BH.oM.Environment.Interface;
using BHG = BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double Orientation(IBuildingObject buildingPanel)
        {
            Panel panel = buildingPanel as Panel;
            BHG.Polyline pLine = new BHG.Polyline { ControlPoints = panel.PanelCurve.IControlPoints() };

            List<BHG.Point> pts = pLine.DiscontinuityPoints();
            BHG.Plane plane = BH.Engine.Geometry.Create.Plane(pts[0], pts[1], pts[2]); //Some protection on this needed maybe?

            BHG.Vector xyNormal = BH.Engine.Geometry.Create.Vector(0, 1, 0);

            return BH.Engine.Geometry.Query.Angle(plane.Normal, xyNormal) * (180 / Math.PI);
        }
    }
}