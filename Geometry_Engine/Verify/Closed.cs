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

        public static bool IsClosed(this ICurve curve)
        {
            return _IsClosed(curve as dynamic);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool _IsClosed(this Arc arc)
        {
            return arc.Start.GetSquareDistance(arc.End) < Tolerance.SqrtDist;
        }

        /***************************************************/

        private static bool _IsClosed(this Circle circle)
        {
            return true;
        }

        /***************************************************/

        private static bool _IsClosed(this Line line)
        {
            return line.Start.GetSquareDistance(line.End) < Tolerance.SqrtDist;
        }

        /***************************************************/

        private static bool _IsClosed(this NurbCurve curve)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return false;

            return pts.First().GetSquareDistance(pts.Last()) < Tolerance.SqrtDist;
        }

        /***************************************************/

        private static bool _IsClosed(this PolyCurve curve)
        {
            List<ICurve> curves = curve.Curves;

            if (curves[0].GetStartPoint().GetSquareDistance(curves.Last().GetEndPoint()) > Tolerance.SqrtDist)
                return false;

            for (int i = 1; i < curves.Count; i++)
            {
                if (curves[i - 1].GetEndPoint().GetSquareDistance(curves[i].GetStartPoint()) > Tolerance.SqrtDist)
                    return false;
            }

            return true;
        }

        /***************************************************/

        private static bool _IsClosed(this Polyline curve)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return false;

            return pts.First().GetSquareDistance(pts.Last()) < Tolerance.SqrtDist;
        }
    }
}
