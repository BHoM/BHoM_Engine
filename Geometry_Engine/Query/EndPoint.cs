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

        public static Point GetEndPoint(this Arc arc)
        {
            return arc.End;
        }

        /***************************************************/

        public static Point GetEndPoint(this Circle circle)
        {
            Vector n = circle.Normal;
            Vector EndDir = Math.Abs(n.Z) < Math.Abs(n.X) ? new Vector(n.Y, -n.X, 0) : new Vector(0, n.Z, -n.Y);
            return circle.Centre + circle.Radius * EndDir.Normalise();
        }

        /***************************************************/

        public static Point GetEndPoint(this Line line)
        {
            return line.End;
        }

        /***************************************************/

        public static Point GetEndPoint(this NurbCurve curve)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return null;

            return pts.Last();
        }

        /***************************************************/

        public static Point GetEndPoint(this PolyCurve curve)
        {
            List<ICurve> curves = curve.Curves;

            for (int i = curves.Count -1; i >= 0; i--)
            {
                Point End = curves[i].IGetEndPoint();
                if (End != null)
                    return End;
            }

            return null;
        }

        /***************************************************/

        public static Point GetEndPoint(this Polyline curve)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return null;

            return pts.Last();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Point IGetEndPoint(this ICurve curve)
        {
            return GetEndPoint(curve as dynamic);
        }
    }
}
