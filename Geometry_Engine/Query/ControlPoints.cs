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

        public static List<Point> ControlPoints(this Arc curve)
        {
            return new List<Point> { curve.Start, curve.Middle, curve.End };
        }

        /***************************************************/

        public static List<Point> ControlPoints(this Circle curve)
        {
            return new List<Point>();
        }

        /***************************************************/

        public static List<Point> ControlPoints(this Line curve)
        {
            return new List<Point> { curve.Start, curve.End };
        }

        /***************************************************/

        public static List<Point> ControlPoints(this NurbCurve curve)
        {
            return curve.ControlPoints;
        }

        /***************************************************/

        public static List<Point> ControlPoints(this PolyCurve curve)
        {
            return curve.Curves.SelectMany(x => x.IControlPoints()).ToList();
        }

        /***************************************************/

        public static List<Point> ControlPoints(this Polyline curve)
        {
            return curve.ControlPoints;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<Point> IControlPoints(this ICurve curve)
        {
            return ControlPoints(curve as dynamic);
        }

        /***************************************************/
    }
}
