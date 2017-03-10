using BHoM.Base;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHoM.Geometry
{


    public static class XCurve
    {
        public static void CreateNurbForm(this Curve curve)
        {
            switch (curve.GeometryType)
            {
                case GeometryType.Arc:
                    XArc.CreateNurbForm(curve as Arc);
                    break;
                case GeometryType.Line:
                    XLine.CreateNurbForm(curve as Line);
                    break;
                case GeometryType.PolyCurve:
                    XPolyCurve.CreateNurbFrom(curve as PolyCurve);
                    break;
                case BHoM.Geometry.GeometryType.Polyline:
                    XPolyline.CreateNurbForm(curve as Polyline);
                    break;
                case GeometryType.Circle:
                    XCircle.CreateNurbForm(curve as Circle);
                    break;
            }
        }

        internal static double[] ControlPoint(this Curve curve, int i)
        {
            return i < (int)(curve.ControlPointVector.Length / (curve.Dimensions + 1)) ?
                new Point(Utils.SubArray<double>(curve.ControlPointVector, i * (curve.Dimensions + 1), curve.Dimensions)) : null;
        }

        public static bool ContainsCurve(this Curve curve, Curve other)
        {
            Plane p = null;
            if (!curve.IsNurbForm) curve.CreateNurbForm();
            if (curve.IsClosed() && curve.TryGetPlane(out p))
            {
                if (p.InPlane(other.ControlPointVector, other.Dimensions + 1, 0.001))
                {
                    for (int i = 0; i < other.ControlPointVector.Length; i += other.Dimensions + 1)
                    {
                        double[] pointArray = Utils.SubArray(other.ControlPointVector, i, other.Dimensions + 1);
                        double[] direction = ArrayUtils.Add(ArrayUtils.Sub(pointArray, p.Origin), pointArray);
                        double[] up = ArrayUtils.Add(pointArray, p.Normal);
                        Plane cuttingPlane = Create.PlaneFrom3Points(Utils.Merge(pointArray, direction, up), curve.Dimensions + 1);
                        Point point = new Point(pointArray);
                        List<Point> intersects = Intersect.PlaneCurve(cuttingPlane, curve, 0.0001);

                        if (intersects.Count == 1) return false;

                        intersects.Add(point);
                        //intersects = Point.RemoveDuplicates(intersects, 3);

                        intersects.Sort(delegate (Point p1, Point p2)
                        {
                            return ArrayUtils.DotProduct(p1, direction).CompareTo(ArrayUtils.DotProduct(p2, direction));
                        });

                        for (int j = 0; j < intersects.Count; j++)
                        {
                            if (j % 2 == 0 && intersects[j] == point) return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public static bool ContainsPoints(this Curve curve, List<Point> points)
        {
            Plane p = null;
            if (!curve.IsNurbForm) curve.CreateNurbForm();
            if (curve.IsClosed() && curve.TryGetPlane(out p))
            {
                for (int i = 0; i < points.Count; i++)
                {
                    if (p.InPlane(points[i], 0.001))
                    {
                        Vector direction = points[i] - p.Origin;
                        Vector up = p.Normal;
                        Plane cuttingPlane = new Plane(points[i], points[i] + direction, points[i] + up);
                        List<Point> intersects = Intersect.PlaneCurve(cuttingPlane, curve, 0.001);
                        intersects.Add(points[i]);
                        intersects = PointUtils.RemoveDuplicates(intersects, 3);

                        intersects.Sort(delegate (Point p1, Point p2)
                        {
                            return ArrayUtils.DotProduct(p1, direction).CompareTo(ArrayUtils.DotProduct(p2, direction));
                        });

                        for (int j = 0; j < intersects.Count; j++)
                        {
                            if (j % 2 == 0 && intersects[j] == points[i]) return false;
                        }
                    }
                    else return false;
                }
                return true;
            }
            return false;
        }

        public static double Length(this Curve curve)
        {          
            double length = 0;
            for (int i = 0; i < curve.ControlPointVector.Length / (curve.Dimensions + 1) - (curve.Dimensions + 1); i++)
            {
                length += ArrayUtils.Length(ArrayUtils.Sub(curve.ControlPointVector, i, i + curve.Dimensions + 1, curve.Dimensions + 1));
            }
            return length;            
        }

        private static double BasisFunction(this Curve curve, int i, int n, double t)
        {
            if (n > 0 && t >= curve.Knots[i] && t < curve.Knots[i + n + 1])
            {
                double result = 0;
                if (curve.Knots[i + n] - curve.Knots[i] > 0)
                {
                    result += curve.BasisFunction(i, n - 1, t) * (t - curve.Knots[i]) / (curve.Knots[i + n] - curve.Knots[i]);
                }
                if (curve.Knots[i + n + 1] - curve.Knots[i + 1] > 0)
                {
                    result += curve.BasisFunction(i + 1, n - 1, t) * (curve.Knots[i + n + 1] - t) / (curve.Knots[i + n + 1] - curve.Knots[i + 1]);
                }

                return result;
            }
            else
            {
                return t >= curve.Knots[i] && t < curve.Knots[i + 1] ? 1 : 0;
            }
        }

        private static double DerivativeFunction(this Curve curve, int i, int n, double t)
        {
            if (n > 0)
            {
                double result = 0;
                if (i + n < curve.Knots.Length && curve.Knots[i + n] - curve.Knots[i] > 0)
                {
                    result += curve.BasisFunction(i, n - 1, t) * n / (curve.Knots[i + n] - curve.Knots[i]);
                }
                if (i + n + 1 < curve.Knots.Length && curve.Knots[i + n + 1] - curve.Knots[i + 1] > 0)
                {
                    result -= curve.BasisFunction(i + 1, n - 1, t) * n / (curve.Knots[i + n + 1] - curve.Knots[i + 1]);
                }
                return result;
            }

            return 0;
        }

        public static Point PointAt(this Curve curve, double t)
        {
            if (!curve.IsNurbForm) curve.CreateNurbForm();
            return new Point(curve.UnsafePointAt(t));
        }

        internal static double[] UnsafePointAt(this Curve curve, double t)
        {
            double[] sumNwP = new double[curve.Dimensions];
            double sumNw = 0;
            if (t == 0) return Utils.SubArray<double>(curve.ControlPointVector, 0, 3);
            else if (t >= curve.Knots[curve.Knots.Length - 1]) return Utils.SubArray<double>(curve.ControlPointVector, curve.ControlPointVector.Length - 4, 3);
            for (int i = 0; i < curve.ControlPointVector.Length / (curve.Dimensions + 1); i++)
            {
                double Nt = curve.BasisFunction(i, curve.Order - 1, t);
                if (Nt == 0) continue;
                sumNwP = ArrayUtils.Add(sumNwP, ArrayUtils.Multiply(curve.ControlPointVector, Nt * curve.Weights[i], i * (curve.Dimensions + 1), curve.Dimensions));
                sumNw += Nt * curve.Weights[i];
            }
            return ArrayUtils.Divide(sumNwP, sumNw);
        }

        public static Vector TangentAt(this Curve curve, double t)
        {
            if (!curve.IsNurbForm) curve.CreateNurbForm();
            double[] sumNwP = new double[curve.Dimensions];
            double[] sumNwPDer = new double[curve.Dimensions];
            double sumNw = 0;
            double sumNwDer = 0;

            for (int i = 0; i < curve.ControlPointVector.Length / (curve.Dimensions + 1); i++)
            {
                double Nt = curve.BasisFunction(i, curve.Degree, t);
                double Nder = curve.DerivativeFunction(i, curve.Degree, t);
                double[] P = Utils.SubArray<double>(curve.ControlPointVector, i * (curve.Dimensions + 1), curve.Dimensions);
                sumNwP = ArrayUtils.Add(sumNwP, ArrayUtils.Multiply(P, Nt * curve.Weights[i]));
                sumNwPDer = ArrayUtils.Add(sumNwPDer, ArrayUtils.Multiply(P, Nder * curve.Weights[i]));
                sumNw += Nt * curve.Weights[i];
                sumNwDer += Nder * curve.Weights[i];
            }

            double[] numerator = ArrayUtils.Sub(ArrayUtils.Multiply(sumNwPDer, sumNw), ArrayUtils.Multiply(sumNwP, sumNwDer));
            return new Vector(ArrayUtils.Divide(numerator, Math.Pow(sumNw, 2)));
        }

        public static List<Curve> Explode(this Curve curve)
        {
            return new List<Curve>() { curve };
        }

        public static double[] LeftControlPoints(this Curve curve, double t)
        {
            int index = 0;
            if (!curve.IsNurbForm) curve.CreateNurbForm();
            while (t >= curve.Knots[index]) { index++; }
            index -= curve.Degree;

            return Utils.SubArray<double>(curve.ControlPointVector, 0, index * (curve.Dimensions + 1));
        }

        public static double[] RightControlPoints(this Curve curve, double t)
        {
            int index = 0;
            if (!curve.IsNurbForm) curve.CreateNurbForm();
            while (t > curve.Knots[index]) { index++; }
            index--;

            if (index < curve.PointCount)
            {
                return Utils.SubArray<double>(curve.ControlPointVector, index * (curve.Dimensions + 1), (curve.PointCount - index) * (curve.Dimensions + 1));
            }
            else
            {
                return new double[0];
            }
        }

        public static void ChangeDegree(this Curve curve, int newDegree)
        {
            if (!curve.IsNurbForm) curve.CreateNurbForm();
            while (newDegree > curve.Degree)
            {
                int size = curve.Dimensions + 1;

                List<double> knots = new List<double>();
                List<double> newPoints = new List<double>();
                List<double> weights = new List<double>();
                knots.Add(curve.Knots[0]);
                for (int i = 1; i < curve.Knots.Length; i++)
                {
                    if (curve.Knots[i] != curve.Knots[i - 1])
                    {
                        knots.Add(curve.Knots[i - 1]);

                        double[] pnts = Utils.SubArray<double>(curve.ControlPointVector, (i - curve.Order) * size, size * curve.Order);
                        double[] weightResults = Utils.SubArray<double>(curve.Weights, i - curve.Order, curve.Order);

                        for (int j = 0; j < curve.Degree + 1; j++)
                        {
                            double lhs = (double)j / curve.Order;
                            double rhs = (1 - lhs);
                            double weight = lhs > 0 ? rhs > 0 ? lhs * weightResults[j - 1] + weightResults[j] * rhs : lhs * weightResults[j - 1] : weightResults[j] * rhs;
                            weights.Add(weight);
                            if (lhs > 0)
                            {
                                for (int k = 0; k < size; k++)
                                {
                                    newPoints.Add((lhs * pnts[(j - 1) * size + k] * weightResults[j - 1] + rhs * pnts[j * size + k] * weightResults[j]) / weight);
                                }
                            }
                            else
                            {
                                for (int k = 0; k < size; k++)
                                {
                                    newPoints.Add((rhs * pnts[j * size + k] * weightResults[j]) / weight);
                                }
                            }
                        }
                    }
                    knots.Add(curve.Knots[i]);
                }

                weights.Add(curve.Weights[curve.Weights.Length - 1]);
                newPoints.AddRange(Utils.SubArray<double>(curve.ControlPointVector, curve.ControlPointVector.Length - size, size));

                knots.Add(curve.Knots[curve.Knots.Length - 1]);
                curve.SetDegree(newDegree);
                curve.SetControlPoints(newPoints.ToArray());
                curve.SetWeights(weights.ToArray());
                curve.SetKnots(knots.ToArray());
            }
        }
        
        public static int InsertKnot(this Curve curve, double value)
        {
            if (!curve.IsNurbForm) curve.CreateNurbForm();
            int controlPointIndex = 0;
            double lowerKnot = 0;
            double upperKnot = 0;
            int size = curve.Dimensions + 1;
            int sameKnotCount = 0;
            for (int i = 0; i < curve.Knots.Length; i++)
            {
                if (curve.Knots[i] == value)
                {
                    sameKnotCount++;
                    if (sameKnotCount == curve.Degree)
                    {
                        return i - curve.Degree;

                    }
                }
                if (curve.Knots[i] > value)
                {
                    lowerKnot = curve.Knots[i - 1];
                    upperKnot = curve.Knots[i];

                    controlPointIndex = i - curve.Degree;
                    break;
                }
            }

            double[] points = new double[curve.Degree * (size)];
            double[] weightResults = new double[curve.Degree];

            int i1 = 0;
            int j1 = 0;
            for (int i = 0; i < curve.Degree; i++)
            {
                i1 = i + controlPointIndex;
                double t = (value - curve.Knots[i1]) / (curve.Knots[i1 + curve.Degree] - curve.Knots[i1]);
                weightResults[i] = curve.Weights[i1 - 1] * (1 - t) + t * curve.Weights[i1];
                for (int j = 0; j < size; j++)
                {
                    j1 = j + i1 * size;
                    points[j + i * size] = (curve.Weights[i1 - 1] * curve.ControlPointVector[j1 - size] * (1 - t) + t * curve.Weights[i1] * curve.ControlPointVector[j1]) / weightResults[i];
                }
            }

            double[] newControlPnts = Utils.Merge<double>(curve.LeftControlPoints(lowerKnot), points, curve.RightControlPoints(upperKnot));

            double[] newWeight = new double[curve.Weights.Length + 1];
            Array.Copy(curve.Weights, newWeight, controlPointIndex);
            Array.Copy(weightResults, 0, newWeight, controlPointIndex, curve.Degree);
            Array.Copy(curve.Weights, controlPointIndex + curve.Degree - 1, newWeight, controlPointIndex + curve.Degree, curve.Weights.Length - curve.Degree - controlPointIndex + 1);

            double[] knots = new double[curve.Knots.Length + 1];

            Array.Copy(curve.Knots, knots, controlPointIndex + curve.Degree);
            knots[controlPointIndex + curve.Degree] = value;
            Array.Copy(curve.Knots, controlPointIndex + curve.Degree, knots, controlPointIndex + curve.Degree + 1, curve.Knots.Length - controlPointIndex - curve.Degree);
            curve.SetKnots(knots);
            curve.SetControlPoints(newControlPnts);
            curve.SetWeights(newWeight);
            return controlPointIndex;
        }

        public static List<Curve> Split(this Curve curve, List<Plane> planes, bool keepClosed = false, double tolerance = 0.0001)
        {
            if (!curve.IsNurbForm) curve.CreateNurbForm();
            List<Curve> unSplit = new List<Curve>() { curve };
            List<Curve> split = null;

            for (int i = 0; i < planes.Count; i++)
            {
                split = new List<Curve>();
                for (int j = 0; j < unSplit.Count; j++)
                {
                    split.AddRange(unSplit[j].Split(planes[i], keepClosed, tolerance));
                }
                unSplit = split;
            }
            return split;
        }

        public static List<Curve> Split(this Curve curve, Plane p, bool keepClosed = false, double tolerance = 0.0001)
        {
            if (!curve.IsNurbForm) curve.CreateNurbForm();
            List<Curve> unsplit = new List<Curve>() { curve };
            if (p.IsSameSide(curve.ControlPointVector, tolerance)) return unsplit;

            List<double> curveParams = new List<double>();
            Intersect.PlaneCurve(p, curve, tolerance, out curveParams);

            List<Curve> split = curve.Split(curveParams);

            if (keepClosed && curve.IsClosed())
            {
                List<Curve> lhs = new List<Curve>();
                List<Curve> rhs = new List<Curve>();

                for (int i = 0; i < split.Count; i++)
                {
                    int[] side = p.GetSide(split[i].ControlPointVector, 0.001);
                    int counter = 0;
                    while (counter < side.Length && side[counter] == 0) counter++;
                    if (counter < side.Length)
                    {
                        if (side[counter] == -1) lhs.Add(split[i]);
                        else rhs.Add(split[i]);
                    }
                }

                split = CurveUtils.Join(lhs);
                split.AddRange(CurveUtils.Join(rhs));
                for (int i = 0; i < split.Count; i++)
                {
                    split[i].Append(new Line(split[i].EndPoint, split[i].StartPoint));
                }
            }

            return split;
        }

        public static List<Curve> Split(this Curve curve, List<double> t)
        {
            if (!curve.IsNurbForm) curve.CreateNurbForm();
            t.Sort();
            List<Curve> split = new List<Curve>();
            Curve unsplit = curve;
            double tPrev = 0;
            for (int i = 0; i < t.Count; i++)
            {
                if (t[i] - tPrev < unsplit.Domain[1])
                {
                    List<Curve> temp = unsplit.Split(t[i] - tPrev);
                    split.Add(temp[0]);
                    unsplit = temp[1];
                }
                else
                {
                    break;
                }
                tPrev = t[i];
            }

            split.Add(unsplit);

            return split;
        }

        public static List<Curve> Split(this Curve curve, double t)
        {
            if (!curve.IsNurbForm) curve.CreateNurbForm();
            if (t > curve.Domain[1]) return new List<Curve>() { curve };

            Curve newCurve = curve.DuplicateCurve();

            int insertedIndex = newCurve.InsertKnot(t);
            int startIndex = insertedIndex;

            while (insertedIndex + 1 < curve.Degree || curve.PointCount - (startIndex + curve.Degree - 1) < curve.Degree)
            {
                insertedIndex = newCurve.InsertKnot(t);
            }

            double[] midPoint = curve.Degree > 1 ? Utils.Merge<double>(newCurve.UnsafePointAt(t), new double[] { 1 }) : new double[0];
            double[] lhsPnts = Utils.Merge<double>(newCurve.LeftControlPoints(t), midPoint);
            double[] rhsPnts = Utils.Merge<double>(midPoint, newCurve.RightControlPoints(t));

            double[] lhsWeights = new double[lhsPnts.Length / (curve.Dimensions + 1)];
            double[] rhsWeights = new double[rhsPnts.Length / (curve.Dimensions + 1)];

            double[] lhsKnots = new double[lhsWeights.Length + curve.Order];
            double[] rhsKnots = new double[rhsWeights.Length + curve.Order];

            Array.Copy(newCurve.Weights, lhsWeights, lhsWeights.Length - 1);
            Array.Copy(newCurve.Weights, insertedIndex + 1, rhsWeights, 1, rhsWeights.Length - 1);

            double tRatio = (t - curve.Knots[insertedIndex + curve.Degree - 1]) / (newCurve.Knots[insertedIndex + curve.Degree + 1] - newCurve.Knots[insertedIndex + curve.Degree - 1]);

            double midWeightValue = (1 - tRatio) * newCurve.Weights[insertedIndex] + tRatio * newCurve.Weights[insertedIndex + 1];

            lhsWeights[lhsWeights.Length - 1] = midWeightValue;
            rhsWeights[0] = midWeightValue;

            Array.Copy(newCurve.Knots, lhsKnots, insertedIndex + curve.Order);
            for (int i = lhsKnots.Length - curve.Order; i < lhsKnots.Length; i++)
            {
                lhsKnots[i] = t;
            }

            Array.Copy(newCurve.Knots, insertedIndex + curve.Degree, rhsKnots, curve.Degree, rhsKnots.Length - curve.Degree);
            for (int i = curve.Degree; i < rhsKnots.Length; i++)
            {
                rhsKnots[i] -= t;
            }

            Curve lhs = curve.ShallowDuplicate();
            Curve rhs = curve.ShallowDuplicate();
            
            lhs.SetControlPoints(lhsPnts);
            lhs.SetKnots(lhsKnots);
            lhs.SetWeights(lhsWeights);
            lhs.SetDegree(curve.Degree);
            
            rhs.SetControlPoints(rhsPnts);
            rhs.SetKnots(rhsKnots);
            rhs.SetWeights(rhsWeights);
            rhs.SetDegree(curve.Degree);

            return new List<Curve>() { lhs, rhs };
        }

        public static bool Equal(this Curve curve, Curve other, double tolerance = 0.001)
        {
            if (!curve.IsNurbForm) curve.CreateNurbForm();
            if (Utils.Equal(curve.Length(), other.Length(), tolerance))
            {
                if ((ArrayUtils.Equal(curve.StartPoint, other.StartPoint, tolerance) &&
                    ArrayUtils.Equal(curve.EndPoint, other.EndPoint, tolerance)) ||
                    ArrayUtils.Equal(curve.StartPoint, other.EndPoint, tolerance) &&
                    ArrayUtils.Equal(curve.EndPoint, other.StartPoint, tolerance))
                {
                    return ArrayUtils.Equal(curve.UnsafePointAt(curve.Domain[1] / 2), other.UnsafePointAt(other.Domain[1] / 2), tolerance);
                }
            }
            return false;
        }

        public static Point ClosestPoint(this Curve curve, Point point)
        {
            switch (curve.GeometryType)
            {
                case GeometryType.Arc:
                case GeometryType.Circle:
                    break;
                case GeometryType.Line:
                    XLine.ClosestPoint(curve as Line, point);
                    break;
                case GeometryType.Polyline:
                    XPolyline.ClosestPoint(curve as Polyline, point);
                    break;
                case GeometryType.PolyCurve:
                    XPolyCurve.ClosestPoint(curve as PolyCurve, point);
                    break;
            }
            throw new NotImplementedException();
        }

        public static bool IsPlanar(this Curve curve)
        {
            return PlaneUtils.PointsInSamePlane(curve.ControlPointVector, curve.Dimensions + 1);
        }

        public static bool IsClosed(this Curve curve)
        {
            return ArrayUtils.Equal(curve.EndPoint, curve.StartPoint, 0.0001);
        }

        public static bool TryGetPlane(this Curve curve, out Plane plane)
        {
            plane = Create.PlaneFromPointArray(curve.ControlPointVector, curve.Dimensions + 1);
            return plane != null;
        }

        public static void Transform(Curve curve, Transform t)
        {
            curve.SetControlPoints(ArrayUtils.MultiplyMany(t, curve.ControlPointVector));
            curve.Update();
        }

        public static void Translate(Curve curve, Vector v)
        {
            curve.SetControlPoints(ArrayUtils.Add(curve.ControlPointVector, v));
        }

        public static void Mirror(Curve curve, Plane p)
        {
            curve.SetControlPoints(ArrayUtils.Add(ArrayUtils.Multiply(p.ProjectionVectors(curve.ControlPointVector), 2), curve.ControlPointVector));
            curve.Update();
        }

        public static void Project(Curve curve, Plane p)
        {
            curve.SetControlPoints(ArrayUtils.Add(p.ProjectionVectors(curve.ControlPointVector), curve.ControlPointVector));
            curve.Update();
        }

        public static Curve Flip(this Curve curve)
        {
            if (curve.GeometryType == BHoM.Geometry.GeometryType.PolyCurve)
            {
                PolyCurve c = curve as PolyCurve;
                for (int i = 0; i < c.Curves.Count; i++)
                {
                    c.Curves[i].Flip();
                }
                c.Curves.Reverse();
            }
            curve.SetControlPoints(Utils.Reverse<double>(curve.ControlPointVector, curve.Dimensions + 1));
            if (curve.IsNurbForm)
            {
                double max = curve.Knots.Max();
                curve.SetKnots(ArrayUtils.Sub(new double[] { max }, curve.Knots));
                curve.SetWeights(curve.Weights.Reverse().ToArray());
            }
            return curve;
        }

 

        public static Curve Append(this Curve curve, Curve other)
        {
            curve.ChangeDegree(other.Degree);
            other.ChangeDegree(curve.Degree);

            List<double> knots = new List<double>();
            List<double> pnts = new List<double>();
            List<double> weights = new List<double>();

            knots.AddRange(Utils.SubArray<double>(curve.Knots, 0, curve.Knots.Length - 1));
            pnts.AddRange(curve.ControlPointVector);
            weights.AddRange(curve.Weights);

            for (int i = 0; i < other.Knots.Length; i++)
            {
                if (other.Knots[i] != 0) knots.Add(other.Knots[i] + curve.Knots[curve.Knots.Length - 1]);
            }

            pnts.AddRange(Utils.SubArray<double>(other.ControlPointVector, curve.Dimensions + 1, other.ControlPointVector.Length - curve.Dimensions - 1));
            weights.AddRange(Utils.SubArray<double>(other.Weights, 1, other.Weights.Length - 1));

            if (curve.Degree == 1)
            {
                return new Polyline(pnts.ToArray(), curve.Dimensions);
            }
            else
            {
                List<Curve> crvs = new List<Curve>();
                crvs.AddRange(curve.Explode());
                crvs.AddRange(other.Explode());

                PolyCurve result = new PolyCurve(crvs);
                result.SetControlPoints(pnts.ToArray());
                result.SetDegree(curve.Order);
                result.SetWeights(weights.ToArray());
                result.SetKnots(knots.ToArray());

                return result;
            }
        }

    }
}
