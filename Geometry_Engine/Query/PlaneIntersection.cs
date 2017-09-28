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
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Point> GetIntersections(this ICurve curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            return _GetIntersections(curve as dynamic, plane, tolerance);
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

        public static Line GetIntersection(this Plane p, Plane plane, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<Point> _GetIntersections(this Arc curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static List<Point> _GetIntersections(this Circle curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static List<Point> _GetIntersections(this Line curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static List<Point> _GetIntersections(this NurbCurve c, Plane p,  double tolerance = Tolerance.Distance)
        {
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
            //    result.Add(c._GetEndPoint());
            //    curveParameters.Add(sameSide[sameSide.Length - 1]);
            //}

            //return result;

            throw new NotImplementedException();
        }

        /***************************************************/

        private static List<Point> _GetIntersections(this PolyCurve curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static List<Point> _GetIntersections(this Polyline curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }
    }
}
