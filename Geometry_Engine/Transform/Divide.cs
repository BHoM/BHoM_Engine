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
            List<Point> points = new List<Point>();
            double dist = 0;
            while (dist <= curve._GetLength())
            {
                points.Add(curve._GetPointAtLength(dist));
                dist += length;
            }
            return points;
        }

        /***************************************************/

        public static List<Point> GetDivided(this ICurve curve, int number)
        {
            List<Point> points = new List<Point>();
            double iter = curve._GetLength() / number;
            for (double i = 0; i < 1; i += iter)
            {
                points.Add(curve._GetPointAtLength(i));
            }
            return points;
        }

    }
}
