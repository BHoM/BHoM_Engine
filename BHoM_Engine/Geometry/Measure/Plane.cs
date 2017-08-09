using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Measure
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Plane GetPlane(this ICurve curve)
        {
            return _GetPlane(curve as dynamic);
        }

        /***************************************************/

        public static Plane GetPlane(this IEnumerable<Point> points)
        {
            // Code from http://www.ilikebigbits.com/blog/2015/3/2/plane-from-points

            int n = points.Count();
            if (n < 3) return null;

            Point centroid = points.GetCentre();

            // Calc full 3x3 covariance matrix, excluding symmetries:
            double xx = 0.0; double xy = 0.0; double xz = 0.0;
            double yy = 0.0; double yz = 0.0; double zz = 0.0;

            foreach(Point p in points)
            {
                Vector r = p - centroid;
                xx += r.X * r.X;
                xy += r.X * r.Y;
                xz += r.X * r.Z;
                yy += r.Y * r.Y;
                yz += r.Y * r.Z;
                zz += r.Z * r.Z;
            }

            double det_x = yy * zz - yz * yz;
            double det_y = xx * zz - xz * xz;
            double det_z = xx * yy - xy * xy;

            double det_max = Math.Max(Math.Max(det_x, det_y), det_z);
            if (det_max <= 0.0) //The points don't span a plane
                return null;

            // Pick path with best conditioning:
            Vector dir;
            if (det_max == det_x)
            {
                double a = (xz * yz - xy * zz) / det_x;
                double b = (xy * yz - xz * yy) / det_x;
                dir = new Vector(1.0, a, b);
            }
            else if (det_max == det_y)
            {
                double a = (yz * xz - xy * zz) / det_y;
                double b = (xy * xz - yz * xx) / det_y;
                dir = new Vector(a, 1.0, b);
            }
            else
            {
                double a = (yz * xy - xz * yy) / det_z;
                double b = (xz * xy - yz * xx) / det_z;
                dir = new Vector(a, b, 1.0);
            };

            return new Plane(centroid, dir.GetNormalised());
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static Plane _GetPlane(this Arc curve)
        {
            return Create.CreatePlane(curve.Start, curve.Middle, curve.End);
        }

        /***************************************************/

        private static Plane _GetPlane(this Circle curve)
        {
            return new Plane(curve.Centre, curve.Normal);
        }

        /***************************************************/

        private static Plane _GetPlane(this Line curve)
        {
            return null;
        }

        /***************************************************/

        private static Plane _GetPlane(this NurbCurve curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static Plane _GetPlane(this PolyCurve curve)
        {
            return GetPlane(curve.Curves.SelectMany(x => x.GetControlPoints()));
        }

        /***************************************************/

        private static Plane _GetPlane(this Polyline curve)
        {
            return GetPlane(curve.ControlPoints);
        }
    }
}
