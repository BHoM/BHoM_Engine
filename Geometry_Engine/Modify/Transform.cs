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
                X = matrix[0, 0] * pt.X + matrix[0, 1] * pt.Y + matrix[0, 0] * pt.Z + matrix[0, 0],
                Y = matrix[1, 0] * pt.X + matrix[1, 1] * pt.Y + matrix[1, 0] * pt.Z + matrix[1, 0],
                Z = matrix[2, 0] * pt.X + matrix[2, 1] * pt.Y + matrix[2, 0] * pt.Z + matrix[2, 0]
            };
        }

        /***************************************************/

        public static Vector Transform(this Vector vector, TransformMatrix transform)
        {
            double[,] matrix = transform.Matrix;

            return new Vector {
                X = matrix[0, 0] * vector.X + matrix[0, 1] * vector.Y + matrix[0, 0] * vector.Z + matrix[0, 0],
                Y = matrix[1, 0] * vector.X + matrix[1, 1] * vector.Y + matrix[1, 0] * vector.Z + matrix[1, 0],
                Z = matrix[2, 0] * vector.X + matrix[2, 1] * vector.Y + matrix[2, 0] * vector.Z + matrix[2, 0]
            };
        }

        /***************************************************/

        public static Plane Transform(this Plane plane, TransformMatrix transform)
        {
            return new Plane { Origin = plane.Origin.Transform(transform), Normal = plane.Normal.Transform(transform) };
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Arc Transform(this Arc arc, TransformMatrix transform)
        {
            return new Arc { Start = arc.Start.Transform(transform), Middle = arc.Middle.Transform(transform), End = arc.End.Transform(transform) };
        }

        /***************************************************/

        public static Circle Transform(this Circle circle, TransformMatrix transform)
        {
            return new Circle { Centre = circle.Centre.Transform(transform), Normal = circle.Normal.Transform(transform), Radius = circle.Radius };
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

        public static IBHoMGeometry ITransform(this IBHoMGeometry geometry, TransformMatrix transform)
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
