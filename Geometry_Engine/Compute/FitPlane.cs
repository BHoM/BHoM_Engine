using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** public Methods - Vectors                  ****/
        /***************************************************/

        public static Plane FitPlane(this IEnumerable<Point> points, double tolerance = Tolerance.Distance)
        {
            // Code from http://www.ilikebigbits.com/blog/2015/3/2/plane-from-points

            int n = points.Count();
            if (n < 3) return null;

            Point centroid = points.Average();

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

            double detX = yy * zz - yz * yz;
            double detY = xx * zz - xz * xz;
            double detZ = xx * yy - xy * xy;

            double det_max = Math.Max(Math.Max(detX, detY), detZ);
            if (det_max <= tolerance) //The points don't span a plane
                return null;

            // Pick path with best conditioning:
            Vector dir;
            if (det_max == detX)
            {
                double a = (xz * yz - xy * zz) / detX;
                double b = (xy * yz - xz * yy) / detX;
                dir = new Vector { X = 1.0, Y = a, Z = b };
            }
            else if (det_max == detY)
            {
                double a = (yz * xz - xy * zz) / detY;
                double b = (xy * xz - yz * xx) / detY;
                dir = new Vector { X = a, Y = 1.0, Z = b };
            }
            else
            {
                double a = (yz * xy - xz * yy) / detZ;
                double b = (xz * xy - yz * xx) / detZ;
                dir = new Vector { X = a, Y = b, Z = 1.0 };
            };

            return new Plane { Origin = centroid, Normal = dir.Normalise() };
        }


        /***************************************************/
        /**** public Methods - Curves                   ****/
        /***************************************************/

        public static Plane FitPlane(this Arc curve, double tolerance = Tolerance.Distance)
        {
            return (Plane)curve.CoordinateSystem;
        }

        /***************************************************/

        public static Plane FitPlane(this Circle curve, double tolerance = Tolerance.Distance)
        {
            return new Plane { Origin = curve.Centre, Normal = curve.Normal };
        }

        /***************************************************/

        public static Plane FitPlane(this Line curve, double tolerance = Tolerance.Distance)
        {
            return null;
        }

        /***************************************************/

        public static Plane FitPlane(this NurbCurve curve, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static Plane FitPlane(this PolyCurve curve, double tolerance = Tolerance.Distance)
        {
            return FitPlane(curve.Curves.SelectMany(x => x.IControlPoints()), tolerance);
        }

        /***************************************************/

        public static Plane FitPlane(this Polyline curve, double tolerance = Tolerance.Distance)
        {
            return FitPlane(curve.ControlPoints, tolerance);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Plane IFitPlane(this ICurve curve, double tolerance = Tolerance.Distance)
        {
            return FitPlane(curve as dynamic, tolerance);
        }

        /***************************************************/
    }
}
