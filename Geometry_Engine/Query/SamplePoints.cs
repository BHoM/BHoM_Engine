using System.Collections.Generic;
using BH.oM.Geometry;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Point> SamplePoints(this ICurve curve, double step)
        {
            if (step <= 0) { throw new ArgumentException("step value must be greater than 0"); }
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
            if (number <= 0) { throw new ArgumentException("number value must be greater than 0"); }
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
