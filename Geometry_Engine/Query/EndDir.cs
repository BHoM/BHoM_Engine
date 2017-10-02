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
        /**** public Methods - Curves                   ****/
        /***************************************************/

        public static Vector GetEndDir(this Arc arc)
        {
            throw new NotImplementedException(); //TODO: get End dir of arc
        }

        /***************************************************/

        public static Vector GetEndDir(this Circle circle)
        {
            Vector n = circle.Normal;
            Vector endDir = Math.Abs(n.Z) < Math.Abs(n.X) ? new Vector(n.Y, -n.X, 0) : new Vector(0, n.Z, -n.Y);
            return circle.Normal.GetCrossProduct(endDir).GetNormalised();
        }

        /***************************************************/

        public static Vector GetEndDir(this Line line)
        {
            return new Vector(line.End.X - line.Start.X, line.End.Y - line.Start.Y, line.End.Z - line.Start.Z).GetNormalised();
        }

        /***************************************************/

        public static Vector GetEndDir(this NurbCurve curve)
        {
            throw new NotImplementedException(); //TODO: get End dir of nurbcurve
        }

        /***************************************************/

        public static Vector GetEndDir(this PolyCurve curve)
        {
            return curve.Curves.Count > 0 ? curve.Curves.Last().IGetEndDir() : null;
        }

        /***************************************************/

        public static Vector GetEndDir(this Polyline curve)
        {
            List<Point> pts = curve.ControlPoints;

            if (pts.Count < 2) return null;

            Point pt1 = pts[pts.Count - 2];
            Point pt2 = pts[pts.Count - 1];

            return new Vector(pt2.X - pt1.X, pt2.Y - pt1.Y, pt2.Z - pt1.Z).GetNormalised();
        }


        /***************************************************/
        /**** Public Methods = Interfaces               ****/
        /***************************************************/

        public static Vector IGetEndDir(this ICurve curve)
        {
            return GetEndDir(curve as dynamic);
        }
    }
}
