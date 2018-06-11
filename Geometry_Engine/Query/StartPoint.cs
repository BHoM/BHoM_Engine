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
            return arc.CoordinateSystem.Origin + arc.CoordinateSystem.X * arc.Radius;
        }

        /***************************************************/

        public static Point StartPoint(this Circle circle)
        {
            Vector n = circle.Normal;
            Vector startDir = Math.Abs(n.Z) < Math.Abs(n.X) ? new Vector { X = n.Y, Y = -n.X, Z = 0 } : new Vector { X = 0, Y = n.Z, Z = -n.Y };
            return circle.Centre + circle.Radius * startDir.Normalise();
        }

        /***************************************************/

        public static Point StartPoint(this Line line)
        {
            return line.Start;
        }

        /***************************************************/

        public static Point StartPoint(this NurbCurve curve)
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
