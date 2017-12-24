using BH.oM.Geometry;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static Plane Clone(this Plane plane)
        {
            return new Plane(plane.Origin.Clone(), plane.Normal.Clone());
        }

        /***************************************************/

        public static Point Clone(this Point point)
        {
            return new Point(point.X, point.Y, point.Z);
        }

        /***************************************************/

        public static Vector Clone(this Vector vector)
        {
            return new Vector(vector.X, vector.Y, vector.Z);
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Arc Clone(this Arc arc)
        {
            return new Arc { Start = arc.Start.Clone(), Middle = arc.Middle.Clone(), End = arc.End.Clone() };
        }

        /***************************************************/

        public static Circle Clone(this Circle circle)
        {
            return new Circle { Centre = circle.Centre.Clone(), Normal = circle.Normal.Clone(), Radius = circle.Radius };
        }

        /***************************************************/

        public static Line Clone(this Line line)
        {
            return new Line { Start = line.Start.Clone(), End = line.End.Clone() };
        }

        /***************************************************/

        public static NurbCurve Clone(this NurbCurve curve)
        {
            return new NurbCurve { ControlPoints = curve.ControlPoints.Select(x => x.Clone()).ToList(), Weights = curve.Weights.ToList(), Knots = curve.Knots.ToList() };
        }

        /***************************************************/

        public static PolyCurve Clone(this PolyCurve curve)
        {
            return new PolyCurve { Curves = curve.Curves.Select(x => x.IClone()).ToList() };
        }

        /***************************************************/

        public static Polyline Clone(this Polyline curve)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.Select(x => x.Clone()).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static Extrusion Clone(this Extrusion surface)
        {
            return new Extrusion(surface.Curve.IClone(), surface.Direction.Clone(), surface.Capped);
        }

        /***************************************************/

        public static Loft Clone(this Loft surface)
        {
            return new Loft(surface.Curves.Select(x => x.IClone()));
        }

        /***************************************************/

        public static NurbSurface Clone(this NurbSurface surface)
        {
            return new NurbSurface(surface.ControlPoints.Select(x => x.Clone()), surface.Weights.ToList(), surface.UKnots.ToList(), surface.VKnots.ToList());
        }

        /***************************************************/

        public static Pipe Clone(this Pipe surface)
        {
            return new Pipe(surface.Centreline.IClone(), surface.Radius, surface.Capped);
        }

        /***************************************************/

        public static PolySurface Clone(this PolySurface surface)
        {
            return new PolySurface(surface.Surfaces.Select(x => x.IClone()));
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static Mesh Clone(this Mesh mesh)
        {
            return new Mesh { Vertices = mesh.Vertices.Select(x => x.Clone()).ToList(), Faces = mesh.Faces.Select(x => x.Clone()).ToList() };
        }

        /***************************************************/

        public static Face Clone(this Face face)
        {
            return new Face { A = face.A, B = face.B, C = face.C, D = face.D };
        }

        /***************************************************/

        public static BoundingBox Clone(this BoundingBox box)
        {
            return new BoundingBox { Min = box.Min.Clone(), Max = box.Max.Clone() };
        }

        /***************************************************/

        public static CompositeGeometry Clone(this CompositeGeometry group)
        {
            return new CompositeGeometry { Elements = group.Elements.Select(x => x.IClone()).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IBHoMGeometry IClone(this IBHoMGeometry geometry)
        {
            return Clone(geometry as dynamic);
        }

        /***************************************************/

        public static ICurve IClone(this ICurve curve)
        {
            return Clone(curve as dynamic);
        }

        /***************************************************/

        public static ISurface IClone(this ISurface surface)
        {
            return Clone(surface as dynamic);
        }

        /***************************************************/
    }
}
