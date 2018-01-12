using BH.oM.Geometry;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static Point Translate(this Point pt, Vector transform)
        {
            return new Point { X = pt.X + transform.X, Y = pt.Y + transform.Y, Z = pt.Z + transform.Z };
        }

        /***************************************************/

        public static Vector Translate(this Vector vector, Vector transform)
        {
            return new Vector { X = vector.X, Y = vector.Y, Z = vector.Z };
        }

        /***************************************************/

        public static Plane Translate(this Plane plane, Vector transform)
        {
            return new Plane { Origin = plane.Origin + transform, Normal = plane.Normal.Clone() as Vector };
        }


        /***************************************************/
        /**** Public Methods - Curves                  ****/
        /***************************************************/

        public static Arc Translate(this Arc arc, Vector transform)
        {
            return new Arc { Start = arc.Start + transform, Middle = arc.Middle + transform, End = arc.End + transform };
        }

        /***************************************************/

        public static Circle Translate(this Circle circle, Vector transform)
        {
            return new Circle { Centre = circle.Centre + transform, Normal = circle.Normal.Clone() as Vector, Radius = circle.Radius };
        }

        /***************************************************/

        public static Line Translate(this Line line, Vector transform)
        {
            return new Line { Start = line.Start + transform, End = line.End + transform };
        }

        /***************************************************/

        public static NurbCurve Translate(this NurbCurve curve, Vector transform)
        {
            return new NurbCurve { ControlPoints = curve.ControlPoints.Select(x => x + transform).ToList(), Weights = curve.Weights.ToList(), Knots = curve.Knots.ToList() };
        }


        /***************************************************/

        public static PolyCurve Translate(this PolyCurve curve, Vector transform)
        {
            return new PolyCurve { Curves = curve.Curves.Select(x => x.ITranslate(transform)).ToList() };
        }

        /***************************************************/

        public static Polyline Translate(this Polyline curve, Vector transform)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.Select(x => x + transform).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Surfaces                ****/
        /***************************************************/

        public static Extrusion Translate(this Extrusion surface, Vector transform)
        {
            return new Extrusion { Curve = surface.Curve.ITranslate(transform), Direction = surface.Direction.Clone() as Vector, Capped = surface.Capped };
        }

        /***************************************************/

        public static Loft Translate(this Loft surface, Vector transform)
        {
            return new Loft { Curves = surface.Curves.Select(x => x.ITranslate(transform)).ToList() };
        }

        /***************************************************/

        public static NurbSurface Translate(this NurbSurface surface, Vector transform)
        {
            return new NurbSurface { ControlPoints = surface.ControlPoints.Select(x => x + transform).ToList(), Weights = surface.Weights.ToList(), UKnots = surface.UKnots.ToList(), VKnots = surface.VKnots.ToList() };
        }

        /***************************************************/

        public static Pipe Translate(this Pipe surface, Vector transform)
        {
            return new Pipe { Centreline = surface.Centreline.ITranslate(transform), Radius = surface.Radius, Capped = surface.Capped };
        }

        /***************************************************/

        public static PolySurface Translate(this PolySurface surface, Vector transform)
        {
            return new PolySurface { Surfaces = surface.Surfaces.Select(x => x.ITranslate(transform)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Others                  ****/
        /***************************************************/

        public static Mesh Translate(this Mesh mesh, Vector transform)
        {
            return new Mesh { Vertices = mesh.Vertices.Select(x => x + transform).ToList(), Faces = mesh.Faces.Select(x => x.Clone() as Face).ToList() };
        }

        /***************************************************/

        public static CompositeGeometry Translate(this CompositeGeometry group, Vector transform)
        {
            return new CompositeGeometry { Elements = group.Elements.Select(x => x.ITranslate(transform)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IBHoMGeometry ITranslate(this IBHoMGeometry geometry, Vector transform)
        {
            return Translate(geometry as dynamic, transform);
        }

        /***************************************************/

        public static ICurve ITranslate(this ICurve geometry, Vector transform)
        {
            return Translate(geometry as dynamic, transform);
        }

        /***************************************************/

        public static ISurface ITranslate(this ISurface geometry, Vector transform)
        {
            return Translate(geometry as dynamic, transform);
        }

        /***************************************************/
    }
}
