using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Transform
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Point> GetDivided(this ICurve curve, double length)
        {
            List<Point> points = null;
            double dist = 0;
            while (length <= curve.GetLength())
            {
                points.Add(curve.GetPointAtLength(dist));
                dist += length;
            }
            return points;
        }

        /***************************************************/

        public static List<Point> GetDivided(this ICurve curve, int number)
        {
            List<Point> points = null;
            double iter = curve.GetLength() / number;
            for (double i = 0; i < 1; i += iter)
            {
                points.Add(curve.GetPointAtLength(i));
            }
            return points;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        /***************************************************/
    }
}
