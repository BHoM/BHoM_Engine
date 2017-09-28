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

        public static Point GetEndPoint(this ICurve curve)
        {
            return _GetEndPoint(curve as dynamic);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Point _GetEndPoint(this Arc arc)
        {
            return arc.End;
        }

        /***************************************************/

        private static Point _GetEndPoint(this Circle circle)
        {
            Vector n = circle.Normal;
            Vector EndDir = Math.Abs(n.Z) < Math.Abs(n.X) ? new Vector(n.Y, -n.X, 0) : new Vector(0, n.Z, -n.Y);
            return circle.Centre + circle.Radius * EndDir.GetNormalised();
        }

        /***************************************************/

        private static Point _GetEndPoint(this Line line)
        {
            return line.End;
        }

        /***************************************************/

        private static Point _GetEndPoint(this NurbCurve curve)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return null;

            return pts.Last();
        }

        /***************************************************/

        private static Point _GetEndPoint(this PolyCurve curve)
        {
            List<ICurve> curves = curve.Curves;

            for (int i = curves.Count -1; i >= 0; i--)
            {
                Point End = curves[i].GetEndPoint();
                if (End != null)
                    return End;
            }

            return null;
        }

        /***************************************************/

        private static Point _GetEndPoint(this Polyline curve)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return null;

            return pts.Last();
        }
    }
}
