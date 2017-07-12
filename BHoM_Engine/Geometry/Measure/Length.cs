using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Measure
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double GetLength(this Vector vector)
        {
            return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        }

        /***************************************************/

        public static double GetLength(this ICurve curve)
        {
            return _GetLength(curve as dynamic);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double _GetLength(Arc curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static double _GetLength(Circle curve)
        {
            return 2 * Math.PI * curve.Radius;
        }

        /***************************************************/

        public static double _GetLength(Line curve)
        {
            return (curve.Start - curve.End).GetLength();
        }

        /***************************************************/

        private static double _GetLength(NurbCurve curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static double _GetLength(PolyCurve curve)
        {
            return curve.Curves.Sum(x => _GetLength(x as Line));
        }

        /***************************************************/

        public static double _GetLength(Polyline curve)
        {
            double length = 0;
            List<Point> pts = curve.ControlPoints;

            for (int i = 1; i < pts.Count; i++)
                length += (pts[i] - pts[i-1]).GetLength();

            return length;
        }
    }
}
