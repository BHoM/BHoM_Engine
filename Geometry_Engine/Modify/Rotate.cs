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

        public static Point Rotate(this Point pt, double rad, Vector axis)
        {
            throw new NotImplementedException(); //TODO: rotation of a point around arbitrary axis
        }

        /***************************************************/

        public static Vector Rotate(this Vector vector, double rad, Vector axis)
        {
            // using Rodrigues' rotation formula
            axis = axis.Normalise();

            return vector * Math.Cos(rad) + axis.CrossProduct(vector) * Math.Sin(rad) + axis * (axis * vector) * (1 - Math.Cos(rad));
        }

        /***************************************************/

        public static Plane Rotate(this Plane plane, double rad, Vector axis)
        {
            return new Plane { Origin = plane.Origin.Rotate(rad, axis), Normal = plane.Normal.Rotate(rad, axis) };
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Arc Rotate(this Arc arc, double rad, Vector axis)
        {
            return new Arc { Start = arc.Start.Rotate(rad, axis), Middle = arc.Middle.Rotate(rad, axis), End = arc.End.Rotate(rad, axis) };
        }

        /***************************************************/

        public static Circle Rotate(this Circle circle, double rad, Vector axis)
        {
            return new Circle { Centre = circle.Centre.Rotate(rad, axis), Normal = circle.Normal.Rotate(rad, axis), Radius = circle.Radius };
        }

        /***************************************************/

        public static Line Rotate(this Line line, double rad, Vector axis)
        {
            return new Line { Start = line.Start.Rotate(rad, axis), End = line.End.Rotate(rad, axis) };
        }

        /***************************************************/

        public static NurbCurve Rotate(this NurbCurve curve, double rad, Vector axis)
        {
            return new NurbCurve { ControlPoints = curve.ControlPoints.Select(x => x.Rotate(rad, axis)).ToList(), Weights = curve.Weights.ToList(), Knots = curve.Knots.ToList() };
        }


        /***************************************************/

        public static PolyCurve Rotate(this PolyCurve curve, double rad, Vector axis)
        {
            return new PolyCurve { Curves = curve.Curves.Select(x => x.IRotate(rad, axis)).ToList() };
        }

        /***************************************************/

        public static Polyline Rotate(this Polyline curve, double rad, Vector axis)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.Select(x => x.Rotate(rad, axis)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static Extrusion Rotate(this Extrusion surface, double rad, Vector axis)
        {
            return new Extrusion { Curve = surface.Curve.IRotate(rad, axis), Direction = surface.Direction.Rotate(rad, axis), Capped = surface.Capped };
        }

        /***************************************************/

        public static Loft Rotate(this Loft surface, double rad, Vector axis)
        {
            return new Loft { Curves = surface.Curves.Select(x => x.IRotate(rad, axis)).ToList() };
        }

        /***************************************************/

        public static NurbSurface Rotate(this NurbSurface surface, double rad, Vector axis)
        {
            return new NurbSurface { ControlPoints = surface.ControlPoints.Select(x => x.Rotate(rad, axis)).ToList(), Weights = surface.Weights.ToList(), UKnots = surface.UKnots.ToList(), VKnots = surface.VKnots.ToList() };
        }

        /***************************************************/

        public static Pipe Rotate(this Pipe surface, double rad, Vector axis)
        {
            return new Pipe { Centreline = surface.Centreline.IRotate(rad, axis), Radius = surface.Radius, Capped = surface.Capped };
        }

        /***************************************************/

        public static PolySurface Rotate(this PolySurface surface, double rad, Vector axis)
        {
            return new PolySurface { Surfaces = surface.Surfaces.Select(x => x.IRotate(rad, axis)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static Mesh Rotate(this Mesh mesh, double rad, Vector axis)
        {
            return new Mesh { Vertices = mesh.Vertices.Select(x => x.Rotate(rad, axis)).ToList(), Faces = mesh.Faces.Select(x => x.Clone()).ToList() };
        }

        /***************************************************/

        public static CompositeGeometry Rotate(this CompositeGeometry group, double rad, Vector axis)
        {
            return new CompositeGeometry { Elements = group.Elements.Select(x => x.IRotate(rad, axis)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IBHoMGeometry IRotate(this IBHoMGeometry geometry, double rad, Vector axis)
        {
            return Rotate(geometry as dynamic, rad, axis);
        }

        /***************************************************/

        public static ICurve IRotate(this ICurve geometry, double rad, Vector axis)
        {
            return Rotate(geometry as dynamic, rad, axis);
        }

        /***************************************************/

        public static ISurface IRotate(this ISurface geometry, double rad, Vector axis)
        {
            return Rotate(geometry as dynamic, rad, axis);
        }

        /***************************************************/
    }
}
