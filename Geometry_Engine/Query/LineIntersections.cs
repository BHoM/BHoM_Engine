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
            if (useInfiniteLines)
            {
                l1.Infinite = true;
                l2.Infinite = true;
            }

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

        public static List<Point> LineIntersections(this Polyline curve, Line line, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
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