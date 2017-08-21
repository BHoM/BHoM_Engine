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

        public static Point GetStartPoint(this ICurve curve)
        {
            return _GetStartPoint(curve as dynamic);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        
        private static Point _GetStartPoint(this Arc arc)
        {
            return arc.Start;
        }

        /***************************************************/

        private static Point _GetStartPoint(this Circle circle)
        {
            Vector n = circle.Normal;
            Vector startDir = Math.Abs(n.Z) < Math.Abs(n.X) ? new Vector(n.Y, -n.X, 0) : new Vector(0, n.Z, -n.Y);
            return circle.Centre + circle.Radius * startDir.GetNormalised();
        }

        /***************************************************/

        private static Point _GetStartPoint(this Line line)
        {
            return line.Start;
        }

        /***************************************************/

        private static Point _GetStartPoint(this NurbCurve curve)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return null;

            return pts.First();
        }

        /***************************************************/

        private static Point _GetStartPoint(this PolyCurve curve)
        {
            foreach (ICurve c in curve.Curves)
            {
                Point start = c.GetStartPoint();
                if (start != null)
                    return start;
            }

            return null;
        }

        /***************************************************/

        private static Point _GetStartPoint(this Polyline curve)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return null;

            return pts.First();
        }
    }
}
