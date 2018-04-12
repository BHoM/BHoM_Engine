using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static bool IsPlanar(this Point pt, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        public static bool IsPlanar(this Vector vector, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        public static bool IsPlanar(this Plane plane, double tolerance = Tolerance.Distance)
        {
            return true;
        }


        /***************************************************/
        /**** public Methods - Curves                   ****/
        /***************************************************/

        public static bool IsPlanar(this Line line, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        public static bool IsPlanar(this Arc arc, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        public static bool IsPlanar(this Circle circle, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        public static bool IsPlanar(this NurbCurve curve, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.IsCoplanar(tolerance);
        }

        /***************************************************/

        public static bool IsPlanar(this Polyline curve, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.IsCoplanar(tolerance);
        }

        /***************************************************/

        public static bool IsPlanar(this PolyCurve curve, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints().IsCoplanar(tolerance);
        }


        /***************************************************/
        /**** Public Methods - Surfaces                ****/
        /***************************************************/

        public static bool IsPlanar(this Extrusion surface, double tolerance = Tolerance.Distance)
        {
            return (surface.Direction.Length() <= tolerance || surface.Curve.IIsLinear());
        }

        /***************************************************/

        public static bool IsPlanar(this Loft surface, double tolerance = Tolerance.Distance)
        {
            List<Point> controlPts = new List<Point>();
            foreach (ICurve curve in surface.Curves)
            {
                controlPts.AddRange(curve.IControlPoints());
            }
            return controlPts.IsCoplanar(tolerance);
        }

        /***************************************************/

        public static bool IsPlanar(this NurbSurface surface, double tolerance = Tolerance.Distance)
        {
            return surface.ControlPoints.IsCoplanar(tolerance);
        }

        /***************************************************/

        public static bool IsPlanar(this Pipe surface, double tolerance = Tolerance.Distance)
        {
            return surface.Centreline.ILength() <= tolerance || surface.Radius == 0;
        }

        /***************************************************/

        public static bool IsPlanar(this PolySurface surface, double tolerance = Tolerance.Distance)
        {
            foreach (ISurface s in surface.Surfaces)
            {
                if (!s.IIsPlanar(tolerance)) return false;
            }
            return true;
        }


        /***************************************************/
        /**** Public Methods - Others                  ****/
        /***************************************************/

        public static bool IsPlanar(this Mesh mesh, double tolerance = Tolerance.Distance)
        {

            return mesh.Vertices.IsCoplanar(tolerance);
        }

        /***************************************************/

        public static bool IsPlanar(this CompositeGeometry group, double tolerance = Tolerance.Distance)
        {
            foreach (IGeometry element in group.Elements)
            {
                if (!element.IIsPlanar(tolerance)) return false;
            }
            return true;
        }


        /***************************************************/
        /**** Public Methods = Interfaces               ****/
        /***************************************************/


        public static bool IIsPlanar(this IGeometry geometry, double tolerance = Tolerance.Distance)
        {
            return IsPlanar(geometry as dynamic, tolerance);
        }
    }
}
