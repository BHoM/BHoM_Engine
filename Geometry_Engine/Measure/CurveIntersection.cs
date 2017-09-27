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

        public static Point GetIntersection(this Line line1, Line line2, bool useInfiniteLines = false, double tolerance = Tolerance.Distance)
        {
            Point pt1 = line1.Start;
            Point pt2 = line2.Start;
            Vector dir1 = (line1.End - pt1).GetNormalised();
            Vector dir2 = (line2.End - pt2).GetNormalised();
            Vector dir3 = pt2 - pt1;

            Vector cross = Measure.GetCrossProduct(dir1, dir2);

            if (Math.Abs(dir3.GetDotProduct(cross)) > Tolerance.Distance)
                return null; // Lines are not coplanar

            // Test for parallel lines
            if (cross.X < tolerance && cross.X > -tolerance && cross.Y < tolerance && cross.Y > -tolerance && cross.Z < tolerance && cross.Z > -tolerance)
            {
                if (useInfiniteLines || line1.Infinite || line2.Infinite)
                    return null;
                else if (pt1 == pt2 || pt1 == line2.End)
                    return pt1;
                else if (pt2 == line1.End || line2.End == line1.End)
                    return line1.End;
                else
                    return null;
            }

            double t = Measure.GetDotProduct(Measure.GetCrossProduct(dir3, dir2), cross) / Measure.GetSquareLength(cross);

            if (useInfiniteLines)  //TODO: Need to handle the cases where one of the line is Infinite as well
                return pt1 + t * dir1;
            else if (t > -tolerance && t < Measure.GetLength(dir1) + tolerance)
                return pt1 + t * dir1;
            else return null;
        }

        /***************************************************/

        public static List<Point> GetIntersections(this List<Line> lines, bool useInfiniteLine = true, double tolerance = Tolerance.Distance)
        {
            // Get the bounding boxes
            List<BoundingBox> boxes = lines.Select(x => x.GetBounds()).ToList();

            // Get the intersections
            List<Point> intersections = new List<Point>();
            for (int i = lines.Count - 1; i >= 0; i--)     // We should use an octoTree/point matrix instead of using bounding boxes
            {
                for (int j = lines.Count - 1; j > i; j--)
                {
                    if (Verify.IsInRange(boxes[i], boxes[j]))
                    {
                        Point result = GetIntersection(lines[i], lines[j], useInfiniteLine, tolerance);
                        if (result != null)
                            intersections.Add(result);
                    }
                }
            }

            return intersections;
        }

        /***************************************************/

        public static List<Point> GetIntersections(this ICurve curve1, ICurve curve2, double tolerance = Tolerance.Distance)
        {
            return _GetIntersections(curve1 as dynamic, curve2 as dynamic, tolerance);
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        private static List<Point> _GetIntersections(this ICurve curve1, ICurve curve2, double tolerance = Tolerance.Distance)
        {
            //List<Point> result = new List<Point>();
            //if (BoundingBox.InRange(c1.Bounds(), c2.Bounds()))
            //{
            //    t1 = new List<double>();
            //    t2 = new List<double>();
            //    tolerance = tolerance * tolerance;
            //    double[] p11 = c1.ControlPoint(0);
            //    double knot1 = 0;
            //    for (int i = c1.Degree; i < c1.Knots.Length; i++)
            //    {
            //        if (c1.Knots[i] == knot1) continue;
            //        knot1 = c1.Knots[i];

            //        double[] p12 = c1.PointAt(knot1);
            //        double[] p21 = c2.ControlPoint(0);
            //        double[] v1 = ArrayUtils.Sub(p12, p11);
            //        double knot2 = 0;
            //        for (int j = c2.Degree; j < c2.Knots.Length; j++)
            //        {
            //            if (c2.Knots[j] == knot2) continue;
            //            knot2 = c2.Knots[j];

            //            double[] p22 = c2.PointAt(knot2);
            //            double[] v2 = ArrayUtils.Sub(p22, p21);
            //            double[] intersectPoint = ArrayUtils.Intersect(p11, v1, p21, v2, true);

            //            if (intersectPoint != null)
            //            {

            //                double maxT1 = c1.Knots[i];
            //                double minT1 = c1.Knots[i - c1.Degree];

            //                double maxT2 = c2.Knots[j];
            //                double minT2 = c2.Knots[j - c2.Degree];

            //                if (c1.Degree > 1 || c2.Degree > 1)
            //                {

            //                    double[] p1Max = c1.UnsafePointAt(maxT1);
            //                    double[] p1Min = c1.UnsafePointAt(minT1);

            //                    double[] p2Max = c2.UnsafePointAt(maxT2);
            //                    double[] p2Min = c2.UnsafePointAt(minT2);

            //                    int interations = 0;
            //                    double d1 = double.MaxValue;
            //                    double d2 = double.MaxValue;
            //                    while (interations++ < 10)
            //                    {
            //                        if (d1 > tolerance)
            //                        {
            //                            d1 = UpdateNearestEnd(c1, intersectPoint, ref minT1, ref maxT1, ref p1Min, ref p1Max);
            //                            v1 = ArrayUtils.Sub(p1Max, p1Min);
            //                        }
            //                        if (d2 > tolerance)
            //                        {
            //                            d2 = UpdateNearestEnd(c2, intersectPoint, ref minT2, ref maxT2, ref p2Min, ref p2Max);
            //                            v2 = ArrayUtils.Sub(p2Max, p2Min);
            //                        }
            //                        intersectPoint = ArrayUtils.Intersect(p1Min, v1, p2Min, v2, false);

            //                        if (d1 < tolerance && d2 < tolerance) break;
            //                    }

            //                    t1.Add((minT1 + maxT1) / 2);
            //                    t2.Add((minT2 + maxT2) / 2);
            //                }
            //                else
            //                {
            //                    t1.Add(minT1 + (maxT1 - minT1) * ArrayUtils.Length(ArrayUtils.Sub(intersectPoint, p11)) / ArrayUtils.Length(v1));
            //                    t1.Add(minT2 + (maxT2 - minT2) * ArrayUtils.Length(ArrayUtils.Sub(intersectPoint, p21)) / ArrayUtils.Length(v2));
            //                }

            //                result.Add(new Point(intersectPoint));
            //            }
            //            p21 = p22;
            //        }
            //        p11 = p12;
            //    }
            //}
            //else
            //{
            //    t1 = null;
            //    t2 = null;
            //}
            //return result;

            throw new NotImplementedException();
        }

        /***************************************************/

        private static List<Point> _GetIntersections(this Line curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            return new List<Point> { curve1.GetIntersection(curve2, false, tolerance) };
        }

        /***************************************************/

        private static List<Point> _GetIntersections(this Line curve1, Polyline curve2, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static List<Point> _GetIntersections(this Polyline curve1, Line curve2, double tolerance = Tolerance.Distance)
        {
            return _GetIntersections(curve2, curve1, tolerance);
        }
    }
}