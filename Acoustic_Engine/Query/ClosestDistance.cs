using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.Engine.Geometry;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        public static double ClosestDist(this List<Point> ptsA, List<Point> ptsB)
        {
            double closestDist = Double.PositiveInfinity;
            for (int i = 0; i < ptsB.Count; i++)
            {
                double dist = ptsA.GetClosestPoint(ptsB[i]).GetDistance(ptsB[i]);
                if (dist == Tolerance.Distance) { return dist; }
                closestDist = dist < closestDist ? dist : closestDist;
            }
            return closestDist;
        }
    }
}
