using BH.oM.Geometry;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Polyline Simplify(this Polyline polyline)
        {
            List<Point> ctrlPts = polyline.ControlPoints;
            return new Polyline { ControlPoints = ctrlPts };
        }
    }
}