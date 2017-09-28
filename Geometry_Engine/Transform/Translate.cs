using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Transform
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IBHoMGeometry GetTranslated(this IBHoMGeometry geometry, Vector transform)
        {
            return _GetTranslated(geometry as dynamic, transform);
        }


        /***************************************************/
        /**** Private Methods - Vectors                 ****/
        /***************************************************/

        private static Point _GetTranslated(Point pt, Vector transform)
        {
            return pt + transform;
        }

        /***************************************************/

        private static Vector _GetTranslated(Vector vector, Vector transform)
        {
            return new Vector(vector.X, vector.Y, vector.Z);
        }

        /***************************************************/

        private static Plane _GetTranslated(Plane plane, Vector transform)
        {
            return new Plane(plane.Origin + transform, plane.Normal.GetClone() as Vector);
        }


        /***************************************************/
        /**** Private Methods - Curves                  ****/
        /***************************************************/

        private static Arc _GetTranslated(Arc arc, Vector transform)
        {
            return new Arc(arc.Start + transform, arc.Middle + transform, arc.End + transform);
        }

        /***************************************************/

        private static Circle _GetTranslated(Circle circle, Vector transform)
        {
            return new Circle(circle.Centre + transform, circle.Normal.GetClone() as Vector, circle.Radius);
        }

        /***************************************************/

        private static Line _GetTranslated(Line line, Vector transform)
        {
            return new Line(line.Start + transform, line.End + transform);
        }

        /***************************************************/

        private static NurbCurve _GetTranslated(NurbCurve curve, Vector transform)
        {
            return new NurbCurve(curve.ControlPoints.Select(x => x + transform), curve.Weights, curve.Knots);
        }


        /***************************************************/

        private static PolyCurve _GetTranslated(PolyCurve curve, Vector transform)
        {
            return new PolyCurve(curve.Curves.Select(x => x.GetTranslated(transform) as ICurve));
        }

        /***************************************************/

        private static Polyline _GetTranslated(Polyline curve, Vector transform)
        {
            return new Polyline(curve.ControlPoints.Select(x => x + transform));
        }


        /***************************************************/
        /**** Private Methods - Surfaces                ****/
        /***************************************************/

        private static Extrusion _GetTranslated(Extrusion surface, Vector transform)
        {
            return new Extrusion(surface.Curve.GetTranslated(transform) as ICurve, surface.Direction.GetClone() as Vector, surface.Capped);
        }

        /***************************************************/

        private static Loft _GetTranslated(Loft surface, Vector transform)
        {
            return new Loft(surface.Curves.Select(x => x.GetTranslated(transform) as ICurve));
        }

        /***************************************************/

        private static NurbSurface _GetTranslated(NurbSurface surface, Vector transform)
        {
            return new NurbSurface(surface.ControlPoints.Select(x => x + transform), surface.Weights, surface.UKnots, surface.VKnots);
        }

        /***************************************************/

        private static Pipe _GetTranslated(Pipe surface, Vector transform)
        {
            return new Pipe(surface.Centreline.GetTranslated(transform) as ICurve, surface.Radius, surface.Capped);
        }

        /***************************************************/

        private static PolySurface _GetTranslated(PolySurface surface, Vector transform)
        {
            return new PolySurface(surface.Surfaces.Select(x => x.GetTranslated(transform) as ISurface));
        }


        /***************************************************/
        /**** Private Methods - Others                  ****/
        /***************************************************/

        private static Mesh _GetTranslated(Mesh mesh, Vector transform)
        {
            return new Mesh(mesh.Vertices.Select(x => x + transform), mesh.Faces.Select(x => x.GetClone() as Face));
        }

        /***************************************************/

        private static CompositeGeometry _GetTranslated(this CompositeGeometry group, Vector transform)
        {
            return new CompositeGeometry(group.Elements.Select(x => x.GetTranslated(transform)));
        }
    }
}
