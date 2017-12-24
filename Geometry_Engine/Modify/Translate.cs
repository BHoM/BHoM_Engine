using BH.oM.Geometry;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static Point Translate(Point pt, Vector transform)
        {
            return pt + transform;
        }

        /***************************************************/

        public static Vector Translate(Vector vector, Vector transform)
        {
            return new Vector(vector.X, vector.Y, vector.Z);
        }

        /***************************************************/

        public static Plane Translate(Plane plane, Vector transform)
        {
            return new Plane(plane.Origin + transform, plane.Normal.Clone() as Vector);
        }


        /***************************************************/
        /**** Public Methods - Curves                  ****/
        /***************************************************/

        public static Arc Translate(Arc arc, Vector transform)
        {
            return new Arc { Start = arc.Start + transform, Middle = arc.Middle + transform, End = arc.End + transform };
        }

        /***************************************************/

        public static Circle Translate(Circle circle, Vector transform)
        {
            return new Circle { Centre = circle.Centre + transform, Normal = circle.Normal.Clone() as Vector, Radius = circle.Radius };
        }

        /***************************************************/

        public static Line Translate(Line line, Vector transform)
        {
            return new Line { Start = line.Start + transform, End = line.End + transform };
        }

        /***************************************************/

        public static NurbCurve Translate(NurbCurve curve, Vector transform)
        {
            return new NurbCurve { ControlPoints = curve.ControlPoints.Select(x => x + transform).ToList(), Weights = curve.Weights.ToList(), Knots = curve.Knots.ToList() };
        }


        /***************************************************/

        public static PolyCurve Translate(PolyCurve curve, Vector transform)
        {
            return new PolyCurve { Curves = curve.Curves.Select(x => x.ITranslate(transform)).ToList() };
        }

        /***************************************************/

        public static Polyline Translate(Polyline curve, Vector transform)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.Select(x => x + transform).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Surfaces                ****/
        /***************************************************/

        public static Extrusion Translate(Extrusion surface, Vector transform)
        {
            return new Extrusion(surface.Curve.ITranslate(transform), surface.Direction.Clone() as Vector, surface.Capped);
        }

        /***************************************************/

        public static Loft Translate(Loft surface, Vector transform)
        {
            return new Loft(surface.Curves.Select(x => x.ITranslate(transform)));
        }

        /***************************************************/

        public static NurbSurface Translate(NurbSurface surface, Vector transform)
        {
            return new NurbSurface(surface.ControlPoints.Select(x => x + transform), surface.Weights, surface.UKnots, surface.VKnots);
        }

        /***************************************************/

        public static Pipe Translate(Pipe surface, Vector transform)
        {
            return new Pipe(surface.Centreline.ITranslate(transform), surface.Radius, surface.Capped);
        }

        /***************************************************/

        public static PolySurface Translate(PolySurface surface, Vector transform)
        {
            return new PolySurface(surface.Surfaces.Select(x => x.ITranslate(transform)));
        }


        /***************************************************/
        /**** Public Methods - Others                  ****/
        /***************************************************/

        public static Mesh Translate(Mesh mesh, Vector transform)
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
