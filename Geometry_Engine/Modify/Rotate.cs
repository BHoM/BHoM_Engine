using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using System;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static Point Rotate(this Point pt, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(pt, rotationMatrix);
        }

        /***************************************************/

        public static Vector Rotate(this Vector vector, double rad, Vector axis)
        {
            // using Rodrigues' rotation formula
            axis = axis.Normalise();

            return vector * Math.Cos(rad) + axis.CrossProduct(vector) * Math.Sin(rad) + axis * (axis * vector) * (1 - Math.Cos(rad));
        }

        /***************************************************/

        public static Plane Rotate(this Plane plane, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(plane, rotationMatrix);
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Arc Rotate(this Arc arc, Point origin, Vector axis, double rad)
        {
            return new Arc
            {
                CoordinateSystem = arc.CoordinateSystem.Rotate(origin, axis, rad),
                Radius = arc.Radius,
                StartAngle = arc.StartAngle,
                EndAngle = arc.EndAngle
            };
        }

        /***************************************************/

        public static Circle Rotate(this Circle circle, Point origin, Vector axis, double rad)
        {
            return new Circle { Centre = circle.Centre.Rotate(origin, axis, rad), Normal = circle.Normal.Rotate(rad, axis), Radius = circle.Radius };
        }

        /***************************************************/

        public static Line Rotate(this Line line, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(line, rotationMatrix);
        }

        /***************************************************/

        public static NurbsCurve Rotate(this NurbsCurve curve, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(curve, rotationMatrix);
        }


        /***************************************************/

        public static PolyCurve Rotate(this PolyCurve curve, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(curve, rotationMatrix);
        }

        /***************************************************/

        public static Polyline Rotate(this Polyline curve, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(curve, rotationMatrix);
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static Extrusion Rotate(this Extrusion surface, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(surface, rotationMatrix);
        }

        /***************************************************/

        public static Loft Rotate(this Loft surface, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(surface, rotationMatrix);
        }

        /***************************************************/

        public static NurbsSurface Rotate(this NurbsSurface surface, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(surface, rotationMatrix);
        }

        /***************************************************/

        public static Pipe Rotate(this Pipe surface, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(surface, rotationMatrix);
        }

        /***************************************************/

        public static PolySurface Rotate(this PolySurface surface, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(surface, rotationMatrix);
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static Mesh Rotate(this Mesh mesh, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(mesh, rotationMatrix);
        }

        /***************************************************/

        public static CompositeGeometry Rotate(this CompositeGeometry group, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(group, rotationMatrix);
        }

        /***************************************************/

        public static Cartesian Rotate(this Cartesian coordinate, Point origin, Vector axis, double rad)
        {
            return new Cartesian(coordinate.Origin.Rotate(origin, axis, rad), coordinate.X.Rotate(rad, axis), coordinate.Y.Rotate(rad, axis), coordinate.Z.Rotate(rad, axis));
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IGeometry IRotate(this IGeometry geometry, Point origin, Vector axis, double rad)
        {
            return Rotate(geometry as dynamic, rad, axis);
        }

        /***************************************************/

        public static ICurve IRotate(this ICurve geometry, Point origin, Vector axis, double rad)
        {
            return Rotate(geometry as dynamic, origin,axis, rad);
        }

        /***************************************************/

        public static ISurface IRotate(this ISurface geometry, Point origin, Vector axis, double rad)
        {
            return Rotate(geometry as dynamic, rad, axis);
        }
    }
}
