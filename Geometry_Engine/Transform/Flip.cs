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
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Arc GetFlipped(this Arc curve)
        {
            return new Arc(curve.End, curve.Middle, curve.Start);
        }

        /***************************************************/

        public static Circle GetFlipped(this Circle curve)
        {
            return new Circle(curve.Centre, -curve.Normal, curve.Radius);
        }

        /***************************************************/

        public static Line GetFlipped(this Line curve)
        {
            return new Line(curve.End, curve.Start);
        }

        /***************************************************/

        public static NurbCurve GetFlipped(this NurbCurve curve)
        {
            return new NurbCurve(curve.ControlPoints.Reverse<Point>(), curve.Weights.Reverse<double>(), curve.Knots.Reverse<double>());
        }

        /***************************************************/

        public static PolyCurve GetFlipped(this PolyCurve curve)
        {
            return new PolyCurve(curve.Curves.Select(x => x.IGetFlipped()).Reverse());
        }

        /***************************************************/

        public static Polyline GetFlipped(this Polyline curve)
        {
            return new Polyline(curve.ControlPoints.Reverse<Point>());
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static ICurve IGetFlipped(this ICurve curve)
        {
            return GetFlipped(curve as dynamic);
        }
    }
}
