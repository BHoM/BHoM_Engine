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
            Point centre = curve.Centre();
            double alfa = curve.Angle() * length / curve.Length();
            Vector localX = curve.Start - centre;
            return new Point(localX.Rotate(alfa, curve.FitPlane().Normal) as Vector) + centre;
        }

        /***************************************************/

        public static Point PointAtLength(this Circle curve, double length)
        {
            double alfa = 2 * Math.PI * length / curve.Length();
            Vector localX = curve.Normal.CrossProduct(Vector.XAxis).Normalise() * curve.Radius;
            return new Point(localX.Rotate(alfa, curve.Normal) as Vector);
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
            throw new NotImplementedException(); // TODO Relies on NurbCurve PointAt method
        }

        /***************************************************/

        public static Point PointAtLength(this Polyline curve, double length)
        {
            List<Line> lines = curve.SubParts() as List<Line>;
            double sum = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                sum += lines[i].Length();
                if (length <= sum)
                {
                    return lines[i].PointAtLength(length - sum + lines[i].Length());
                }
            }
            return null;
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
