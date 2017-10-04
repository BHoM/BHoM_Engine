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
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static Point GetTranslated(Point pt, Vector transform)
        {
            return pt + transform;
        }

        /***************************************************/

        public static Vector GetTranslated(Vector vector, Vector transform)
        {
            return new Vector(vector.X, vector.Y, vector.Z);
        }

        /***************************************************/

        public static Plane GetTranslated(Plane plane, Vector transform)
        {
            return new Plane(plane.Origin + transform, plane.Normal.GetClone() as Vector);
        }


        /***************************************************/
        /**** Public Methods - Curves                  ****/
        /***************************************************/

        public static Arc GetTranslated(Arc arc, Vector transform)
        {
            return new Arc(arc.Start + transform, arc.Middle + transform, arc.End + transform);
        }

        /***************************************************/

        public static Circle GetTranslated(Circle circle, Vector transform)
        {
            return new Circle(circle.Centre + transform, circle.Normal.GetClone() as Vector, circle.Radius);
        }

        /***************************************************/

        public static Line GetTranslated(Line line, Vector transform)
        {
            return new Line(line.Start + transform, line.End + transform);
        }

        /***************************************************/

        public static NurbCurve GetTranslated(NurbCurve curve, Vector transform)
        {
            return new NurbCurve(curve.ControlPoints.Select(x => x + transform), curve.Weights, curve.Knots);
        }


        /***************************************************/

        public static PolyCurve GetTranslated(PolyCurve curve, Vector transform)
        {
            return new PolyCurve(curve.Curves.Select(x => x.IGetTranslated(transform)));
        }

        /***************************************************/

        public static Polyline GetTranslated(Polyline curve, Vector transform)
        {
            return new Polyline(curve.ControlPoints.Select(x => x + transform));
        }


        /***************************************************/
        /**** Public Methods - Surfaces                ****/
        /***************************************************/

        public static Extrusion GetTranslated(Extrusion surface, Vector transform)
        {
            return new Extrusion(surface.Curve.IGetTranslated(transform), surface.Direction.GetClone() as Vector, surface.Capped);
        }

        /***************************************************/

        public static Loft GetTranslated(Loft surface, Vector transform)
        {
            return new Loft(surface.Curves.Select(x => x.IGetTranslated(transform)));
        }

        /***************************************************/

        public static NurbSurface GetTranslated(NurbSurface surface, Vector transform)
        {
            return new NurbSurface(surface.ControlPoints.Select(x => x + transform), surface.Weights, surface.UKnots, surface.VKnots);
        }

        /***************************************************/

        public static Pipe GetTranslated(Pipe surface, Vector transform)
        {
            return new Pipe(surface.Centreline.IGetTranslated(transform), surface.Radius, surface.Capped);
        }

        /***************************************************/

        public static PolySurface GetTranslated(PolySurface surface, Vector transform)
        {
            return new PolySurface(surface.Surfaces.Select(x => x.IGetTranslated(transform)));
        }


        /***************************************************/
        /**** Public Methods - Others                  ****/
        /***************************************************/

        public static Mesh GetTranslated(Mesh mesh, Vector transform)
        {
            return new Mesh(mesh.Vertices.Select(x => x + transform), mesh.Faces.Select(x => x.GetClone() as Face));
        }

        /***************************************************/

        public static CompositeGeometry GetTranslated(this CompositeGeometry group, Vector transform)
        {
            return new CompositeGeometry(group.Elements.Select(x => x.IGetTranslated(transform)));
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IBHoMGeometry IGetTranslated(this IBHoMGeometry geometry, Vector transform)
        {
            return GetTranslated(geometry as dynamic, transform);
        }

        /***************************************************/

        public static ICurve IGetTranslated(this ICurve geometry, Vector transform)
        {
            return GetTranslated(geometry as dynamic, transform);
        }

        /***************************************************/

        public static ISurface IGetTranslated(this ISurface geometry, Vector transform)
        {
            return GetTranslated(geometry as dynamic, transform);
        }

    }
}
