using BH.oM.Geometry;
using System;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static Point Project(this Point pt, Plane p)
        {
            return pt - p.Normal.DotProduct(pt-p.Origin) * p.Normal;
        }

        /***************************************************/

        public static Point Project(this Point pt, Line line)
        {
            Vector dir = line.GetDirection();
            double t = dir.DotProduct(pt - line.Start);
            return line.Start + dir * t;
        }

        /***************************************************/

        public static Vector Project(this Vector vector, Plane p)
        {
            return vector - vector.DotProduct(p.Normal) * p.Normal;
        }

        /***************************************************/

        public static Plane Project(this Plane plane, Plane p)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Arc Project(this Arc arc, Plane p)
        {
            return new Arc { Start = arc.Start.Project(p), Middle = arc.Middle.Project(p), End = arc.End.Project(p) };
        }

        /***************************************************/

        public static Circle Project(this Circle circle, Plane p)
        {
            if (circle.IIsInPlane(p))
                return new Circle { Centre = circle.Centre.Clone() as Point, Normal = p.Normal.Clone() as Vector, Radius = circle.Radius };
            throw new NotImplementedException(); //TODO: sort out project for a circle
        }

        /***************************************************/

        public static Line Project(this Line line, Plane p)
        {
            return new Line { Start = line.Start.Project(p), End = line.End.Project(p) };
        }

        /***************************************************/

        public static NurbCurve Project(this NurbCurve curve, Plane p)
        {
            return new NurbCurve { ControlPoints = curve.ControlPoints.Select(x => x.Project(p)).ToList(), Weights = curve.Weights.ToList(), Knots = curve.Knots.ToList() };
        }


        /***************************************************/

        public static PolyCurve Project(this PolyCurve curve, Plane p)
        {
            return new PolyCurve { Curves = curve.Curves.Select(x => x.IProject(p)).ToList() };
        }

        /***************************************************/

        public static Polyline Project(this Polyline curve, Plane p)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.Select(x => x.Project(p)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static Extrusion Project(this Extrusion surface, Plane p)
        {
            return new Extrusion(surface.Curve.IProject(p), surface.Direction.Project(p), surface.Capped);
        }

        /***************************************************/

        public static Loft Project(this Loft surface, Plane p)
        {
            return new Loft(surface.Curves.Select(x => x.IProject(p)));
        }

        /***************************************************/

        public static NurbSurface Project(this NurbSurface surface, Plane p)
        {
            return new NurbSurface(surface.ControlPoints.Select(x => x.Project(p)), surface.Weights, surface.UKnots, surface.VKnots);
        }

        /***************************************************/

        public static Pipe Project(this Pipe surface, Plane p)
        {
            throw new NotImplementedException(); //TODO: implement projection of a pipe on a plane
        }

        /***************************************************/

        public static PolySurface Project(this PolySurface surface, Plane p)
        {
            return new PolySurface(surface.Surfaces.Select(x => x.IProject(p)));
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static Mesh Project(this Mesh mesh, Plane p)
        {
            return new Mesh { Vertices = mesh.Vertices.Select(x => x.Project(p)).ToList(), Faces = mesh.Faces.Select(x => x.Clone() as Face).ToList() };
        }

        /***************************************************/

        public static CompositeGeometry Project(this CompositeGeometry group, Plane p)
        {
            return new CompositeGeometry { Elements = group.Elements.Select(x => x.IProject(p)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IBHoMGeometry IProject(this IBHoMGeometry geometry, Plane p)
        {
            return Project(geometry as dynamic, p);
        }

        /***************************************************/

        public static ICurve IProject(this ICurve geometry, Plane p)
        {
            return Project(geometry as dynamic, p);
        }

        /***************************************************/

        public static ISurface IProject(this ISurface geometry, Plane p)
        {
            return Project(geometry as dynamic, p);
        }

        /***************************************************/
    }
}
