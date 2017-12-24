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
        
        public static Point GetStartPoint(this Arc arc)
        {
            return arc.Start;
        }

        /***************************************************/

        public static Point GetStartPoint(this Circle circle)
        {
            Vector n = circle.Normal;
            Vector startDir = Math.Abs(n.Z) < Math.Abs(n.X) ? new Vector(n.Y, -n.X, 0) : new Vector(0, n.Z, -n.Y);
            return circle.Centre + circle.Radius * startDir.GetNormalised();
        }

        /***************************************************/

        public static Point GetStartPoint(this Line line)
        {
            return line.Start;
        }

        /***************************************************/

        public static Point GetStartPoint(this NurbCurve curve)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return null;

            return pts.First();
        }

        /***************************************************/

        public static Point GetStartPoint(this PolyCurve curve)
        {
            foreach (ICurve c in curve.Curves)
            {
                Point start = c.IGetStartPoint();
                if (start != null)
                    return start;
            }

            return null;
        }

        /***************************************************/

        public static Point GetStartPoint(this Polyline curve)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return null;

            return pts.First();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Point IGetStartPoint(this ICurve curve)
        {
            return GetStartPoint(curve as dynamic);
        }
    }
}
