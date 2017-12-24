using BH.oM.Geometry;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** public Methods - Vectors                  ****/
        /***************************************************/

        public static Point Mirror(this Point pt, Plane p)
        {
            return pt - 2 * p.Normal.DotProduct(pt-p.Origin) * p.Normal;
        }

        /***************************************************/

        public static Vector Mirror(this Vector vector, Plane p)
        {
            return vector - 2 * vector.DotProduct(p.Normal) * p.Normal;
        }

        /***************************************************/

        public static Plane Mirror(this Plane plane, Plane p)
        {
            return new Plane(plane.Origin.Mirror(p), plane.Normal.Mirror(p));
        }


        /***************************************************/
        /**** public Methods - Curves                  ****/
        /***************************************************/

        public static Arc Mirror(this Arc arc, Plane p)
        {
            return new Arc { Start = arc.Start.Mirror(p), Middle = arc.Middle.Mirror(p), End = arc.End.Mirror(p) };
        }

        /***************************************************/

        public static Circle Mirror(this Circle circle, Plane p)
        {
            return new Circle { Centre = circle.Centre.Mirror(p), Normal = circle.Normal.Mirror(p), Radius = circle.Radius };
        }

        /***************************************************/

        public static Line Mirror(this Line line, Plane p)
        {
            return new Line { Start = line.Start.Mirror(p), End = line.End.Mirror(p) };
        }

        /***************************************************/

        public static NurbCurve Mirror(this NurbCurve curve, Plane p)
        {
            return new NurbCurve { ControlPoints = curve.ControlPoints.Select(x => x.Mirror(p)).ToList(), Weights = curve.Weights.ToList(), Knots = curve.Knots.ToList() };
        }


        /***************************************************/

        public static PolyCurve Mirror(this PolyCurve curve, Plane p)
        {
            return new PolyCurve { Curves = curve.Curves.Select(x => x.IMirror(p)).ToList() };
        }

        /***************************************************/

        public static Polyline Mirror(this Polyline curve, Plane p)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.Select(x => x.Mirror(p)).ToList() };
        }


        /***************************************************/
        /**** public Methods - Surfaces                 ****/
        /***************************************************/

        public static Extrusion Mirror(this Extrusion surface, Plane p)
        {
            return new Extrusion(surface.Curve.IMirror(p), surface.Direction.Mirror(p), surface.Capped);
        }

        /***************************************************/

        public static Loft Mirror(this Loft surface, Plane p)
        {
            return new Loft(surface.Curves.Select(x => x.IMirror(p)));
        }

        /***************************************************/

        public static NurbSurface Mirror(this NurbSurface surface, Plane p)
        {
            return new NurbSurface(surface.ControlPoints.Select(x => x.Mirror(p)), surface.Weights, surface.UKnots, surface.VKnots);
        }

        /***************************************************/

        public static Pipe Mirror(this Pipe surface, Plane p)
        {
            return new Pipe(surface.Centreline.IMirror(p), surface.Radius, surface.Capped);
        }

        /***************************************************/

        public static PolySurface Mirror(this PolySurface surface, Plane p)
        {
            return new PolySurface(surface.Surfaces.Select(x => x.IMirror(p)));
        }


        /***************************************************/
        /**** public Methods - Others                   ****/
        /***************************************************/

        public static Mesh Mirror(this Mesh mesh, Plane p)
        {
            return new Mesh { Vertices = mesh.Vertices.Select(x => x.Mirror(p)).ToList(), Faces = mesh.Faces.Select(x => x.Clone()).ToList() };
        }

        /***************************************************/

        public static CompositeGeometry Mirror(this CompositeGeometry group, Plane p)
        {
            return new CompositeGeometry { Elements = group.Elements.Select(x => x.IMirror(p)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IBHoMGeometry IMirror(this IBHoMGeometry geometry, Plane p)
        {
            return Mirror(geometry as dynamic, p);
        }

        /***************************************************/

        public static ICurve IMirror(this ICurve geometry, Plane p)
        {
            return Mirror(geometry as dynamic, p);
        }

        /***************************************************/

        public static ISurface IMirror(this ISurface geometry, Plane p)
        {
            return Mirror(geometry as dynamic, p);
        }

        /***************************************************/
    }
}
