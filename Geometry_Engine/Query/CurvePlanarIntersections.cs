using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Point> CurvePlanarIntersections(this Arc curve1, Arc curve2, double tolerance = Tolerance.Distance)
        {
            Point c1 = curve1.Centre();
            Point c2 = curve2.Centre();
            double r1 = curve1.Radius();
            double r2 = curve2.Radius();
            Plane p1 = curve1.FitPlane();
            Plane p2 = curve2.FitPlane();
            Circle circle1 = new Circle { Centre = c1, Normal = p1.Normal, Radius = r1 };
            Circle circle2 = new Circle { Centre = c2, Normal = p2.Normal, Radius = r2 };

            List<Point> iPts = circle1.CurvePlanarIntersections(circle2, tolerance);

            Point midPoint1 = curve1.PointAtParameter(0.5);
            Point midPoint2 = curve2.PointAtParameter(0.5);
            double dist1 = midPoint1.Distance(curve1.StartPoint());
            double dist2 = midPoint2.Distance(curve2.StartPoint());

            for (int i = iPts.Count - 1; i >= 0; i--)
            {
                if (midPoint1.Distance(iPts[i]) - dist1 > tolerance || midPoint2.Distance(iPts[i]) - dist2 > tolerance) iPts.RemoveAt(i);
            }
            return iPts;
        }

        /***************************************************/

        public static List<Point> CurvePlanarIntersections(this Arc curve1, Circle curve2, double tolerance = Tolerance.Distance)
        {
            Point c1 = curve1.Centre();
            double r1 = curve1.Radius();
            Plane p1 = curve1.FitPlane();
            Circle circle1 = new Circle { Centre = c1, Normal = p1.Normal, Radius = r1 };

            List<Point> iPts = circle1.CurvePlanarIntersections(curve2, tolerance);

            Point midPoint1 = curve1.PointAtParameter(0.5);
            double dist = midPoint1.Distance(curve1.StartPoint());

            for (int i = iPts.Count - 1; i >= 0; i--)
            {
                if (midPoint1.Distance(iPts[i]) - dist > tolerance) iPts.RemoveAt(i);
            }
            return iPts;
        }

        /***************************************************/

        public static List<Point> CurvePlanarIntersections(this Circle curve1, Arc curve2, double tolerance = Tolerance.Distance)
        {
            return curve2.CurvePlanarIntersections(curve1, tolerance);
        }

        /***************************************************/

        public static List<Point> CurvePlanarIntersections(this Circle curve1, Circle curve2, double tolerance = Tolerance.Distance)
        {
            List<Point> iPts = new List<Point>();
            Point c1 = curve1.Centre;
            Point c2 = curve2.Centre;
            double r1 = curve1.Radius;
            double r2 = curve2.Radius;
            Vector n1 = curve1.Normal;
            Vector n2 = curve2.Normal;
            Plane p1 = new Plane { Origin = c1, Normal = n1 };
            Plane p2 = new Plane { Origin = c2, Normal = n2 };
            if (!p1.IsCoplanar(p2)) return iPts;

            double sqrDist = c1.SquareDistance(c2);
            double dist = Math.Sqrt(sqrDist);
            if (dist <= tolerance) return iPts;

            double sumRadii = r1 + r2;
            double difRadii = Math.Abs(r1 - r2);
            Vector dir = (c2 - c1).Normalise();

            if (Math.Abs(dist - sumRadii) <= tolerance) iPts.Add(c1 + dir * r1);
            else if (Math.Abs(dist - difRadii) <= tolerance) iPts.Add(c1 - dir * r1);
            else if (dist - sumRadii < tolerance && dist - difRadii >= tolerance)
            {
                double a = (r1 * r1 - r2 * r2 + sqrDist) / (2 * dist);
                Point midPt = (c1 + dir * a);
                Vector perp = dir.Rotate(Math.PI * 0.5, n1);
                double shift = Math.Sqrt(r1 * r1 - midPt.SquareDistance(c1));
                iPts.Add(midPt + perp * shift);
                iPts.Add(midPt - perp * shift);
            }
            return iPts;
        }

        /***************************************************/

        public static List<Point> CurvePlanarIntersections(this Line curve1, Arc curve2, double tolerance = Tolerance.Distance)
        {
            return curve2.LineIntersections(curve1, false, tolerance);
        }

        /***************************************************/

        public static List<Point> CurvePlanarIntersections(this Arc curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            return curve1.LineIntersections(curve2, false, tolerance);
        }

        /***************************************************/

        public static List<Point> CurvePlanarIntersections(this Circle curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            return curve1.LineIntersections(curve2, false, tolerance);
        }

        /***************************************************/

        public static List<Point> CurvePlanarIntersections(this Line curve1, Circle curve2, double tolerance = Tolerance.Distance)
        {
            return curve2.LineIntersections(curve1, false, tolerance);
        }

        /***************************************************/

        public static List<Point> CurvePlanarIntersections(this Line curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            return curve1.LineIntersections(curve2, false, tolerance);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<Point> ICurvePlanarIntersections(this ICurve curve1, ICurve curve2, double tolerance = Tolerance.Distance)
        {
            List<Point> result = new List<Point>();
            foreach (ICurve c1 in curve1.ISubParts())
            {
                foreach (ICurve c2 in curve2.ISubParts())
                {
                    result.AddRange(CurvePlanarIntersections(c1 as dynamic, c2 as dynamic, tolerance));
                }
            }
            return result;
        }

        /***************************************************/
    }
}