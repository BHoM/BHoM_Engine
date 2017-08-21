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

        public static List<Point> GetDiscontinuityPoints(this ICurve curve)
        {
            return _GetDiscontinuityPoints(curve as dynamic);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<Point> _GetDiscontinuityPoints(this Arc curve)
        {
            return new List<Point> { curve.Start, curve.End };
        }

        /***************************************************/

        private static List<Point> _GetDiscontinuityPoints(this Circle curve)
        {
            return new List<Point>();
        }

        /***************************************************/

        private static List<Point> _GetDiscontinuityPoints(this Line curve)
        {
            return new List<Point> { curve.Start, curve.End };
        }

        /***************************************************/

        private static List<Point> _GetDiscontinuityPoints(this NurbCurve curve)
        {
            if (curve.GetDegree() == 1)         //TODO: Check that this is correct
                return curve.ControlPoints;
            else
                return new List<Point> { curve.GetStartPoint(), curve.GetEndPoint() };
        }

        /***************************************************/

        private static List<Point> _GetDiscontinuityPoints(this PolyCurve curve)
        {
            return curve.Curves.SelectMany(x => x.GetDiscontinuityPoints()).ToList();
        }

        /***************************************************/

        private static List<Point> _GetDiscontinuityPoints(this Polyline curve)
        {
            return curve.ControlPoints;
        }
    }
}
