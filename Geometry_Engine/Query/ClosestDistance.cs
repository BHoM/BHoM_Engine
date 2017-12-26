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

        public static double ClosestDistance(this IEnumerable<Point> ptsA, IEnumerable<Point> ptsB)
        {
            double closestDist = Double.PositiveInfinity;
            foreach (Point ptB in ptsB)
            {
                double dist = ptsA.ClosestPoint(ptB).Distance(ptB);
                if (dist <= Tolerance.Distance) { return dist; }
                closestDist = dist < closestDist ? dist : closestDist;
            }
            return closestDist;
        }

        /***************************************************/
    }
}
