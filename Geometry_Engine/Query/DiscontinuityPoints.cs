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

        public static List<Point> GetDiscontinuityPoints(this Arc curve)
        {
            return new List<Point> { curve.Start, curve.End };
        }

        /***************************************************/

        public static List<Point> GetDiscontinuityPoints(this Circle curve)
        {
            return new List<Point>();
        }

        /***************************************************/

        public static List<Point> GetDiscontinuityPoints(this Line curve)
        {
            return new List<Point> { curve.Start, curve.End };
        }

        /***************************************************/

        public static List<Point> GetDiscontinuityPoints(this NurbCurve curve)
        {
            if (curve.Degree() == 1)         //TODO: Check that this is correct
                return curve.ControlPoints;
            else
                return new List<Point> { curve.StartPoint(), curve.GetEndPoint() };
        }

        /***************************************************/

        public static List<Point> GetDiscontinuityPoints(this PolyCurve curve)
        {
            return curve.Curves.SelectMany(x => x.IGetDiscontinuityPoints()).ToList();
        }

        /***************************************************/

        public static List<Point> GetDiscontinuityPoints(this Polyline curve)
        {
            return curve.ControlPoints;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<Point> IGetDiscontinuityPoints(this ICurve curve)
        {
            return GetDiscontinuityPoints(curve as dynamic);
        }
    }
}
