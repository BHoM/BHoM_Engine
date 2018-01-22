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

        public static Point LineIntersection(this Line line1, Line line2, bool useInfiniteLines = false, double tolerance = Tolerance.Distance)
        {
            Line l1 = line1.Clone();
            Line l2 = line2.Clone();

            l1.Infinite = useInfiniteLines ? true : line1.Infinite;
            l2.Infinite = useInfiniteLines ? true : line2.Infinite;

            Point p1 = l1.Start;
            Point p2 = l2.Start;
            Vector v1 = l1.End - p1;
            Vector v2 = l2.End - p2;

            double[,] e = new double[3, 3]
            {
                {v1.X, -v2.X, p2.X-p1.X},
                {v1.Y, -v2.Y, p2.Y-p1.Y},
                {v1.Z, -v2.Z, p2.Z-p1.Z},
            };

            double[,] eref = e.RowEchelonForm(false);
            int nonZero = eref.CountNonZeroRows();

            switch (nonZero)
            {
                case 3:                                                                     // nonplanar
                    return null;
                case 2:
                    if (eref[1, 1] <= tolerance) return null;                               // parallel, not colinear
                    else                                                                    // coplanar
                    {
                        double t2 = eref[1, 2];
                        double t1 = eref[0, 2] - t2 * eref[0, 1];
                        bool i1 = l1.Infinite ? true : t1 >= 0 && t1 <= 1 ? true : false;
                        bool i2 = l2.Infinite ? true : t2 >= 0 && t2 <= 1 ? true : false;
                        if (i1 && i2)
                        {
                            return p1 + t1 * v1;
                        }
                        return null;
                    }
                case 1:                                                                     // colinear
                    if (l1.Infinite || l2.Infinite) return null;
                    double sqrTol = tolerance * tolerance;
                    if (p1.SquareDistance(p2) <= sqrTol || p1.SquareDistance(l2.End) <= sqrTol) return p1;
                    else if (l1.End.SquareDistance(p2) <= sqrTol || l1.End.SquareDistance(l2.End) <= sqrTol) return l1.End;
                    else return null;
            }
            return null;
        }

        /***************************************************/

        public static List<Point> LineIntersections(this Arc arc, Line line, double tolerance = Tolerance.Distance)
        {
            List<Point> iPts = new List<Point>();
            Point midPoint = arc.PointAtParameter(0.5);

            Point center = arc.Centre();
            Plane p = arc.ControlPoints().FitPlane();
            double sqrRadius = center.SquareDistance(arc.Start);

            if (Math.Abs(p.Normal.DotProduct(line.Direction())) > Tolerance.Angle)
            {
                Point pt = line.PlaneIntersection(p);
                if (pt != null && Math.Abs(pt.SquareDistance(center) - sqrRadius) <= tolerance) iPts.Add(pt);
            }
            else
            {
                Point pt = line.ClosestPoint(center, true);
                double sqrDiff = sqrRadius - pt.SquareDistance(center);
                if (Math.Abs(sqrDiff) <= tolerance) iPts.Add(pt);
                else if (sqrDiff > 0)
                {
                    Vector v = pt - center;
                    double o = Math.Sqrt(sqrDiff);
                    Vector vo = v.CrossProduct(p.Normal).Normalise() * o;
                    iPts.Add(pt + vo);
                    iPts.Add(pt - vo);
                }
            }

            List<Point> output = new List<Point>();
            double sqrd = midPoint.SquareDistance(arc.Start);
            {
                foreach (Point pt in iPts)
                {
                    if ((line.Infinite || pt.Distance(line) <= tolerance) && midPoint.SquareDistance(pt) <= sqrd) output.Add(pt);
                }
            }
            return output;
        }

        /***************************************************/

        public static List<Point> LineIntersections(this Circle circle, Line line, double tolerance = Tolerance.Distance)
        {
            List<Point> iPts = new List<Point>();

            Plane p = new Plane { Origin = circle.Centre, Normal = circle.Normal };
            if (Math.Abs(circle.Normal.DotProduct(line.Direction())) > Tolerance.Angle)
            {
                Point pt = line.PlaneIntersection(p);
                if (pt!=null && Math.Abs(pt.SquareDistance(circle.Centre) - circle.Radius * circle.Radius) <= tolerance) iPts.Add(pt);
            }
            else
            {
                Point pt = line.ClosestPoint(circle.Centre, true);
                double sqrDiff = circle.Radius * circle.Radius - pt.SquareDistance(circle.Centre);
                if (Math.Abs(sqrDiff) <= tolerance) iPts.Add(pt);
                else if (sqrDiff > 0)
                {
                    Vector v = pt - circle.Centre;
                    double o = Math.Sqrt(sqrDiff);
                    Vector vo = v.CrossProduct(circle.Normal).Normalise() * o;
                    iPts.Add(pt + vo);
                    iPts.Add(pt - vo);
                }
            }

            List<Point> output = new List<Point>();
            if (line.Infinite) return iPts;
            else
            {
                foreach (Point pt in iPts)
                {
                    if (pt.Distance(line) <= tolerance) output.Add(pt);
                }
            }
            return output;
        }

        /***************************************************/

        public static List<Point> LineIntersections(this Polyline curve, Line line, bool useInfiniteLine = false, double tolerance = Tolerance.Distance)
        {
            Line l = line.Clone();
            l.Infinite = useInfiniteLine ? true : line.Infinite;

            List<Point> iPts = new List<Point>();
            foreach (Line ln in curve.SubParts())
            {
                Point pt = ln.LineIntersection(l);
                if (pt != null) iPts.Add(pt);
            }

            return iPts;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<Point> ILineIntersections(this ICurve curve1, Line line, double tolerance = Tolerance.Distance)
        {
            return LineIntersections(curve1 as dynamic, line, tolerance);
        }

        /***************************************************/
    }
}