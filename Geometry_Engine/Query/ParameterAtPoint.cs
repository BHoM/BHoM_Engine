using System;
using System.Collections.Generic;
using BH.Engine.Geometry;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static double ParameterAtPoint(this Arc curve, Point point, double tolerance = Tolerance.Distance)
        {
            if (curve.ClosestPoint(point).SquareDistance(point) > tolerance * tolerance) return -1;
            Point centre = curve.Centre();
            Vector normal = curve.FitPlane().Normal;
            Vector v1 = curve.Start - centre;
            Vector v2 = curve.End - centre;
            Vector v = point - centre;
            return v1.SignedAngle(v, normal) / v1.SignedAngle(v2, normal);
        }

        /***************************************************/

        public static double ParameterAtPoint(this Circle curve, Point point, double tolerance = Tolerance.Distance)
        {
            if (curve.ClosestPoint(point).SquareDistance(point) > tolerance * tolerance) return -1;
            Vector v1 = curve.StartPoint() - curve.Centre;
            Vector v2 = point - curve.Centre;
            return v1.SignedAngle(v2, curve.Normal) / (2 * Math.PI);
        }

        /***************************************************/

        public static double ParameterAtPoint(this Line curve, Point point, double tolerance = Tolerance.Distance)
        {
            if (curve.ClosestPoint(point).SquareDistance(point) > tolerance * tolerance) return -1;
            return point.Distance(curve.Start) / curve.Length();
        }

        /***************************************************/

        public static double ParameterAtPoint(this NurbCurve curve, Point point, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static double ParameterAtPoint(this PolyCurve curve, Point point, double tolerance = Tolerance.Distance)
        {
            double sqrTol = tolerance * tolerance;
            double length = 0;
            foreach (ICurve c in curve.SubParts())
            {
                if (c.IClosestPoint(point).SquareDistance(point) <= sqrTol)
                {
                    return (length + c.IParameterAtPoint(point) * c.ILength()) / curve.ILength();
                }
                else length += c.ILength();
            }
            return -1;
        }

        /***************************************************/

        public static double ParameterAtPoint(this Polyline curve, Point point, double tolerance = Tolerance.Distance)
        {
            double sqrTol = tolerance * tolerance;
            double param = 0;
            foreach (Line l in curve.SubParts())
            {
                if (l.ClosestPoint(point).SquareDistance(point) <= sqrTol)
                {
                    return (param + l.ParameterAtPoint(point)) / curve.Length();
                }
                else param += l.Length();
            }
            return -1;
        }

        /***************************************************/

        public static double IParameterAtPoint(this ICurve curve, Point point, double tolerance = Tolerance.Distance)
        {
            return ParameterAtPoint(curve as dynamic, point, tolerance);
        }

        /***************************************************/
    }
}