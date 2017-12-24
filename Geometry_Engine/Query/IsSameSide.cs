﻿using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsSameSide(this Point p1, Plane plane, Point p2, double tolerance = Tolerance.Distance)
        {
            double d1 = plane.Normal.DotProduct(p1 - plane.Origin);
            double d2 = plane.Normal.DotProduct(p2 - plane.Origin);

            return ((d1 < -tolerance && d2 < -tolerance) || (d1 > tolerance && d2 > tolerance));
        }

        /***************************************************/

        public static bool IsSameSide(this IList<Point> points, Plane plane, double tolerance = Tolerance.Distance)
        {
            if (points.Count() < 2) return true;

            double d1 = plane.Normal.DotProduct(points[0] - plane.Origin);
            if (d1 >= -tolerance && d1 <= tolerance)
                return false;

            for (int i = 1; i < points.Count; i++)
            {
                double d = plane.Normal.DotProduct(points[i] - plane.Origin);
                if (d * d1 < 0 || (d >= -tolerance && d <= tolerance))
                    return false;
            }

            return true;
        }

        /***************************************************/
    }
}
