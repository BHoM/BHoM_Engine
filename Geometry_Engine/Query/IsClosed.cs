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
            return arc.Start.SquareDistance(arc.End) < Tolerance.SqrtDist;
        }

        /***************************************************/

        public static bool IsClosed(this Circle circle)
        {
            return true;
        }

        /***************************************************/

        public static bool IsClosed(this Line line)
        {
            return line.Start.SquareDistance(line.End) < Tolerance.SqrtDist;
        }

        /***************************************************/

        public static bool IsClosed(this NurbCurve curve)
        {
            return curve.PointAtParameter(0).SquareDistance(curve.PointAtParameter(1)) < Tolerance.SqrtDist;
        }

        /***************************************************/

        public static bool IsClosed(this PolyCurve curve)
        {
            List<ICurve> curves = curve.Curves;

            if (curves[0].IStartPoint().SquareDistance(curves.Last().IEndPoint()) > Tolerance.SqrtDist)
                return false;

            for (int i = 1; i < curves.Count; i++)
            {
                if (curves[i - 1].IEndPoint().SquareDistance(curves[i].IStartPoint()) > Tolerance.SqrtDist)
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

            return pts.First().SquareDistance(pts.Last()) < Tolerance.SqrtDist;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static bool IIsClosed(this ICurve curve)
        {
            return IsClosed(curve as dynamic);
        }

        /***************************************************/
    }
}
