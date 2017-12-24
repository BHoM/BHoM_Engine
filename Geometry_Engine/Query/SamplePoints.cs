using System.Collections.Generic;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Point> SamplePoints(this ICurve curve, double step)
        {
            List<Point> points = new List<Point>();
            double dist = 0;
            while (dist <= curve.ILength())
            {
                points.Add(curve.IPointAtLength(dist));
                dist += step;
            }
            return points;
        }

        /***************************************************/

        public static List<Point> SamplePoints(this ICurve curve, int number)
        {
            List<Point> points = new List<Point>();
            double iter = curve.ILength() / number;
            for (double i = 0; i < 1; i += iter)
            {
                points.Add(curve.IPointAtLength(i));
            }
            return points;
        }

        /***************************************************/
    }
}
