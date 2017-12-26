using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** public Methods - Vectors                  ****/
        /***************************************************/

        //TODO: Testing needed!!
        public static Line PlaneIntersection(this Plane plane1, Plane plane2, double tolerance = Tolerance.Distance)
        {

            if (plane1.Normal.IsParallel(plane2.Normal) != 0)
                return null;

            //Calculate tangent of line perpendicular to the normal of the two planes
            Vector tangent = plane1.Normal.CrossProduct(plane2.Normal).Normalise();

            //d-values from plane equation: ax+by+cz+d=0
            double d1 = -plane1.Normal.DotProduct(Create.Vector(plane1.Origin));
            double d2 = -plane2.Normal.DotProduct(Create.Vector(plane2.Origin));

            Point orgin;

            Vector n1 = plane1.Normal;
            Vector n2 = plane2.Normal;

            if (tangent.Z != 0)
            {
                double x0 = (n1.Y * d2 - n2.Y * d1) / (n1.X * n2.Y - n2.X * n1.Y);
                double y0 = (n2.X * d1 - n1.X * d2) / (n1.X * n2.Y - n2.X * n1.Y);

                orgin = new Point { X = x0, Y = y0, Z = 0 };
            }
            else if (tangent.Y != 0)
            {
                double x0 = (n1.Z * d2 - n2.Z * d1) / (n1.X * n2.Z - n2.X * n1.Z);
                double z0 = (n2.X * d1 - n1.X * d2) / (n1.X * n2.Z - n2.X * n1.Z);
                orgin = new Point { X = x0, Y = 0, Z = z0 };
            }
            else
            {
                double y0 = (n1.Z * d2 - n2.Z * d1) / (n1.Y * n2.Z - n2.Y * n1.Z);
                double z0 = (n2.Y * d1 - n1.Y * d2) / (n1.Y * n2.Z - n2.Y * n1.Z);
                orgin = new Point { X = 0, Y = y0, Z = z0 };
            }


            //Find one point that is on both planes, assume one of the components = 0
            //if (tangent.Z != 0)
            //{
            //    double x0 = (d1 / plane1.Normal.Y - d2) / (plane2.Normal.X - plane1.Normal.X / plane2.Normal.Y);
            //    double y0 = (-d1 - plane1.Normal.X * x0) / plane1.Normal.Y;
            //    orgin = new Point { X = x0, Y = y0, Z = 0 };
            //}
            //else if (tangent.Y != 0)
            //{
            //    double x0 = (d1 / plane1.Normal.Z - d2) / (plane2.Normal.X - plane1.Normal.X / plane2.Normal.Z);
            //    double z0 = (-d1 - plane1.Normal.X * x0) / plane1.Normal.Z;
            //    orgin = new Point { X = x0, Y = 0, Z = z0 };
            //}
            //else
            //{
            //    double y0 = (d1 / plane1.Normal.Z - d2) / (plane2.Normal.Y - plane1.Normal.Y / plane2.Normal.Z);
            //    double z0 = (-d1 - plane1.Normal.Y * y0) / plane1.Normal.Z;
            //    orgin = new Point { X = 0, Y = y0, Z = z0 };
            //}

            Line result = new Line { Start = orgin, End = orgin + tangent };
            result.Infinite = true;
            return result;
        }


        /***************************************************/
        /**** public Methods - Curves                   ****/
        /***************************************************/

        //TODO: Testing needed!!
        public static List<Point> PlaneIntersections(this Arc curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            //Construct cricle for intersection
            Circle circle = Create.Circle(curve.Start, curve.Middle, curve.End);
            //Get circle intersection points
            List<Point> circleInter = circle.PlaneIntersections(plane, tolerance);

            if (circleInter.Count < 0)
                return new List<Point>();

            //Construct lines for checking
            Line line1 = new Line { Start = curve.Start, End = curve.Middle };
            Line line2 = new Line { Start = curve.Middle, End = curve.End };

            List<Point> interPoints = new List<Point>();

            //Check if interpoints are on the arc
            for (int i = 0; i < circleInter.Count; i++)
            {
                if (line1.LineIntersection(new Line { Start = circle.Centre, End = circleInter[i] }, false, tolerance) != null)
                    interPoints.Add(circleInter[i]);
                else if (line2.LineIntersection(new Line { Start = circle.Centre, End = circleInter[i] }, false, tolerance) != null)
                    interPoints.Add(circleInter[i]);
            }

            return interPoints;


            throw new NotImplementedException();
        }

        /***************************************************/

        //TODO: Testing needed!!
        public static List<Point> PlaneIntersections(this Circle curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            if (curve.Normal.IsParallel(plane.Normal) != 0)
                return new List<Point>();

            Line l = plane.PlaneIntersection(new Plane { Origin = curve.Centre, Normal = curve.Normal });

            Point tempPt = l.ClosestPoint(curve.Centre, true);

            double sqrDist = tempPt.GetSquareDistance(curve.Centre);
            double sqrRad = curve.Radius * curve.Radius;

            if (Math.Abs(sqrRad - sqrDist) < tolerance*tolerance)
                return new List<Point> { tempPt };
            else if (sqrDist < sqrRad)
            {
                Vector v = l.GetDirection();
                double dist = Math.Sqrt(sqrRad - sqrDist);
                v = v * dist;
                return new List<Point> { tempPt + v, tempPt - v };
            }
            else
            {
                return new List<Point>();
            }
        }

        /***************************************************/

        public static List<Point> PlaneIntersections(this Line curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            return new List<Point> { curve.PlaneIntersection(plane, false, tolerance) };
        }

        /***************************************************/

        public static Point PlaneIntersection(this Line line, Plane plane, bool useInfiniteLine = true, double tolerance = Tolerance.Distance)
        {
            useInfiniteLine &= line.Infinite;

            Vector dir = (line.End - line.Start);//.Normalise();

            //Return null if parallel
            if (Math.Abs(dir * plane.Normal) < tolerance)
                return null;

            double t = (plane.Normal * (plane.Origin - line.Start)) / (plane.Normal * dir);

            // Return null if intersection out of segment limits
            if (!useInfiniteLine && (t < 0 || t > 1))
                return null;

            return line.Start + t * dir;
        }

        /***************************************************/
        public static List<Point> PlaneIntersections(this NurbCurve c, Plane p, double tolerance = Tolerance.Distance)
        {
            List<double> curveParameters;
            return PlaneIntersections(c, p, out curveParameters, tolerance);
        }

        /***************************************************/

        //TODO: Testing needed!!
        public static List<Point> PlaneIntersections(this NurbCurve c, Plane p, out List<double> curveParameters, double tolerance = Tolerance.Distance)
        {

            List<Point> result = new List<Point>();
            int rounding = (int)Math.Log(1.0 / tolerance, 10);
            curveParameters = new List<double>();
            List<int> sameSide = p.Side(c.ControlPoints, tolerance);

            int degree = c.Degree();
            int previousSide = sameSide[0];
            int Length = c.IsClosed() && sameSide[sameSide.Count - 1] == 0 ? sameSide.Count - 1 : sameSide.Count;

            double prevMin = 0;

            for (int i = 1; i < Length; i++)
            {
                if (sameSide[i] != previousSide)
                {
                    if (previousSide != 0)
                    {
                        //double maxT = c.Knots[i + degree];
                        //double minT = c.Knots[i];
                        double minT = 0;
                        for (int j = i-1; j <= degree+i-1; j++)
                        {
                            minT += c.Knots[j];
                        }
                        minT /= (degree);
                        minT = minT < prevMin ? prevMin : minT;

                        double maxT = 0;
                        for (int j = i; j <= degree+i+1; j++)
                        {
                            maxT += c.Knots[j];
                        }
                        maxT /= (degree);

                        if (i < Length - 1 && sameSide[i] == 0 && sameSide[i + 1] != 0)
                        {
                            maxT = c.Knots[i + degree + 1];
                            minT = c.Knots[i + degree];
                            i++;
                        }

                        Point interPt = CurveParameterAtPlane(p, c, ref minT, ref maxT, c.PointAtParameter(minT), c.PointAtParameter(maxT), tolerance);
                        if (interPt != null)
                        {
                            result.Add(interPt);
                            curveParameters.Add(Math.Round((minT + maxT) / 2, rounding));
                        }
                        prevMin = minT;
                    }
                    else
                    {
                        result.Add(c.PointAtParameter(c.Knots[i - 1]));
                        curveParameters.Add(c.Knots[i - 1]);
                    }
                    previousSide = sameSide[i];
                }
            }

            if (sameSide[sameSide.Count - 1] == 0 && previousSide != sameSide[sameSide.Count - 1] && result.Count % 2 == 1)
            {
                result.Add(c.IGetEndPoint());
                curveParameters.Add(sameSide[sameSide.Count - 1]);
            }

            return result;

            //List<Point> result = new List<Point>();
            //int rounding = (int)Math.Log(1.0 / tolerance, 10);
            //curveParameters = new List<double>();
            //int[] sameSide = p.Side(c.ControlPointVector, tolerance);

            //int degree = c.Degree();
            //int previousSide = sameSide[0];
            //int Length = c.IsClosed() && sameSide[sameSide.Length - 1] == 0 ? sameSide.Length - 1 : sameSide.Length;

            //for (int i = 1; i < Length; i++)
            //{
            //    if (sameSide[i] != previousSide)
            //    {
            //        if (previousSide != 0)
            //        {
            //            double maxT = c.Knots[i + degree];
            //            double minT = c.Knots[i];
            //            if (i < Length - 1 && sameSide[i] == 0 && sameSide[i + 1] != 0)
            //            {
            //                maxT = c.Knots[i + degree + 1];
            //                minT = c.Knots[i + degree];
            //                i++;
            //            }
            //            result.Add(new Point(CurveParameterAtPlane(p, c, tolerance, ref minT, ref maxT, c.UnsafePointAt(minT), c.UnsafePointAt(maxT))));
            //            curveParameters.Add(Math.Round((minT + maxT) / 2, rounding));
            //        }
            //        else
            //        {
            //            result.Add(c.PointAt(c.Knots[i - 1]));
            //            curveParameters.Add(c.Knots[i - 1]);
            //        }
            //        previousSide = sameSide[i];
            //    }
            //}

            //if (sameSide[sameSide.Length - 1] == 0 && previousSide != sameSide[sameSide.Length - 1] && result.Count % 2 == 1)
            //{
            //    result.Add(c.IGetEndPoint());
            //    curveParameters.Add(sameSide[sameSide.Length - 1]);
            //}

            //return result;

            throw new NotImplementedException();
        }

        /***************************************************/

        //TODO: Testing needed!!
        public static List<Point> PlaneIntersections(this PolyCurve curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            return curve.Curves.SelectMany(x => x.IPlaneIntersections(plane, tolerance)).ToList();
            //throw new NotImplementedException();
        }

        /***************************************************/

        //TODO: Testing needed!!
        public static List<Point> PlaneIntersections(this Polyline curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            List<Point> result = new List<Point>();
            List<int> sameSide = plane.Side(curve.ControlPoints, tolerance);

            int previousSide = sameSide[0];
            int Length = curve.IsClosed() && sameSide[sameSide.Count - 1] == 0 ? sameSide.Count - 1 : sameSide.Count;

            for (int i = 1; i < Length; i++)
            {
                if (sameSide[i] != previousSide)
                {
                    if (previousSide != 0)
                    {
                        Line line = new Line { Start = curve.ControlPoints[i - 1], End = curve.ControlPoints[i] };
                        Point pt = PlaneIntersection(line, plane, false, tolerance);
                        if(pt != null)
                            result.Add(pt);
                    }
                    else
                    {
                        result.Add(curve.ControlPoints[i-1]);
                    }
                    previousSide = sameSide[i];
                }
            }

            if (sameSide[sameSide.Count - 1] == 0 && previousSide != sameSide[sameSide.Count - 1] && result.Count % 2 == 1)
            {
                result.Add(curve.IGetEndPoint());
            }

            return result;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<Point> IPlaneIntersections(this ICurve curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            return PlaneIntersections(curve as dynamic, plane, tolerance);
        }


        /***************************************************/
        /**** Private Methods - Support methods         ****/   //TODO: To be moved?
        /***************************************************/

        private static Point CurveParameterAtPlane(Plane p, NurbCurve c, ref double minT, ref double maxT, Point p1, Point p2, double tolerance = Tolerance.Distance)
        {
              
            double mid = (minT + maxT) / 2;
            if (minT == maxT)
            {
                return p1;
            }

            Point p3 = c.PointAtParameter(mid);
            if (p3.IsInPlane(p, tolerance)) return p3;

            if (!p1.IsSameSide(p, p3, tolerance))
            {
                maxT = mid;
                return CurveParameterAtPlane(p, c, ref minT, ref maxT, p1, p3, tolerance);
            }
            else if (!p2.IsSameSide(p, p3, tolerance))
            {
                minT = mid;
                return CurveParameterAtPlane(p, c, ref minT, ref maxT, p3, p2, tolerance);
            }
            else
            {
                return null;

                //List<int> side = p.Side(new List<Point> { p3 }, tolerance);

                //Vector tangent = c.GetTangentAt(mid);
                //double dotProd = tangent.DotProduct(p.Normal);

                //if (Math.Abs(dotProd) < Tolerance.Angle)
                //    return null;
                //else if (dotProd * (double)side[0] > 0)
                //{
                //    maxT = mid;
                //    return CurveParameterAtPlane(p, c, ref minT, ref maxT, p1, p3, tolerance);
                //}
                //else
                //{
                //    minT = mid;
                //    return CurveParameterAtPlane(p, c, ref minT, ref maxT, p3, p2, tolerance);
                //}
            }
        }

        /***************************************************/
    }
}
