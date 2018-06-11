using BH.oM.Geometry;
using System.Linq;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Arc Flip(this Arc curve)
        {
            Vector newX = curve.EndPoint() - curve.CoordinateSystem.Origin;
            Vector newY = -curve.CoordinateSystem.Z.CrossProduct(newX);
            CoordinateSystem system = Create.CoordinateSystem(curve.CoordinateSystem.Origin, newX, newY);
            return new Arc { CoordinateSystem = system, Angle = curve.Angle, Radius = curve.Radius };
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
            List<double> oldKnots = curve.Knots;
            double prevValue = 0;
            List<double> newKnots = new List<double> { prevValue };
            for (int i = oldKnots.Count - 1; i > 0; i--)
            {
                newKnots.Add(prevValue + oldKnots[i] - oldKnots[i - 1]);
                prevValue = newKnots.Last();
            }
            return new NurbCurve
            {
                ControlPoints = curve.ControlPoints.Reverse<Point>().ToList(),
                Weights = curve.Weights.Reverse<double>().ToList(),
                Knots = newKnots
            };
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
