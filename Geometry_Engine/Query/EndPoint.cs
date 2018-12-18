using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Point EndPoint(this Arc arc)
        {
            Vector locSt = arc.CoordinateSystem.X * arc.Radius;
            return arc.CoordinateSystem.Origin + locSt.Rotate(arc.EndAngle, arc.CoordinateSystem.Z);
        }

        /***************************************************/

        public static Point EndPoint(this Circle circle)
        {
            return circle.StartPoint();
        }

        /***************************************************/

        public static Point EndPoint(this Line line)
        {
            return line.End;
        }

        /***************************************************/

        public static Point EndPoint(this NurbsCurve curve)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return null;

            return pts.Last();
        }

        /***************************************************/

        public static Point EndPoint(this PolyCurve curve)
        {
            List<ICurve> curves = curve.Curves;

            for (int i = curves.Count -1; i >= 0; i--)
            {
                Point End = curves[i].IEndPoint();
                if (End != null)
                    return End;
            }

            return null;
        }

        /***************************************************/

        public static Point EndPoint(this Polyline curve)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return null;

            return pts.Last();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Point IEndPoint(this ICurve curve)
        {
            return EndPoint(curve as dynamic);
        }

        /***************************************************/
    }
}
