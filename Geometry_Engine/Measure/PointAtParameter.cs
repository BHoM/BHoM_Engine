using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Measure
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Point GetPointAtParameter(this ICurve curve, double t)
        {
            if (t > 1)
            {
                throw new ArgumentOutOfRangeException("Parameter must be less than the t of the curve"); // Turn into warning
            }
            return _GetPointAtParameter(curve as dynamic, t);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Point _GetPointAtParameter(this Arc curve, double t)
        {
            Point centre = curve.GetCentre();
            double alfa = curve.GetAngle() * t;
            Vector localX = curve.Start - curve.GetCentre();
            return new Point(localX.GetRotated(alfa, curve.GetPlane().Normal) as Vector) + centre;
        }

        private static Point _GetPointAtParameter(this Circle curve, double t)
        {
            double alfa = 2 * Math.PI * t;
            Vector localX = curve.Normal.GetCrossProduct(Vector.XAxis).GetNormalised() * curve.Radius;
            return new Point(localX.GetRotated(alfa, curve.Normal) as Vector);
        }

        private static Point _GetPointAtParameter(this Line curve, double t)
        {
            Vector vector = curve.End - curve.Start;
            return (new Point(vector * t) + curve.Start);
        }

        private static Point _GetPointAtParameter(this NurbCurve curve, double t)
        {
            throw new NotImplementedException(); // TODO NurbCurve.GetPointAtParameter()
        }

        private static Point _GetPointAtParameter(this PolyCurve curve, double t)
        {
            throw new NotImplementedException(); // TODO Polycurve.GetPointAtParameter() Relies on NurbCurve PointAt method
        }

        private static Point _GetPointAtParameter(this Polyline curve, double t)
        {
            List<Line> lines = curve.GetExploded() as List<Line>;
            double length = t * curve.GetLength();
            double sum = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                sum += lines[i].GetLength();
                if (length <= sum)
                {
                    return lines[i]._GetPointAtLength(length - sum + lines[i].GetLength());
                }
            }
            return null;
        }
    }
}
