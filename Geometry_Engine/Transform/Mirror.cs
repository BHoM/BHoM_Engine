using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class p
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IBHoMGeometry GetMirrored(this IBHoMGeometry geometry, Plane p)
        {
            return _GetMirrored(geometry as dynamic, p);
        }


        /***************************************************/
        /**** Private Methods - Vectors                 ****/
        /***************************************************/

        private static Point _GetMirrored(this Point pt, Plane p)
        {
            return pt - 2 * p.Normal.GetDotProduct(pt-p.Origin) * p.Normal;
        }

        /***************************************************/

        private static Vector _GetMirrored(this Vector vector, Plane p)
        {
            return vector - 2 * vector.GetDotProduct(p.Normal) * p.Normal;
        }

        /***************************************************/

        private static Plane _GetMirrored(this Plane plane, Plane p)
        {
            return new Plane(plane.Origin._GetMirrored(p), plane.Normal._GetMirrored(p));
        }


        /***************************************************/
        /**** Private Methods - Curves                  ****/
        /***************************************************/

        private static Arc _GetMirrored(this Arc arc, Plane p)
        {
            return new Arc(arc.Start._GetMirrored(p), arc.Middle._GetMirrored(p), arc.End._GetMirrored(p));
        }

        /***************************************************/

        private static Circle _GetMirrored(this Circle circle, Plane p)
        {
            return new Circle(circle.Centre._GetMirrored(p), circle.Normal._GetMirrored(p), circle.Radius);
        }

        /***************************************************/

        private static Line _GetMirrored(this Line line, Plane p)
        {
            return new Line(line.Start._GetMirrored(p), line.End._GetMirrored(p));
        }

        /***************************************************/

        private static NurbCurve _GetMirrored(this NurbCurve curve, Plane p)
        {
            return new NurbCurve(curve.ControlPoints.Select(x => x._GetMirrored(p)), curve.Weights, curve.Knots);
        }


        /***************************************************/

        private static PolyCurve _GetMirrored(this PolyCurve curve, Plane p)
        {
            return new PolyCurve(curve.Curves.Select(x => x.GetMirrored(p) as ICurve));
        }

        /***************************************************/

        private static Polyline _GetMirrored(this Polyline curve, Plane p)
        {
            return new Polyline(curve.ControlPoints.Select(x => x._GetMirrored(p)));
        }


        /***************************************************/
        /**** Private Methods - Surfaces                ****/
        /***************************************************/

        private static Extrusion _GetMirrored(this Extrusion surface, Plane p)
        {
            return new Extrusion(surface.Curve.GetMirrored(p) as ICurve, surface.Direction._GetMirrored(p), surface.Capped);
        }

        /***************************************************/

        private static Loft _GetMirrored(this Loft surface, Plane p)
        {
            return new Loft(surface.Curves.Select(x => x.GetMirrored(p) as ICurve));
        }

        /***************************************************/

        private static NurbSurface _GetMirrored(this NurbSurface surface, Plane p)
        {
            return new NurbSurface(surface.ControlPoints.Select(x => x._GetMirrored(p)), surface.Weights, surface.UKnots, surface.VKnots);
        }

        /***************************************************/

        private static Pipe _GetMirrored(this Pipe surface, Plane p)
        {
            return new Pipe(surface.Centreline.GetMirrored(p) as ICurve, surface.Radius, surface.Capped);
        }

        /***************************************************/

        private static PolySurface _GetMirrored(this PolySurface surface, Plane p)
        {
            return new PolySurface(surface.Surfaces.Select(x => x.GetMirrored(p) as ISurface));
        }


        /***************************************************/
        /**** Private Methods - Others                  ****/
        /***************************************************/

        private static Mesh _GetMirrored(this Mesh mesh, Plane p)
        {
            return new Mesh(mesh.Vertices.Select(x => x._GetMirrored(p)), mesh.Faces.Select(x => x.GetClone() as Face));
        }

        /***************************************************/

        private static GeometryGroup _GetMirrored(this GeometryGroup group, Plane p)
        {
            return new GeometryGroup(group.Elements.Select(x => x.GetMirrored(p)));
        }
    }
}
