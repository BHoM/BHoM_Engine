using BH.oM.Geometry;
using BHoM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Verify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsClosed(ICurve curve)
        {
            return _IsClosed(curve as dynamic);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool _IsClosed(Arc arc)
        {
            return arc.Start.GetSquareDistance(arc.End) < Tolerance.SqrtDist;
        }

        /***************************************************/

        private static bool _IsClosed(Circle circle)
        {
            return true;
        }

        /***************************************************/

        private static bool _IsClosed(Line line)
        {
            return line.Start.GetSquareDistance(line.End) < Tolerance.SqrtDist;
        }

        /***************************************************/

        private static bool _IsClosed(NurbCurve curve)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return false;

            return pts.First().GetSquareDistance(pts.Last()) < Tolerance.SqrtDist;
        }

        /***************************************************/

        private static bool _IsClosed(PolyCurve curve)
        {
            Point start = curve.GetStartPoint();
            if (start == null)
                return false;

            Point end = curve.GetEndPoint();
            if (end == null)
                return false;

            return start.GetSquareDistance(end) < Tolerance.SqrtDist;
        }

        /***************************************************/

        private static bool _IsClosed(Polyline curve)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return false;

            return pts.First().GetSquareDistance(pts.Last()) < Tolerance.SqrtDist;
        }
    }
}
