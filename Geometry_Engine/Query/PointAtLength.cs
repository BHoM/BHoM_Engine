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

        public static Point GetPointAtLength(this Arc curve, double length)
        {
            Point centre = curve.GetCentre();
            double alfa = curve.GetAngle() * length / curve.GetLength();
            Vector localX = curve.Start - centre;
            return new Point(localX.GetRotated(alfa, curve.GetPlane().Normal) as Vector) + centre;
        }

        /***************************************************/

        public static Point GetPointAtLength(this Circle curve, double length)
        {
            double alfa = 2 * Math.PI * length / curve.GetLength();
            Vector localX = curve.Normal.GetCrossProduct(Vector.XAxis).GetNormalised() * curve.Radius;
            return new Point(localX.GetRotated(alfa, curve.Normal) as Vector);
        }

        /***************************************************/

        public static Point GetPointAtLength(this Line curve, double length)
        {
            return GetPointAtParameter(curve, length / curve.GetLength());
        }

        /***************************************************/

        public static Point GetPointAtLength(this NurbCurve curve, double length)
        {
            throw new NotImplementedException(); // TODO Add NurbCurve PointAt method
        }

        /***************************************************/

        public static Point GetPointAtLength(this PolyCurve curve, double length)
        {
            throw new NotImplementedException(); // TODO Relies on NurbCurve PointAt method
        }

        /***************************************************/

        public static Point GetPointAtLength(this Polyline curve, double length)
        {
            List<Line> lines = curve.GetExploded() as List<Line>;
            double sum = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                sum += lines[i].GetLength();
                if (length <= sum)
                {
                    return lines[i].GetPointAtLength(length - sum + lines[i].GetLength());
                }
            }
            return null;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Point IGetPointAtLength(this ICurve curve, double length)
        {
            if (length > curve.IGetLength())
            {
                throw new ArgumentOutOfRangeException("Length must be less than the length of the curve"); // Turn into warning
            }
            return GetPointAtLength(curve as dynamic, length);
        }
    }
}
