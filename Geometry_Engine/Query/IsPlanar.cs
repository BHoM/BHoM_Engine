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

        public static bool IsPlanar(this Point pt)
        {
            return true;
        }

        /***************************************************/

        public static bool IsPlanar(this Vector vector)
        {
            return true;
        }

        /***************************************************/

        public static bool IsPlanar(this Plane plane)
        {
            return true;
        }


        /***************************************************/
        /**** public Methods - Curves                   ****/
        /***************************************************/

        public static bool IsPlanar(this Line line)
        {
            return true;
        }

        /***************************************************/

        public static bool IsPlanar(this Arc arc)
        {
            return true;
        }

        /***************************************************/

        public static bool IsPlanar(this Circle circle)
        {
            return true;
        }

        /***************************************************/

        public static bool IsPlanar(this NurbCurve curve)
        {
            return curve.ControlPoints.IsCoplanar();
        }

        /***************************************************/

        public static bool IsPlanar(this Polyline curve)
        {
            return curve.ControlPoints.IsCoplanar();
        }

        /***************************************************/

        public static bool IsPlanar(this PolyCurve curve)
        {
            return curve.ControlPoints().IsCoplanar();
        }


        /***************************************************/
        /**** Public Methods - Surfaces                ****/
        /***************************************************/

        public static bool IsPlanar(this Extrusion surface)
        {
            return (surface.Direction.Length() <= Tolerance.Distance || surface.Curve.IIsLinear());
        }

        /***************************************************/

        public static bool IsPlanar(this Loft surface)
        {
            List<Point> controlPts = new List<Point>();
            foreach (ICurve curve in surface.Curves)
            {
                controlPts.AddRange(curve.IControlPoints());
            }
            return controlPts.IsCoplanar();
        }

        /***************************************************/

        public static bool IsPlanar(this NurbSurface surface)
        {
            return surface.ControlPoints.IsCoplanar();
        }

        /***************************************************/

        public static bool IsPlanar(this Pipe surface)
        {
            return surface.Centreline.ILength() <= Tolerance.Distance || surface.Radius == 0;
        }

        /***************************************************/

        public static bool IsPlanar(this PolySurface surface)
        {
            foreach (ISurface s in surface.Surfaces)
            {
                if (!s.IIsPlanar()) return false;
            }
            return true;
        }


        /***************************************************/
        /**** Public Methods - Others                  ****/
        /***************************************************/

        public static bool IsPlanar(this Mesh mesh)
        {

            return mesh.Vertices.IsCoplanar();
        }

        /***************************************************/

        public static bool IsPlanar(this CompositeGeometry group)
        {
            foreach (IBHoMGeometry element in group.Elements)
            {
                if (!element.IIsPlanar()) return false;
            }
            return true;
        }


        /***************************************************/
        /**** Public Methods = Interfaces               ****/
        /***************************************************/


        public static bool IIsPlanar(this IBHoMGeometry geometry)
        {
            return IsPlanar(geometry as dynamic);
        }
    }
}
