using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Transform
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ICurve GetFlipped(this ICurve curve)
        {
            return _GetFlipped(curve as dynamic);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Arc _GetFlipped(this Arc curve)
        {
            return new Arc(curve.End, curve.Middle, curve.Start);
        }

        /***************************************************/

        private static Circle _GetFlipped(this Circle curve)
        {
            return new Circle(curve.Centre, -curve.Normal, curve.Radius);
        }

        /***************************************************/

        private static Line _GetFlipped(this Line curve)
        {
            return new Line(curve.End, curve.Start);
        }

        /***************************************************/

        private static NurbCurve _GetFlipped(this NurbCurve curve)
        {
            return new NurbCurve(curve.ControlPoints.Reverse<Point>(), curve.Weights.Reverse<double>(), curve.Knots.Reverse<double>());
        }

        /***************************************************/

        private static PolyCurve _GetFlipped(this PolyCurve curve)
        {
            return new PolyCurve(curve.Curves.Select(x => x.GetFlipped()).Reverse<ICurve>());
        }

        /***************************************************/

        private static Polyline _GetFlipped(this Polyline curve)
        {
            return new Polyline(curve.ControlPoints.Reverse<Point>());
        }
    }
}
