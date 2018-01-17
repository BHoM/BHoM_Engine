using BH.oM.Geometry;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static int IsParallel(this Vector v1, Vector v2, double tolerance = Tolerance.Angle)
        {
            double angle = v1.Angle(v2);

            return angle > Math.PI / 2 ? Math.PI - angle <= tolerance ? -1 : 0 : angle <= tolerance ? 1 : 0;
        }

        /***************************************************/

        public static int IsParallel(this Line l1, Line l2, double tolerance = Tolerance.Angle)
        {
            Vector v1 = l1.Direction();
            Vector v2 = l2.Direction();
            double angle = v1.Angle(v2);

            return angle > Math.PI / 2 ? Math.PI - angle <= tolerance ? -1 : 0 : angle <= tolerance ? 1 : 0;
        }

        /***************************************************/
    }
}
