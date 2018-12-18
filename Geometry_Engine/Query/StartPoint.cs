using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/
        
        public static Point StartPoint(this Arc arc)
        {
            Vector locSt = arc.CoordinateSystem.X * arc.Radius;
            return arc.CoordinateSystem.Origin + locSt.Rotate(arc.StartAngle, arc.CoordinateSystem.Z);
        }

        /***************************************************/

        public static Point StartPoint(this Circle circle)
        {
            Vector refVector = 1 - Math.Abs(circle.Normal.DotProduct(Vector.XAxis)) > Tolerance.Angle ? Vector.XAxis : Vector.ZAxis;
            Vector localX = circle.Normal.CrossProduct(refVector).Normalise() * circle.Radius;
            return circle.Centre + circle.Radius * localX.Normalise();
        }

        /***************************************************/

        public static Point StartPoint(this Line line)
        {
            return line.Start;
        }

        /***************************************************/

        public static Point StartPoint(this NurbsCurve curve)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return null;

            return pts.First();
        }

        /***************************************************/

        public static Point StartPoint(this PolyCurve curve)
        {
            foreach (ICurve c in curve.Curves)
            {
                Point start = c.IStartPoint();
                if (start != null)
                    return start;
            }

            return null;
        }

        /***************************************************/

        public static Point StartPoint(this Polyline curve)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return null;

            return pts.First();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Point IStartPoint(this ICurve curve)
        {
            return StartPoint(curve as dynamic);
        }

        /***************************************************/
    }
}
