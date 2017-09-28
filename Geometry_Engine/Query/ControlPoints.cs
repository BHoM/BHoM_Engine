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

        public static List<Point> GetControlPoints(this ICurve curve)
        {
            return _GetControlPoints(curve as dynamic);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<Point> _GetControlPoints(this Arc curve)
        {
            return new List<Point> { curve.Start, curve.End };
        }

        /***************************************************/

        private static List<Point> _GetControlPoints(this Circle curve)
        {
            return new List<Point>();
        }

        /***************************************************/

        private static List<Point> _GetControlPoints(this Line curve)
        {
            return new List<Point> { curve.Start, curve.End };
        }

        /***************************************************/

        private static List<Point> _GetControlPoints(this NurbCurve curve)
        {
            return curve.ControlPoints;
        }

        /***************************************************/

        private static List<Point> _GetControlPoints(this PolyCurve curve)
        {
            return curve.Curves.SelectMany(x => x.GetControlPoints()).ToList();
        }

        /***************************************************/

        private static List<Point> _GetControlPoints(this Polyline curve)
        {
            return curve.ControlPoints;
        }
    }
}
