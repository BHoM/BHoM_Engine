using BH.oM.Geometry;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Arc Flip(this Arc curve)
        {
            return new Arc { Start = curve.End, Middle = curve.Middle, End = curve.Start };
        }

        /***************************************************/

        public static Circle Flip(this Circle curve)
        {
            return new Circle { Centre = curve.Centre, Normal = -curve.Normal, Radius = curve.Radius };
        }

        /***************************************************/

        public static Line Flip(this Line curve)
        {
            return new Line { Start = curve.End, End = curve.Start };
        }

        /***************************************************/

        public static NurbCurve Flip(this NurbCurve curve)
        {
            return new NurbCurve { ControlPoints = curve.ControlPoints.Reverse<Point>().ToList(), Weights = curve.Weights.Reverse<double>().ToList(), Knots = curve.Knots.Reverse<double>().ToList() };
        }

        /***************************************************/

        public static PolyCurve Flip(this PolyCurve curve)
        {
            return new PolyCurve { Curves = curve.Curves.Select(x => x.IFlip()).Reverse().ToList() };
        }

        /***************************************************/

        public static Polyline Flip(this Polyline curve)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.Reverse<Point>().ToList() };
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static ICurve IFlip(this ICurve curve)
        {
            return Flip(curve as dynamic);
        }

        /***************************************************/
    }
}
