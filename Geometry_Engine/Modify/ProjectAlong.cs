using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static Point ProjectAlong(this Point point, Plane plane, Vector vector)
        {
            if (Math.Abs(vector.DotProduct(plane.Normal)) <= Tolerance.Angle) return null;

            double t = (plane.Normal * (plane.Origin - point)) / (plane.Normal * vector);
            return point + t * vector;
        }

        /***************************************************/

        public static Point ProjectAlong(this Point point, Line line, Vector vector, double tolerance = Tolerance.Distance)
        {
            Line l = new Line { Start = point, End = point + vector };
            return line.LineIntersection(l, true, tolerance);
        }

        /***************************************************/

        public static Plane Project(this Plane plane, Plane ToPlane, Vector vector)
        {
            double dp = plane.Normal.DotProduct(ToPlane.Normal);
            if (Math.Abs(dp) <= Tolerance.Angle) return null;
            Vector normal = dp > 0 ? ToPlane.Normal : ToPlane.Normal.Reverse();
            return new Plane { Origin = plane.Origin.ProjectAlong(ToPlane, vector), Normal = normal };
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [NotImplemented]
        public static Arc ProjectAlong(this Arc arc, Plane plane, Vector vector)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static ICurve ProjectAlong(this Circle circle, Plane plane, Vector vector)
        {
            if (circle.Normal.IsParallel(plane.Normal) != 0)
                return new Circle { Centre = circle.Centre.ProjectAlong(plane, vector), Normal = circle.Normal.Clone(), Radius = circle.Radius };

            Vector axis1 = plane.Normal.CrossProduct(circle.Normal);
            Vector axis2 = axis1.CrossProduct(plane.Normal);
            double radius2 = circle.Radius * circle.Normal.DotProduct(plane.Normal);
            
            return new Ellipse { Centre = circle.Centre.ProjectAlong(plane, vector), Axis1 = axis1, Axis2 = axis2, Radius1 = circle.Radius, Radius2 = radius2 };
        }

        /***************************************************/

        public static Line ProjectAlong(this Line line, Plane plane, Vector vector)
        {
            return new Line { Start = line.Start.ProjectAlong(plane, vector), End = line.End.ProjectAlong(plane, vector) };
        }

        /***************************************************/

        public static NurbCurve ProjectAlong(this NurbCurve curve, Plane plane, Vector vector)
        {
            return new NurbCurve { ControlPoints = curve.ControlPoints.Select(x => x.ProjectAlong(plane, vector)).ToList(), Weights = curve.Weights.ToList(), Knots = curve.Knots.ToList() };
        }


        /***************************************************/

        public static PolyCurve ProjectAlong(this PolyCurve curve, Plane plane, Vector vector)
        {
            return new PolyCurve { Curves = curve.Curves.Select(x => x.IProjectAlong(plane, vector)).ToList() };
        }

        /***************************************************/

        public static Polyline ProjectAlong(this Polyline curve, Plane plane, Vector vector)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.Select(x => x.ProjectAlong(plane, vector)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static Extrusion ProjectAlong(this Extrusion surface, Plane plane, Vector vector)
        {
            return new Extrusion { Curve = surface.Curve.IProjectAlong(plane, vector), Direction = surface.Direction.Project(plane), Capped = surface.Capped };
        }

        /***************************************************/

        public static Loft ProjectAlong(this Loft surface, Plane plane, Vector vector)
        {
            return new Loft { Curves = surface.Curves.Select(x => x.IProjectAlong(plane, vector)).ToList() };
        }

        /***************************************************/

        public static NurbSurface ProjectAlong(this NurbSurface surface, Plane plane, Vector vector)
        {
            return new NurbSurface { ControlPoints = surface.ControlPoints.Select(x => x.ProjectAlong(plane, vector)).ToList(), Weights = surface.Weights.ToList(), UKnots = surface.UKnots.ToList(), VKnots = surface.VKnots.ToList() };
        }

        /***************************************************/

        [NotImplemented]
        public static Pipe ProjectAlong(this Pipe surface, Plane plane, Vector vector)
        {
            throw new NotImplementedException(); //TODO: implement projection of a pipe on a plane
        }

        /***************************************************/

        public static PolySurface ProjectAlong(this PolySurface surface, Plane plane, Vector vector)
        {
            return new PolySurface { Surfaces = surface.Surfaces.Select(x => x.IProjectAlong(plane, vector)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static Mesh ProjectAlong(this Mesh mesh, Plane plane, Vector vector)
        {
            return new Mesh { Vertices = mesh.Vertices.Select(x => x.ProjectAlong(plane, vector)).ToList(), Faces = mesh.Faces.Select(x => x.Clone() as Face).ToList() };
        }

        /***************************************************/

        public static CompositeGeometry ProjectAlong(this CompositeGeometry group, Plane plane, Vector vector)
        {
            return new CompositeGeometry { Elements = group.Elements.Select(x => x.IProjectAlong(plane, vector)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IGeometry IProjectAlong(this IGeometry geometry, Plane plane, Vector vector)
        {
            return ProjectAlong(geometry as dynamic, plane, vector);
        }

        /***************************************************/

        public static ICurve IProjectAlong(this ICurve geometry, Plane plane, Vector vector)
        {
            return ProjectAlong(geometry as dynamic, plane, vector);
        }

        /***************************************************/

        public static ISurface IProjectAlong(this ISurface geometry, Plane plane, Vector vector)
        {
            return ProjectAlong(geometry as dynamic, plane, vector);
        }
    }
}
