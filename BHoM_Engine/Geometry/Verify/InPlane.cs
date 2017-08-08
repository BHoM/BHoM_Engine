using BH.oM.Geometry;
using BHoM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Verify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsInPlane(this IBHoMGeometry geometry, Plane plane, double tolerance = Tolerance.Distance)
        {
            return _IsInPlane(geometry as dynamic, plane, tolerance);
        }

        /***************************************************/

        public static bool IsInPlane(this IEnumerable<Point> points, Plane plane, double tolerance = Tolerance.Distance)
        {
            Vector normal = plane.Normal;
            Point origin = plane.Origin;

            foreach (Point pt in points)
            {
                double d = normal.GetDotProduct(pt - origin);
                if (d < -tolerance && d > tolerance)
                    return false;
            }

            return true;
        }


        /***************************************************/
        /**** Private Methods - Vectors                 ****/
        /***************************************************/

        public static bool _IsInPlane(this Point pt, Plane plane, double tolerance = Tolerance.Distance)
        {
            double d = plane.Normal.GetDotProduct(pt - plane.Origin);
            return (d >= -tolerance && d <= tolerance);
        }

        /***************************************************/

        public static bool _IsInPlane(this Vector vector, Plane plane, double tolerance = Tolerance.Distance)
        {
            return Math.Abs(vector.GetDotProduct(plane.Normal)) < tolerance;
        }

        /***************************************************/

        public static bool _IsInPlane(this Plane plane1, Plane plane, double tolerance = Tolerance.Distance)
        {
            return 1 - Math.Abs(plane1.Normal.GetNormalised().GetDotProduct(plane.Normal.GetNormalised())) < tolerance;
        }


        /***************************************************/
        /**** Private Methods - Curves                  ****/
        /***************************************************/

        public static bool _IsInPlane(this Arc arc, Plane plane, double tolerance = Tolerance.Distance)
        {
            return arc.Start._IsInPlane(plane, tolerance) && arc.Middle._IsInPlane(plane, tolerance) && arc.End._IsInPlane(plane, tolerance);
        }

        /***************************************************/

        public static bool _IsInPlane(this Circle circle, Plane plane, double tolerance = Tolerance.Distance)
        {
            return Math.Abs(circle.Normal.GetDotProduct(plane.Normal)) < tolerance;
        }

        /***************************************************/

        public static bool _IsInPlane(this Line line, Plane plane, double tolerance = Tolerance.Distance)
        {
            return line.Start._IsInPlane(plane, tolerance) && line.End._IsInPlane(plane, tolerance);
        }

        /***************************************************/

        public static bool _IsInPlane(this NurbCurve curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.IsInPlane(plane, tolerance); //TODO: probably incorrect
        }


        /***************************************************/

        public static bool _IsInPlane(this PolyCurve curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            Vector normal = plane.Normal;
            Point origin = plane.Origin;

            foreach (ICurve c in curve.Curves)
            {
                if (!c.IsInPlane(plane, tolerance))
                    return false;
            }

            return true;
        }

        /***************************************************/

        public static bool _IsInPlane(this Polyline curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.IsInPlane(plane, tolerance);
        }


        /***************************************************/
        /**** Private Methods - Surfaces                ****/
        /***************************************************/

        public static bool _IsInPlane(this Extrusion surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            return surface.Direction._IsInPlane(plane, tolerance) && surface.Curve.IsInPlane(plane, tolerance);
        }

        /***************************************************/

        public static bool _IsInPlane(this Loft surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            Vector normal = plane.Normal;
            Point origin = plane.Origin;

            foreach (ICurve c in surface.Curves)
            {
                if (!c.IsInPlane(plane, tolerance))
                    return false;
            }

            return true;
        }

        /***************************************************/

        public static bool _IsInPlane(this NurbSurface surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            return surface.ControlPoints.IsInPlane(plane, tolerance); //TODO: probably incorrect
        }

        /***************************************************/

        public static bool _IsInPlane(this Pipe surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        public static bool _IsInPlane(this PolySurface surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            Vector normal = plane.Normal;
            Point origin = plane.Origin;

            foreach (ISurface s in surface.Surfaces)
            {
                if (!s.IsInPlane(plane, tolerance))
                    return false;
            }

            return true;
        }


        /***************************************************/
        /**** Private Methods - Others                  ****/
        /***************************************************/

        public static bool _IsInPlane(this Mesh mesh, Plane plane, double tolerance = Tolerance.Distance)
        {
            return mesh.Vertices.IsInPlane(plane, tolerance);
        }

        /***************************************************/

        public static bool _IsInPlane(this GeometryGroup group, Plane plane, double tolerance = Tolerance.Distance)
        {
            Vector normal = plane.Normal;
            Point origin = plane.Origin;

            foreach (IBHoMGeometry g in group.Elements)
            {
                if (!g.IsInPlane(plane, tolerance))
                    return false;
            }

            return true;
        }
    }
}
