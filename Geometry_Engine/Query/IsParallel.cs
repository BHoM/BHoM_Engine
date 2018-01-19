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
            double dp = v1.DotProduct(v2);
            return 1 - Math.Abs(dp) <= tolerance ? dp > 0 ? 1 : -1 : 0;
        }

        /***************************************************/

        public static int IsParallel(this Line line1, Line line2, double tolerance = Tolerance.Angle)
        {
            return line1.Direction().IsParallel(line2.Direction());
        }

        /***************************************************/
    }
}
