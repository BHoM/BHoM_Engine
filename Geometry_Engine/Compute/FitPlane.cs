using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** public Methods - Vectors                 ****/
        /***************************************************/

        public static Plane FitPlane(this IEnumerable<Point> points)
        {
            // Code from http://www.ilikebigbits.com/blog/2015/3/2/plane-from-points

            int n = points.Count();
            if (n < 3) return null;

            Point centroid = points.Centre();

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
                dir = new Vector { X = 1.0, Y = a, Z = b };
            }
            else if (det_max == det_y)
            {
                double a = (yz * xz - xy * zz) / det_y;
                double b = (xy * xz - yz * xx) / det_y;
                dir = new Vector { X = a, Y = 1.0, Z = b };
            }
            else
            {
                double a = (yz * xy - xz * yy) / det_z;
                double b = (xz * xy - yz * xx) / det_z;
                dir = new Vector { X = a, Y = b, Z = 1.0 };
            };

            return new Plane { Origin = centroid, Normal = dir.Normalise() };
        }


        /***************************************************/
        /**** public Methods - Curves                  ****/
        /***************************************************/

        public static Plane FitPlane(this Arc curve)
        {
            return Create.Plane(curve.Start, curve.Middle, curve.End);
        }

        /***************************************************/

        public static Plane FitPlane(this Circle curve)
        {
            return new Plane { Origin = curve.Centre, Normal = curve.Normal };
        }

        /***************************************************/

        public static Plane FitPlane(this Line curve)
        {
            return null;
        }

        /***************************************************/

        public static Plane FitPlane(this NurbCurve curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static Plane FitPlane(this PolyCurve curve)
        {
            return FitPlane(curve.Curves.SelectMany(x => x.IControlPoints()));
        }

        /***************************************************/

        public static Plane FitPlane(this Polyline curve)
        {
            return FitPlane(curve.ControlPoints);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Plane IFitPlane(this ICurve curve)
        {
            return FitPlane(curve as dynamic);
        }

        /***************************************************/
    }
}
