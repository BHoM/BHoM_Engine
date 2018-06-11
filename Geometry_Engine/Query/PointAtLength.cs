using System;
using System.Collections.Generic;

using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Point PointAtLength(this Arc curve, double length)
        {
            return curve.PointAtParameter(length / curve.Length());
        }

        /***************************************************/

        public static Point PointAtLength(this Circle curve, double length)
        {
            double alfa = 2 * Math.PI * length / curve.Length();
            Vector localX = curve.Normal.CrossProduct(Vector.XAxis).Normalise() * curve.Radius;
            return Create.Point(localX.Rotate(alfa, curve.Normal)) + curve.Centre;
        }

        /***************************************************/

        public static Point PointAtLength(this Line curve, double length)
        {
            return PointAtParameter(curve, length / curve.Length());
        }

        /***************************************************/

        public static Point PointAtLength(this NurbCurve curve, double length)
        {
            throw new NotImplementedException(); // TODO Add NurbCurve PointAt method
        }

        /***************************************************/

        public static Point PointAtLength(this PolyCurve curve, double length)
        {
            double parameter = length / curve.Length();
            return curve.PointAtParameter(parameter);
        }

        /***************************************************/

        public static Point PointAtLength(this Polyline curve, double length)
        {
            double parameter = length / curve.Length();
            return curve.PointAtParameter(parameter);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Point IPointAtLength(this ICurve curve, double length)
        {
            if (length > curve.ILength())
            {
                throw new ArgumentOutOfRangeException("Length must be less than the length of the curve"); // Turn into warning
            }
            return PointAtLength(curve as dynamic, length);
        }

        /***************************************************/
    }
}
