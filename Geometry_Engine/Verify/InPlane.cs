using BH.oM.Geometry;
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
        /**** Public Methods - Vectors                  ****/
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

        public static bool IsInPlane(this Point pt, Plane plane, double tolerance = Tolerance.Distance)
        {
            double d = plane.Normal.GetDotProduct(pt - plane.Origin);
            return (d >= -tolerance && d <= tolerance);
        }

        /***************************************************/

        public static bool IsInPlane(this Vector vector, Plane plane, double tolerance = Tolerance.Distance)
        {
            return Math.Abs(vector.GetDotProduct(plane.Normal)) < tolerance;
        }

        /***************************************************/

        public static bool IsInPlane(this Plane plane1, Plane plane, double tolerance = Tolerance.Distance)
        {
            return 1 - Math.Abs(plane1.Normal.GetNormalised().GetDotProduct(plane.Normal.GetNormalised())) < tolerance;
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static bool IsInPlane(this Arc arc, Plane plane, double tolerance = Tolerance.Distance)
        {
            return arc.Start.IsInPlane(plane, tolerance) && arc.Middle.IsInPlane(plane, tolerance) && arc.End.IsInPlane(plane, tolerance);
        }

        /***************************************************/

        public static bool IsInPlane(this Circle circle, Plane plane, double tolerance = Tolerance.Distance)
        {
            return Math.Abs(circle.Normal.GetDotProduct(plane.Normal)) < tolerance && Math.Abs(plane.Normal.GetDotProduct(circle.Centre - plane.Origin)) < tolerance;
        }

        /***************************************************/

        public static bool IsInPlane(this Line line, Plane plane, double tolerance = Tolerance.Distance)
        {
            return line.Start.IsInPlane(plane, tolerance) && line.End.IsInPlane(plane, tolerance);
        }

        /***************************************************/

        public static bool IsInPlane(this NurbCurve curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.IsInPlane(plane, tolerance); //TODO: probably incorrect
        }


        /***************************************************/

        public static bool IsInPlane(this PolyCurve curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            Vector normal = plane.Normal;
            Point origin = plane.Origin;

            foreach (ICurve c in curve.Curves)
            {
                if (!c.IIsInPlane(plane, tolerance))
                    return false;
            }

            return true;
        }

        /***************************************************/

        public static bool IsInPlane(this Polyline curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.IsInPlane(plane, tolerance);
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static bool IsInPlane(this Extrusion surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            return surface.Direction.IsInPlane(plane, tolerance) && surface.Curve.IIsInPlane(plane, tolerance);
        }

        /***************************************************/

        public static bool IsInPlane(this Loft surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            Vector normal = plane.Normal;
            Point origin = plane.Origin;

            foreach (ICurve c in surface.Curves)
            {
                if (!c.IIsInPlane(plane, tolerance))
                    return false;
            }

            return true;
        }

        /***************************************************/

        public static bool IsInPlane(this NurbSurface surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            return surface.ControlPoints.IsInPlane(plane, tolerance); //TODO: probably incorrect
        }

        /***************************************************/

        public static bool IsInPlane(this Pipe surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        public static bool IsInPlane(this PolySurface surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            Vector normal = plane.Normal;
            Point origin = plane.Origin;

            foreach (ISurface s in surface.Surfaces)
            {
                if (!s.IIsInPlane(plane, tolerance))
                    return false;
            }

            return true;
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static bool IsInPlane(this Mesh mesh, Plane plane, double tolerance = Tolerance.Distance)
        {
            return mesh.Vertices.IsInPlane(plane, tolerance);
        }

        /***************************************************/

        public static bool IsInPlane(this CompositeGeometry group, Plane plane, double tolerance = Tolerance.Distance)
        {
            Vector normal = plane.Normal;
            Point origin = plane.Origin;

            foreach (IBHoMGeometry g in group.Elements)
            {
                if (!g.IIsInPlane(plane, tolerance))
                    return false;
            }

            return true;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static bool IIsInPlane(this IBHoMGeometry geometry, Plane plane, double tolerance = Tolerance.Distance)
        {
            return IsInPlane(geometry as dynamic, plane, tolerance);
        }
    }
}
