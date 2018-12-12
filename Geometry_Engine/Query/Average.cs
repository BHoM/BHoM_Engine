using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Point Average(this IEnumerable<Point> points)
        {
            int count = points.Count();
            if (count < 1)
                return null;

            Point mean = new Point { X = 0, Y = 0, Z = 0 };

            foreach (Point pt in points)
                mean += pt;

            return mean /= count;
        }

        /***************************************************/

        public static Vector Average(this List<Vector> vs)
        {
            int count = vs.Count();
            if (count < 1)
                return null;

            Vector mean = new Vector { X = 0, Y = 0, Z = 0 };

            foreach (Vector v in vs)
                mean += v;

            return mean /= count;
        }

        /***************************************************/
    }
}
