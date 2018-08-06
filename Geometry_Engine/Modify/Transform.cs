using BH.oM.Geometry;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static Point Transform(this Point pt, TransformMatrix transform)
        {
            double[,] matrix = transform.Matrix;

            return new Point {
                X = matrix[0, 0] * pt.X + matrix[0, 1] * pt.Y + matrix[0, 2] * pt.Z + matrix[0, 3],
                Y = matrix[1, 0] * pt.X + matrix[1, 1] * pt.Y + matrix[1, 2] * pt.Z + matrix[1, 3],
                Z = matrix[2, 0] * pt.X + matrix[2, 1] * pt.Y + matrix[2, 2] * pt.Z + matrix[2, 3]
            };
        }

        /***************************************************/

        public static Vector Transform(this Vector vector, TransformMatrix transform)
        {
            double[,] matrix = transform.Matrix;

            return new Vector {
                X = matrix[0, 0] * vector.X + matrix[0, 1] * vector.Y + matrix[0, 2] * vector.Z + matrix[0, 3],
                Y = matrix[1, 0] * vector.X + matrix[1, 1] * vector.Y + matrix[1, 2] * vector.Z + matrix[1, 3],
                Z = matrix[2, 0] * vector.X + matrix[2, 1] * vector.Y + matrix[2, 2] * vector.Z + matrix[2, 3]
            };
        }

        /***************************************************/

        public static Plane Transform(this Plane plane, TransformMatrix transform)
        {
            return new Plane { Origin = plane.Origin.Transform(transform), Normal = plane.Normal.Transform(transform) };
        }

        /***************************************************/

        public static CoordinateSystem Transform(this CoordinateSystem coordinateSystem, TransformMatrix transform)
        {
            //TODO: This could create a non-cartesian coordinate system. WOuld that be acceptable..?
            return new CoordinateSystem
            {
                X = coordinateSystem.X.Transform(transform),
                Y = coordinateSystem.Y.Transform(transform),
                Z = coordinateSystem.Z.Transform(transform),
                Origin = coordinateSystem.Origin.Transform(transform)
            };
        }

        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Arc Transform(this Arc arc, TransformMatrix transform)
        {
            return new Arc { CoordinateSystem = arc.CoordinateSystem.Transform(transform), StartAngle = arc.StartAngle, EndAngle = arc.EndAngle, Radius = arc.Radius };
        }

        /***************************************************/

        public static Circle Transform(this Circle circle, TransformMatrix transform)
        {
            return new Circle { Centre = circle.Centre.Transform(transform), Normal = circle.Normal.Transform(transform), Radius = circle.Radius };
        }

        /***************************************************/

        public static Ellipse Transform(this Ellipse ellipse, TransformMatrix transform)
        {
            return new Ellipse { Centre = ellipse.Centre.Transform(transform), Axis1 = ellipse.Axis1.Transform(transform).Normalise(), Axis2 = ellipse.Axis2.Transform(transform).Normalise(), Radius1 = ellipse.Radius1, Radius2 = ellipse.Radius2 };
        }

        /***************************************************/

        public static Line Transform(this Line line, TransformMatrix transform)
        {
            return new Line { Start = line.Start.Transform(transform), End = line.End.Transform(transform) };
        }

        /***************************************************/

        public static NurbCurve Transform(this NurbCurve curve, TransformMatrix transform)
        {
            return new NurbCurve { ControlPoints = curve.ControlPoints.Select(x => x.Transform(transform)).ToList(), Weights = curve.Weights.ToList(), Knots = curve.Knots.ToList() };
        }


        /***************************************************/

        public static PolyCurve Transform(this PolyCurve curve, TransformMatrix transform)
        {
            return new PolyCurve { Curves = curve.Curves.Select(x => x.ITransform(transform)).ToList() };
        }

        /***************************************************/

        public static Polyline Transform(this Polyline curve, TransformMatrix transform)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.Select(x => x.Transform(transform)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static Extrusion Transform(this Extrusion surface, TransformMatrix transform)
        {
            return new Extrusion { Curve = surface.Curve.ITransform(transform), Direction = surface.Direction.Transform(transform), Capped = surface.Capped };
        }

        /***************************************************/

        public static Loft Transform(this Loft surface, TransformMatrix transform)
        {
            return new Loft { Curves = surface.Curves.Select(x => x.ITransform(transform)).ToList() };
        }

        /***************************************************/

        public static NurbSurface Transform(this NurbSurface surface, TransformMatrix transform)
        {
            return new NurbSurface { ControlPoints = surface.ControlPoints.Select(x => x.Transform(transform)).ToList(), Weights = surface.Weights.ToList(), UKnots = surface.UKnots.ToList(), VKnots = surface.VKnots.ToList() };
        }

        /***************************************************/

        public static Pipe Transform(this Pipe surface, TransformMatrix transform)
        {
            return new Pipe { Centreline = surface.Centreline.ITransform(transform), Radius = surface.Radius, Capped = surface.Capped };
        }

        /***************************************************/

        public static PolySurface Transform(this PolySurface surface, TransformMatrix transform)
        {
            return new PolySurface { Surfaces = surface.Surfaces.Select(x => x.ITransform(transform)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static Mesh Transform(this Mesh mesh, TransformMatrix transform)
        {
            return new Mesh { Vertices = mesh.Vertices.Select(x => x.Transform(transform)).ToList(), Faces = mesh.Faces.Select(x => x.Clone() as Face).ToList() };
        }

        /***************************************************/

        public static CompositeGeometry Transform(this CompositeGeometry group, TransformMatrix transform)
        {
            return new CompositeGeometry { Elements = group.Elements.Select(x => x.ITransform(transform)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IGeometry ITransform(this IGeometry geometry, TransformMatrix transform)
        {
            return Transform(geometry as dynamic, transform);
        }

        /***************************************************/

        public static ICurve ITransform(this ICurve geometry, TransformMatrix transform)
        {
            return Transform(geometry as dynamic, transform);
        }

        /***************************************************/

        public static ISurface ITransform(this ISurface geometry, TransformMatrix transform)
        {
            return Transform(geometry as dynamic, transform);
        }

        /***************************************************/
    }
}
