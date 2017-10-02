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
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static List<Point> GetControlPoints(this Arc curve)
        {
            return new List<Point> { curve.Start, curve.End };
        }

        /***************************************************/

        public static List<Point> GetControlPoints(this Circle curve)
        {
            return new List<Point>();
        }

        /***************************************************/

        public static List<Point> GetControlPoints(this Line curve)
        {
            return new List<Point> { curve.Start, curve.End };
        }

        /***************************************************/

        public static List<Point> GetControlPoints(this NurbCurve curve)
        {
            return curve.ControlPoints;
        }

        /***************************************************/

        public static List<Point> GetControlPoints(this PolyCurve curve)
        {
            return curve.Curves.SelectMany(x => x.IGetControlPoints()).ToList();
        }

        /***************************************************/

        public static List<Point> GetControlPoints(this Polyline curve)
        {
            return curve.ControlPoints;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<Point> IGetControlPoints(this ICurve curve)
        {
            return GetControlPoints(curve as dynamic);
        }
    }
}
