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

        public static Point GetPointAtLength(this ICurve curve, double length)
        {
            if (length > curve.GetLength())
            {
                throw new ArgumentOutOfRangeException("Length must be less than the length of the curve"); // Turn into warning
            }
            return _GetPointAtLength(curve as dynamic, length);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Point _GetPointAtLength(this Arc curve, double length)
        {
            Point centre = curve.GetCentre();
            double alfa = curve.GetAngle() * length / curve.GetLength();
            Vector localX = curve.Start - centre;
            return new Point(localX.GetRotated(alfa, curve.GetPlane().Normal) as Vector) + centre;
        }

        private static Point _GetPointAtLength(this Circle curve, double length)
        {
            double alfa = 2 * Math.PI * length / curve.GetLength();
            Vector localX = curve.Normal.GetCrossProduct(Vector.XAxis).GetNormalised() * curve.Radius;
            return new Point(localX.GetRotated(alfa, curve.Normal) as Vector);
        }

        private static Point _GetPointAtLength(this Line curve, double length)
        {
            Vector vector = curve.End - curve.Start;
            return (new Point(vector.GetNormalised() * length) + curve.Start);
        }

        private static Point _GetPointAtLength(this NurbCurve curve, double length)
        {
            throw new NotImplementedException(); // TODO Add NurbCurve PointAt method
        }

        private static Point _GetPointAtLength(this PolyCurve curve, double length)
        {
            throw new NotImplementedException(); // TODO Relies on NurbCurve PointAt method
        }

        private static Point _GetPointAtLength(this Polyline curve, double length)
        {
            List<Line> lines = curve.GetExploded() as List<Line>;
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
