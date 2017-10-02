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
        /**** Public Methods - Vector                   ****/
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
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static double GetLength(this Arc curve)
        {
            return curve.GetAngle() * curve.GetRadius();
        }

        /***************************************************/

        public static double GetLength(this Circle curve)
        {
            return 2 * Math.PI * curve.Radius;
        }

        /***************************************************/

        public static double GetLength(this Line curve)
        {
            return (curve.Start - curve.End).GetLength();
        }

        /***************************************************/

        public static double GetLength(this NurbCurve curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static double GetLength(this PolyCurve curve)
        {
            return curve.Curves.Sum(x => GetLength(x as Line));
        }

        /***************************************************/

        public static double GetLength(this Polyline curve)
        {
            double length = 0;
            List<Point> pts = curve.ControlPoints;

            for (int i = 1; i < pts.Count; i++)
                length += (pts[i] - pts[i-1]).GetLength();

            return length;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double _GetLength(this ICurve curve)
        {
            return GetLength(curve as dynamic);
        }
    }
}
