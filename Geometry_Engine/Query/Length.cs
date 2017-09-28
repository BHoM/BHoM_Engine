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

        public static double GetLength(this Vector vector)
        {
            return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
        }

        /***************************************************/

        public static double GetSquareLength(this Vector vector)
        {
            return vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;
        }

        /***************************************************/

        public static double GetLength(this ICurve curve)
        {
            return _GetLength(curve as dynamic);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static double _GetLength(this Arc curve)
        {
            return curve.GetAngle() * curve.GetRadius();
        }

        /***************************************************/

        private static double _GetLength(this Circle curve)
        {
            return 2 * Math.PI * curve.Radius;
        }

        /***************************************************/

        private static double _GetLength(this Line curve)
        {
            return (curve.Start - curve.End).GetLength();
        }

        /***************************************************/

        private static double _GetLength(this NurbCurve curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static double _GetLength(this PolyCurve curve)
        {
            return curve.Curves.Sum(x => _GetLength(x as Line));
        }

        /***************************************************/

        private static double _GetLength(this Polyline curve)
        {
            double length = 0;
            List<Point> pts = curve.ControlPoints;

            for (int i = 1; i < pts.Count; i++)
                length += (pts[i] - pts[i-1]).GetLength();

            return length;
        }
    }
}
