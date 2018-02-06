using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** public Methods - Vectors                  ****/
        /***************************************************/

        public static List<Point> CullDuplicates(this List<Point> points, double maxDist = Tolerance.Distance)
        {
            List<List<Point>> clusteredPoints = points.PointClusters(maxDist);
            return clusteredPoints.Select(x => x.Average()).ToList();
        }
    }
}
