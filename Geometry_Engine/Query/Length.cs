using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Vector                   ****/
        /***************************************************/

        public static double Length(this Vector vector)
        {
            return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        }

        /***************************************************/

        public static double SquareLength(this Vector vector)
        {
            return vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;
        }

        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static double Length(this Arc curve)
        {
            return curve.Angle() * curve.Radius();
        }

        /***************************************************/

        public static double Length(this Circle curve)
        {
            return 2 * Math.PI * curve.Radius;
        }

        /***************************************************/

        public static double Length(this Line curve)
        {
            return (curve.Start - curve.End).Length();
        }

        /***************************************************/

        public static double Length(this NurbCurve curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static double Length(this PolyCurve curve)
        {
            return curve.Curves.Sum(c => c.ILength());
        }

        /***************************************************/

        public static double Length(this Polyline curve)
        {
            double length = 0;
            List<Point> pts = curve.ControlPoints;

            for (int i = 1; i < pts.Count; i++)
                length += (pts[i] - pts[i-1]).Length();

            return length;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double ILength(this ICurve curve)
        {
            return Length(curve as dynamic);
        }

        /***************************************************/
    }
}
