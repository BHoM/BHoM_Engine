using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static double ClosestDist(this IEnumerable<Point> ptsA, IEnumerable<Point> ptsB)
        {
            double closestDist = Double.PositiveInfinity;
            foreach (Point ptB in ptsB)
            {
                double dist = ptsA.GetClosestPoint(ptB).GetDistance(ptB);
                if (dist <= Tolerance.Distance) { return dist; }
                closestDist = dist < closestDist ? dist : closestDist;
            }
            return closestDist;
        }
    }
}
