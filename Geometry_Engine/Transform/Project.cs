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

        public static IBHoMGeometry GetProjected(this IBHoMGeometry geometry, Plane p)
        {
            return _GetProjected(geometry as dynamic, p);
        }

        /***************************************************/

        public static Point GetProjected(this Point pt, Line line)
        {
            Vector dir = line.GetDirection();
            double t = dir.GetDotProduct(pt - line.Start);
            return line.Start + dir * t;
        }


        /***************************************************/
        /**** Private Methods - Vectors                 ****/
        /***************************************************/

        private static Point _GetProjected(this Point pt, Plane p)
        {
            return pt - p.Normal.GetDotProduct(pt-p.Origin) * p.Normal;
        }

        /***************************************************/

        private static Vector _GetProjected(this Vector vector, Plane p)
        {
            return vector - vector.GetDotProduct(p.Normal) * p.Normal;
        }

        /***************************************************/

        private static Plane _GetProjected(this Plane plane, Plane p)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Private Methods - Curves                  ****/
        /***************************************************/

        private static Arc _GetProjected(this Arc arc, Plane p)
        {
            return new Arc(arc.Start._GetProjected(p), arc.Middle._GetProjected(p), arc.End._GetProjected(p));
        }

        /***************************************************/

        private static Circle _GetProjected(this Circle circle, Plane p)
        {
            if (circle._IsInPlane(p))
                return new Circle(circle.Centre.GetClone() as Point, p.Normal.GetClone() as Vector, circle.Radius);
            throw new NotImplementedException(); //TODO: sort out project for a circle
        }

        /***************************************************/

        private static Line _GetProjected(this Line line, Plane p)
        {
            return new Line(line.Start._GetProjected(p), line.End._GetProjected(p));
        }

        /***************************************************/

        private static NurbCurve _GetProjected(this NurbCurve curve, Plane p)
        {
            return new NurbCurve(curve.ControlPoints.Select(x => x._GetProjected(p)), curve.Weights, curve.Knots);
        }


        /***************************************************/

        private static PolyCurve _GetProjected(this PolyCurve curve, Plane p)
        {
            return new PolyCurve(curve.Curves.Select(x => x.GetProjected(p) as ICurve));
        }

        /***************************************************/

        private static Polyline _GetProjected(this Polyline curve, Plane p)
        {
            return new Polyline(curve.ControlPoints.Select(x => x._GetProjected(p)));
        }


        /***************************************************/
        /**** Private Methods - Surfaces                ****/
        /***************************************************/

        private static Extrusion _GetProjected(this Extrusion surface, Plane p)
        {
            return new Extrusion(surface.Curve.GetProjected(p) as ICurve, surface.Direction._GetProjected(p), surface.Capped);
        }

        /***************************************************/

        private static Loft _GetProjected(this Loft surface, Plane p)
        {
            return new Loft(surface.Curves.Select(x => x.GetProjected(p) as ICurve));
        }

        /***************************************************/

        private static NurbSurface _GetProjected(this NurbSurface surface, Plane p)
        {
            return new NurbSurface(surface.ControlPoints.Select(x => x._GetProjected(p)), surface.Weights, surface.UKnots, surface.VKnots);
        }

        /***************************************************/

        private static Pipe _GetProjected(this Pipe surface, Plane p)
        {
            throw new NotImplementedException(); //TODO: implement projection of a pipe on a plane
        }

        /***************************************************/

        private static PolySurface _GetProjected(this PolySurface surface, Plane p)
        {
            return new PolySurface(surface.Surfaces.Select(x => x.GetProjected(p) as ISurface));
        }


        /***************************************************/
        /**** Private Methods - Others                  ****/
        /***************************************************/

        private static Mesh _GetProjected(this Mesh mesh, Plane p)
        {
            return new Mesh(mesh.Vertices.Select(x => x._GetProjected(p)), mesh.Faces.Select(x => x.GetClone() as Face));
        }

        /***************************************************/

        private static CompositeGeometry _GetProjected(this CompositeGeometry group, Plane p)
        {
            return new CompositeGeometry(group.Elements.Select(x => x.GetProjected(p)));
        }
    }
}
