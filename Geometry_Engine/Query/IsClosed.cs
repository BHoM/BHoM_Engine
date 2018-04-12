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

        public static bool IsClosed(this Arc arc, double tolerance = Tolerance.Distance)
        {
            return false;
            //TODO: Repleacing previous code below with allways returning false.
            //Start and end the same will lead to an invalid arc with our implementation
            //return arc.Start.SquareDistance(arc.End) <= tolerance; 
        }

        /***************************************************/

        public static bool IsClosed(this Circle circle)
        {
            return true;
        }

        /***************************************************/

        public static bool IsClosed(this Line line)
        {
            return false;
        }

        /***************************************************/

        public static bool IsClosed(this NurbCurve curve, double tolerance = Tolerance.Distance)
        {
            return curve.PointAtParameter(0).SquareDistance(curve.PointAtParameter(1)) < tolerance* tolerance;
        }

        /***************************************************/

        public static bool IsClosed(this PolyCurve curve, double tolerance = Tolerance.Distance)
        {
            List<ICurve> curves = curve.Curves;
            double sqTol = tolerance * tolerance;
            if (curves[0].IStartPoint().SquareDistance(curves.Last().IEndPoint()) > sqTol)
                return false;

            for (int i = 1; i < curves.Count; i++)
            {
                if (curves[i - 1].IEndPoint().SquareDistance(curves[i].IStartPoint()) > sqTol)
                    return false;
            }

            return true;
        }

        /***************************************************/

        public static bool IsClosed(this Polyline curve, double tolerance = Tolerance.Distance)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return false;

            return pts.First().SquareDistance(pts.Last()) < tolerance* tolerance;
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
