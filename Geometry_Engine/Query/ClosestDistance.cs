using BH.oM.Geometry;
using System;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static double ClosestDistance(this IEnumerable<Point> ptsA, IEnumerable<Point> ptsB, double tolerance = Tolerance.Distance)
        {
            double sqTol = tolerance * tolerance;
            double closestDist = Double.PositiveInfinity;

            foreach (Point ptB in ptsB)
            {
                double sqDist = ptsA.ClosestPoint(ptB).SquareDistance(ptB);
                if (sqDist <= sqTol)
                    return Math.Sqrt(sqDist);

                closestDist = sqDist < closestDist ? sqDist : closestDist;
            }

            return Math.Sqrt(closestDist);
        }

        /***************************************************/
    }
}
