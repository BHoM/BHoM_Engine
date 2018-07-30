using System;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [NotImplemented]
        private static Point[] LineProximity(this Line line1, Line line2, bool useInfiniteLines = false, double tolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double[] SkewLineProximity(this Line line1, Line line2, double angleTolerance = Tolerance.Angle)
        {
            Vector v1 = line1.End - line1.Start;
            Vector v2 = line2.End - line2.Start;
            Vector v1N = v1.Normalise();
            Vector v2N = v2.Normalise();
            if (v1N == null || v2N == null || 1 - Math.Abs(v1N.DotProduct(v2N)) <= angleTolerance) return null;

            Point p1 = line1.Start;
            Point p2 = line2.Start;

            Vector cp = v1.CrossProduct(v2);
            Vector n1 = v1.CrossProduct(-cp);
            Vector n2 = v2.CrossProduct(cp);

            double t1 = (p2 - p1) * n2 / (v1 * n2);
            double t2 = (p1 - p2) * n1 / (v2 * n1);

            return new double[] { t1, t2 };
        }
    }
}
