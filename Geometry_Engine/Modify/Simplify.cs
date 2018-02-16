using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Polyline Simplify(this Polyline polyline)
        {
            polyline.ControlPoints = polyline.DiscontinuityPoints();
            return polyline;
        }
    }
}