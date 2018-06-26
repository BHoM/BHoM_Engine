using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** public Methods - Curves                   ****/
        /***************************************************/

        public static Vector EndDir(this Arc arc)
        {
            return arc.CoordinateSystem.Y.Rotate(arc.EndAngle, arc.CoordinateSystem.Z);
        }

        /***************************************************/

        public static Vector EndDir(this Circle circle)
        {
            Vector n = circle.Normal;
            Vector endDir = Math.Abs(n.Z) < Math.Abs(n.X) ? new Vector { X = n.Y, Y = -n.X, Z = 0 } : new Vector { X = 0, Y = n.Z, Z = -n.Y };
            return circle.Normal.CrossProduct(endDir).Normalise();
        }

        /***************************************************/

        public static Vector EndDir(this Line line)
        {
            return new Vector { X = line.End.X - line.Start.X, Y = line.End.Y - line.Start.Y, Z = line.End.Z - line.Start.Z }.Normalise();
        }

        /***************************************************/

        public static Vector EndDir(this NurbCurve curve)
        {
            throw new NotImplementedException(); //TODO: get End dir of nurbcurve
        }

        /***************************************************/

        public static Vector EndDir(this PolyCurve curve)
        {
            return curve.Curves.Count > 0 ? curve.Curves.Last().IEndDir() : null;
        }

        /***************************************************/

        public static Vector EndDir(this Polyline curve)
        {
            List<Point> pts = curve.ControlPoints;

            if (pts.Count < 2) return null;

            Point pt1 = pts[pts.Count - 2];
            Point pt2 = pts[pts.Count - 1];

            return new Vector { X = pt2.X - pt1.X, Y = pt2.Y - pt1.Y, Z = pt2.Z - pt1.Z }.Normalise();
        }


        /***************************************************/
        /**** Public Methods = Interfaces               ****/
        /***************************************************/

        public static Vector IEndDir(this ICurve curve)
        {
            return EndDir(curve as dynamic);
        }
    }
}
