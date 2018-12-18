using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Point PointAtParameter(this Arc curve, double t)
        {
            if (t < 0)
                t = 0;
            if (t > 1)
                t = 1;

            double alfa = curve.Angle() * t + curve.StartAngle;
            Vector localX = curve.CoordinateSystem.X;
            return curve.CoordinateSystem.Origin + localX.Rotate(alfa, curve.FitPlane().Normal) * curve.Radius;
        }

        /***************************************************/

        public static Point PointAtParameter(this Circle curve, double t)
        {
            if (t < 0)
                t = 0;
            if (t > 1)
                t = 1;

            return PointAtLength(curve, t * curve.Length());
        }

        /***************************************************/

        public static Point PointAtParameter(this Line curve, double t)
        {
            if (t < 0)
                t = 0;
            if (t > 1)
                t = 1;

            Vector vector = curve.End - curve.Start;
            return (curve.Start + vector * t);
        }

        /***************************************************/

        [NotImplemented]
        public static Point PointAtParameter(this NurbsCurve curve, double t)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static Point PointAtParameter(this PolyCurve curve, double parameter)
        {
            if (parameter == 1)
                return curve.IEndPoint();

            double cLength = parameter * curve.Length();
            foreach (ICurve c in curve.SubParts())
            {
                double l = c.ILength();
                if (l >= cLength)
                    return c.IPointAtParameter(cLength / l);

                cLength -= l;
            }

            return null;
        }

        /***************************************************/

        public static Point PointAtParameter(this Polyline curve, double parameter)
        {
            double cLength = parameter * curve.Length();
            foreach (Line line in curve.SubParts())
            {
                double l = line.Length();
                if (l >= cLength)
                    return line.IPointAtParameter(cLength / l);

                cLength -= l;
            }

            return null;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Point IPointAtParameter(this ICurve curve, double t)
        {
            return PointAtParameter(curve as dynamic, t);
        }

        /***************************************************/
    }
}
