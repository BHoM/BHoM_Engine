using BH.oM.Geometry;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Transform
    {
        /***************************************************/
        /**** public Methods - Vectors                  ****/
        /***************************************************/

        public static Point GetMirrored(this Point pt, Plane p)
        {
            return pt - 2 * p.Normal.GetDotProduct(pt-p.Origin) * p.Normal;
        }

        /***************************************************/

        public static Vector GetMirrored(this Vector vector, Plane p)
        {
            return vector - 2 * vector.GetDotProduct(p.Normal) * p.Normal;
        }

        /***************************************************/

        public static Plane GetMirrored(this Plane plane, Plane p)
        {
            return new Plane(plane.Origin.GetMirrored(p), plane.Normal.GetMirrored(p));
        }


        /***************************************************/
        /**** public Methods - Curves                  ****/
        /***************************************************/

        public static Arc GetMirrored(this Arc arc, Plane p)
        {
            return new Arc(arc.Start.GetMirrored(p), arc.Middle.GetMirrored(p), arc.End.GetMirrored(p));
        }

        /***************************************************/

        public static Circle GetMirrored(this Circle circle, Plane p)
        {
            return new Circle(circle.Centre.GetMirrored(p), circle.Normal.GetMirrored(p), circle.Radius);
        }

        /***************************************************/

        public static Line GetMirrored(this Line line, Plane p)
        {
            return new Line(line.Start.GetMirrored(p), line.End.GetMirrored(p));
        }

        /***************************************************/

        public static NurbCurve GetMirrored(this NurbCurve curve, Plane p)
        {
            return new NurbCurve(curve.ControlPoints.Select(x => x.GetMirrored(p)), curve.Weights, curve.Knots);
        }


        /***************************************************/

        public static PolyCurve GetMirrored(this PolyCurve curve, Plane p)
        {
            return new PolyCurve(curve.Curves.Select(x => x.IGetMirrored(p)));
        }

        /***************************************************/

        public static Polyline GetMirrored(this Polyline curve, Plane p)
        {
            return new Polyline(curve.ControlPoints.Select(x => x.GetMirrored(p)));
        }


        /***************************************************/
        /**** public Methods - Surfaces                 ****/
        /***************************************************/

        public static Extrusion GetMirrored(this Extrusion surface, Plane p)
        {
            return new Extrusion(surface.Curve.IGetMirrored(p), surface.Direction.GetMirrored(p), surface.Capped);
        }

        /***************************************************/

        public static Loft GetMirrored(this Loft surface, Plane p)
        {
            return new Loft(surface.Curves.Select(x => x.IGetMirrored(p)));
        }

        /***************************************************/

        public static NurbSurface GetMirrored(this NurbSurface surface, Plane p)
        {
            return new NurbSurface(surface.ControlPoints.Select(x => x.GetMirrored(p)), surface.Weights, surface.UKnots, surface.VKnots);
        }

        /***************************************************/

        public static Pipe GetMirrored(this Pipe surface, Plane p)
        {
            return new Pipe(surface.Centreline.IGetMirrored(p), surface.Radius, surface.Capped);
        }

        /***************************************************/

        public static PolySurface GetMirrored(this PolySurface surface, Plane p)
        {
            return new PolySurface(surface.Surfaces.Select(x => x.IGetMirrored(p)));
        }


        /***************************************************/
        /**** public Methods - Others                   ****/
        /***************************************************/

        public static Mesh GetMirrored(this Mesh mesh, Plane p)
        {
            return new Mesh(mesh.Vertices.Select(x => x.GetMirrored(p)), mesh.Faces.Select(x => x.GetClone()));
        }

        /***************************************************/

        public static CompositeGeometry GetMirrored(this CompositeGeometry group, Plane p)
        {
            return new CompositeGeometry(group.Elements.Select(x => x.IGetMirrored(p)));
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IBHoMGeometry IGetMirrored(this IBHoMGeometry geometry, Plane p)
        {
            return GetMirrored(geometry as dynamic, p);
        }

        /***************************************************/

        public static ICurve IGetMirrored(this ICurve geometry, Plane p)
        {
            return GetMirrored(geometry as dynamic, p);
        }

        /***************************************************/

        public static ISurface IGetMirrored(this ISurface geometry, Plane p)
        {
            return GetMirrored(geometry as dynamic, p);
        }
    }
}
