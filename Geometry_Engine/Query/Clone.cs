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
            return new Plane { Origin = plane.Origin.Clone(), Normal = plane.Normal.Clone() };
        }

        /***************************************************/

        public static Point Clone(this Point point)
        {
            return new Point { X = point.X, Y = point.Y, Z = point.Z };
        }

        /***************************************************/

        public static Vector Clone(this Vector vector)
        {
            return new Vector { X = vector.X, Y = vector.Y, Z = vector.Z };
        }

        /***************************************************/

        public static CoordinateSystem Clone(this CoordinateSystem coordinateSystem)
        {
            return new CoordinateSystem
            {
                X = coordinateSystem.X.Clone(),
                Y = coordinateSystem.Y.Clone(),
                Z = coordinateSystem.Z.Clone(),
                Origin = coordinateSystem.Origin.Clone()
            };
        }

        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Arc Clone(this Arc arc)
        {
            return new Arc { CoordinateSystem = arc.CoordinateSystem.Clone(), StartAngle = arc.StartAngle, EndAngle = arc.EndAngle, Radius = arc.Radius };
        }

        /***************************************************/

        public static Circle Clone(this Circle circle)
        {
            return new Circle { Centre = circle.Centre.Clone(), Normal = circle.Normal.Clone(), Radius = circle.Radius };
        }

        /***************************************************/

        public static Line Clone(this Line line)
        {
            return new Line { Start = line.Start.Clone(), End = line.End.Clone(), Infinite = line.Infinite };
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
            return new Extrusion { Curve = surface.Curve.IClone(), Direction = surface.Direction.Clone(), Capped = surface.Capped };
        }

        /***************************************************/

        public static Loft Clone(this Loft surface)
        {
            return new Loft { Curves = surface.Curves.Select(x => x.IClone()).ToList() };
        }

        /***************************************************/

        public static NurbSurface Clone(this NurbSurface surface)
        {
            return new NurbSurface { ControlPoints = surface.ControlPoints.Select(x => x.Clone()).ToList(), Weights = surface.Weights.ToList(), UKnots = surface.UKnots.ToList(), VKnots = surface.VKnots.ToList() };
        }

        /***************************************************/

        public static Pipe Clone(this Pipe surface)
        {
            return new Pipe { Centreline = surface.Centreline.IClone(), Radius = surface.Radius, Capped = surface.Capped };
        }

        /***************************************************/

        public static PolySurface Clone(this PolySurface surface)
        {
            return new PolySurface { Surfaces = surface.Surfaces.Select(x => x.IClone()).ToList() };
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

        public static IGeometry IClone(this IGeometry geometry)
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
