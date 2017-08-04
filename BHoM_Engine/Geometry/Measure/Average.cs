using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Measure
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Point GetAverage(this List<Point> pts)
        {
            int count = pts.Count;
            if (count < 1) return null;
            Point mean = pts[0].GetClone() as Point;

            for (int i = 1; i < count; i++)
                mean += pts[i];

            return mean /= count;
        }

        /***************************************************/

        public static Vector GetAverage(this List<Vector> vs)
        {
            int count = vs.Count;
            if (count < 1) return null;
            Vector mean = vs[0].GetClone() as Vector;

            for (int i = 1; i < count; i++)
                mean += vs[i];

            return mean /= count;
        }
    }
}
