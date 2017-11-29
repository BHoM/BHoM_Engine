using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** public Methods - Vectors                  ****/
        /***************************************************/

        //TODO: Testing needed!!
        public static Line GetIntersection(this Plane plane1, Plane plane2, double tolerance = Tolerance.Distance)
        {

            if (plane1.Normal.IsParallel(plane2.Normal) == 0)
                return null;

            //Calculate tangent of line perpendicular to the normal of the two planes
            Vector tangent = plane1.Normal.GetCrossProduct(plane2.Normal).GetNormalised();

            //d-values from plane equation: ax+by+cz+d=0
            double d1 = -plane1.Normal.GetDotProduct(new Vector(plane1.Origin));
            double d2 = -plane2.Normal.GetDotProduct(new Vector(plane2.Origin));

            Point orgin;

            //Find one point that is on both planes, assume one of the components = 0
            if (tangent.Z != 0)
            {
                double x0 = (d1 / plane1.Normal.Y - d2) / (plane2.Normal.X - plane1.Normal.X / plane2.Normal.Y);
                double y0 = (-d1 - plane1.Normal.X * x0) / plane1.Normal.Y;
                orgin = new Point(x0, y0, 0);
            }
            else if (tangent.Y != 0)
            {
                double x0 = (d1 / plane1.Normal.Z - d2) / (plane2.Normal.X - plane1.Normal.X / plane2.Normal.Z);
                double z0 = (-d1 - plane1.Normal.X * x0) / plane1.Normal.Z;
                orgin = new Point(x0, 0, z0);
            }
            else
            {
                double y0 = (d1 / plane1.Normal.Z - d2) / (plane2.Normal.Y - plane1.Normal.Y / plane2.Normal.Z);
                double z0 = (-d1 - plane1.Normal.Y * y0) / plane1.Normal.Z;
                orgin = new Point(0, y0, z0);
            }

            Line result = new Line(orgin, orgin + tangent);
            result.Infinite = true;
            return result;
        }


        /***************************************************/
        /**** public Methods - Curves                   ****/
        /***************************************************/

        //TODO: Testing needed!!
        public static List<Point> GetIntersections(this Arc curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            //Construct cricle for intersection
            Circle circle = Create.Circle(curve.Start, curve.Middle, curve.End);
            //Get circle intersection points
            List<Point> circleInter = circle.GetIntersections(plane, tolerance);

            if (circleInter.Count < 0)
                return new List<Point>();

            //Construct lines for checking
            Line line1 = new Line(curve.Start, curve.Middle);
            Line line2 = new Line(curve.Middle, curve.End);

            List<Point> interPoints = new List<Point>();

            //Check if interpoints are on the arc
            for (int i = 0; i < circleInter.Count; i++)
            {
                if (line1.GetIntersection(new Line(curve.End, circleInter[i]), false, tolerance) != null)
                    interPoints.Add(circleInter[i]);
                else if (line2.GetIntersection(new Line(curve.Start, circleInter[i]), false, tolerance) != null)
                    interPoints.Add(circleInter[i]);
            }

            return interPoints;


            throw new NotImplementedException();
        }

        /***************************************************/

        //TODO: Testing needed!!
        public static List<Point> GetIntersections(this Circle curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            if (curve.Normal.IsParallel(plane.Normal) == 0)
                return new List<Point>();

            Line l = plane.GetIntersection(new Plane(curve.Centre, curve.Normal));

            Point tempPt = l.GetClosestPoint(curve.Centre, true);

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

        public static List<Point> GetIntersections(this Line curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            return new List<Point> { curve.GetIntersection(plane, false, tolerance) };
        }

        /***************************************************/

        public static Point GetIntersection(this Line line, Plane plane, bool useInfiniteLine = true, double tolerance = Tolerance.Distance)
        {
            useInfiniteLine &= line.Infinite;

            Vector dir = (line.End - line.Start).GetNormalised();

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
        public static List<Point> GetIntersections(this NurbCurve c, Plane p, double tolerance = Tolerance.Distance)
        {
            List<double> curveParameters;
            return GetIntersections(c, p, out curveParameters, tolerance);
        }

        /***************************************************/

        //TODO: Testing needed!!
        public static List<Point> GetIntersections(this NurbCurve c, Plane p, out List<double> curveParameters, double tolerance = Tolerance.Distance)
        {

            List<Point> result = new List<Point>();
            int rounding = (int)Math.Log(1.0 / tolerance, 10);
            curveParameters = new List<double>();
            List<int> sameSide = p.GetSide(c.ControlPoints, tolerance);

            int degree = c.GetDegree();
            int previousSide = sameSide[0];
            int Length = c.IsClosed() && sameSide[sameSide.Count - 1] == 0 ? sameSide.Count - 1 : sameSide.Count;

            for (int i = 1; i < Length; i++)
            {
                if (sameSide[i] != previousSide)
                {
                    if (previousSide != 0)
                    {
                        double maxT = c.Knots[i + degree];
                        double minT = c.Knots[i];
                        if (i < Length - 1 && sameSide[i] == 0 && sameSide[i + 1] != 0)
                        {
                            maxT = c.Knots[i + degree + 1];
                            minT = c.Knots[i + degree];
                            i++;
                        }
                        result.Add(CurveParameterAtPlane(p, c, ref minT, ref maxT, c.GetPointAtParameter(minT), c.GetPointAtParameter(maxT), tolerance));
                        curveParameters.Add(Math.Round((minT + maxT) / 2, rounding));
                    }
                    else
                    {
                        result.Add(c.GetPointAtParameter(c.Knots[i - 1]));
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
            //int[] sameSide = p.GetSide(c.ControlPointVector, tolerance);

            //int degree = c.GetDegree();
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
        public static List<Point> GetIntersections(this PolyCurve curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            return curve.Curves.SelectMany(x => x.GetIntersections(plane, tolerance)).ToList();
            //throw new NotImplementedException();
        }

        /***************************************************/

        //TODO: Testing needed!!
        public static List<Point> GetIntersections(this Polyline curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            List<Point> result = new List<Point>();
            List<int> sameSide = plane.GetSide(curve.ControlPoints, tolerance);

            int previousSide = sameSide[0];
            int Length = curve.IsClosed() && sameSide[sameSide.Count - 1] == 0 ? sameSide.Count - 1 : sameSide.Count;

            for (int i = 1; i < Length; i++)
            {
                if (sameSide[i] != previousSide)
                {
                    if (previousSide != 0)
                    {
                        Line line = new Line(curve.ControlPoints[i - 1], curve.ControlPoints[i]);
                        result.Add(GetIntersection(line, plane, false, tolerance));
                    }
                    else
                    {
                        result.Add(curve.ControlPoints[i]);
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

        public static List<Point> GetIntersections(this ICurve curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            return GetIntersections(curve as dynamic, plane, tolerance);
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

            Point p3 = c.GetPointAtParameter(mid);
            if (p3.IsInPlane(p, tolerance)) return p3;

            if (p1.IsSameSide(p, p3, tolerance))
            {
                minT = mid;
                return CurveParameterAtPlane(p, c, ref minT, ref maxT, p3, p2, tolerance);
            }
            else
            {
                maxT = mid;
                return CurveParameterAtPlane(p, c, ref minT, ref maxT, p1, p3, tolerance);
            }
        }
    }
}
