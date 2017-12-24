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

        public static Vector GetStartDir(this Arc arc)
        {
            throw new NotImplementedException(); //TODO: get start dir of arc
        }

        /***************************************************/

        public static Vector GetStartDir(this Circle circle)
        {
            Vector n = circle.Normal;
            Vector startDir = Math.Abs(n.Z) < Math.Abs(n.X) ? new Vector(n.Y, -n.X, 0) : new Vector(0, n.Z, -n.Y);
            return circle.Normal.GetCrossProduct(startDir).GetNormalised();
        }

        /***************************************************/

        public static Vector GetStartDir(this Line line)
        {
            return new Vector(line.End.X - line.Start.X, line.End.Y - line.Start.Y, line.End.Z - line.Start.Z).GetNormalised();
        }

        /***************************************************/

        public static Vector GetStartDir(this NurbCurve curve)
        {
            throw new NotImplementedException(); //TODO: get start dir of nurbcurve
        }

        /***************************************************/

        public static Vector GetStartDir(this PolyCurve curve)
        {
            return curve.Curves.Count > 0 ? curve.Curves.First().IGetStartDir() : null;
        }

        /***************************************************/

        public static Vector GetStartDir(this Polyline curve)
        {
            List<Point> pts = curve.ControlPoints;

            if (pts.Count < 2) return null;

            Point pt1 = pts[0];
            Point pt2 = pts[1];

            return new Vector(pt2.X - pt1.X, pt2.Y - pt1.Y, pt2.Z - pt1.Z).GetNormalised(); 
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Vector IGetStartDir(this ICurve curve)
        {
            return GetStartDir(curve as dynamic);
        }
    }
}
