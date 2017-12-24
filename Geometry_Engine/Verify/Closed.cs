using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static bool IsClosed(this Arc arc)
        {
            return arc.Start.GetSquareDistance(arc.End) < Tolerance.SqrtDist;
        }

        /***************************************************/

        public static bool IsClosed(this Circle circle)
        {
            return true;
        }

        /***************************************************/

        public static bool IsClosed(this Line line)
        {
            return line.Start.GetSquareDistance(line.End) < Tolerance.SqrtDist;
        }

        /***************************************************/

        public static bool IsClosed(this NurbCurve curve)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return false;

            return pts.First().GetSquareDistance(pts.Last()) < Tolerance.SqrtDist;
        }

        /***************************************************/

        public static bool IsClosed(this PolyCurve curve)
        {
            List<ICurve> curves = curve.Curves;

            if (curves[0].IGetStartPoint().GetSquareDistance(curves.Last().IGetEndPoint()) > Tolerance.SqrtDist)
                return false;

            for (int i = 1; i < curves.Count; i++)
            {
                if (curves[i - 1].IGetEndPoint().GetSquareDistance(curves[i].IGetStartPoint()) > Tolerance.SqrtDist)
                    return false;
            }

            return true;
        }

        /***************************************************/

        public static bool IsClosed(this Polyline curve)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return false;

            return pts.First().GetSquareDistance(pts.Last()) < Tolerance.SqrtDist;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static bool IIsClosed(this ICurve curve)
        {
            return IsClosed(curve as dynamic);
        }


    }
}
