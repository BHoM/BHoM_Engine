using BH.oM.Geometry;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static int IsParallel(this Vector v1, Vector v2, double angleTolerance = Tolerance.Angle)
        {
            Vector v1N = v1.Normalise();
            Vector v2N = v2.Normalise();
            double dotTolerance = Math.Cos(angleTolerance);
            double dp = v1N.DotProduct(v2N);
            return Math.Abs(dp) >= dotTolerance ? dp > 0 ? 1 : -1 : 0;
        }

        /***************************************************/

        public static int IsParallel(this Line line1, Line line2, double angleTolerance = Tolerance.Angle)
        {
            return line1.Direction().IsParallel(line2.Direction(), angleTolerance);
        }

        /***************************************************/
    }
}
