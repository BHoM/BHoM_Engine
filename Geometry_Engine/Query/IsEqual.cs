using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** public Methods - Vectors                 ****/
        /***************************************************/

        public static bool IsEqual(this Plane plane, Plane other, double tolerance = Tolerance.Distance)
        {
            return plane.Normal.IsEqual(other.Normal, tolerance) 
                && plane.Origin.IsEqual(other.Origin, tolerance);
        }

        /***************************************************/

        public static bool IsEqual(this Point pt, Point other, double tolerance = Tolerance.Distance)
        {
            return pt.Distance(other) < tolerance;
        }

        /***************************************************/

        public static bool IsEqual(this Vector vector, Vector other, double tolerance = Tolerance.Distance)
        {
            return vector.Distance(other) < tolerance;
        }


        /***************************************************/
        /**** public Computation - Curves              ****/
        /***************************************************/

        public static bool IsEqual(this Arc arc, Arc other, double tolerance = Tolerance.Distance)
        {
            return arc.Start.IsEqual(other.Start, tolerance)
                && arc.End.IsEqual(other.End, tolerance)
                && arc.Middle.IsEqual(other.Middle, tolerance);
        }

        /***************************************************/

        public static bool IsEqual(this Circle circle, Circle other, double tolerance = Tolerance.Distance)
        {
            return Math.Abs(circle.Radius - other.Radius) < tolerance 
                && circle.Centre.IsEqual(other.Centre, tolerance) 
                && circle.Normal.IsEqual(other.Normal, tolerance);
        }

        /***************************************************/

        public static bool IsEqual(this Ellipse ellipse, Ellipse other, double tolerance = Tolerance.Distance)
        {
            return Math.Abs(ellipse.Radius1 - other.Radius1) < tolerance
                && Math.Abs(ellipse.Radius2 - other.Radius2) < tolerance
                && ellipse.Centre.IsEqual(other.Centre, tolerance)
                && ellipse.Axis1.IsEqual(other.Axis1, tolerance)
                && ellipse.Axis2.IsEqual(other.Axis2, tolerance);
        }

        /***************************************************/

        public static bool IsEqual(this Line line, Line other, double tolerance = Tolerance.Distance)
        {
            return line.Start.IsEqual(other.Start, tolerance)
                && line.End.IsEqual(other.End, tolerance);
        }

        /***************************************************/

        public static bool IsEqual(this NurbCurve curve, NurbCurve other, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.Count == other.ControlPoints.Count
                && curve.Weights.Count == other.Weights.Count
                && curve.Knots.Count == other.Knots.Count
                && curve.ControlPoints.Zip(other.ControlPoints, (a, b) => a.IsEqual(b, tolerance)).All(x => x)
                && curve.Weights.Zip(other.Weights, (a, b) => Math.Abs(a - b) < tolerance).All(x => x)
                && curve.Knots.Zip(other.Knots, (a, b) => Math.Abs(a - b) < tolerance).All(x => x);
        }

        /***************************************************/

        public static bool IsEqual(this PolyCurve curve, PolyCurve other, double tolerance = Tolerance.Distance)
        {
            return curve.Curves.Count == other.Curves.Count
                 && curve.Curves.Zip(other.Curves, (a, b) => a.IIsEqual(b, tolerance)).All(x => x);
        }

        /***************************************************/

        public static bool IsEqual(this Polyline curve, Polyline other, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.Count == other.ControlPoints.Count
                 && curve.ControlPoints.Zip(other.ControlPoints, (a, b) => a.IsEqual(b, tolerance)).All(x => x);
        }


        /***************************************************/
        /**** public Computation - Surfaces            ****/
        /***************************************************/

        public static bool IsEqual(this Extrusion surface, Extrusion other, double tolerance = Tolerance.Distance)
        {
            return surface.Capped == other.Capped
                && surface.Direction.IsEqual(other.Direction, tolerance)
                && surface.Curve.IIsEqual(other.Curve, tolerance);
        }

        /***************************************************/

        public static bool IsEqual(this Loft surface, Loft other, double tolerance = Tolerance.Distance)
        {
            return surface.Curves.Count == other.Curves.Count
                  && surface.Curves.Zip(other.Curves, (a, b) => a.IIsEqual(b, tolerance)).All(x => x);
        }

        /***************************************************/

        public static bool IsEqual(this NurbSurface surface, NurbSurface other, double tolerance = Tolerance.Distance)
        {
            return surface.ControlPoints.Count == other.ControlPoints.Count
                && surface.Weights.Count == other.Weights.Count
                && surface.UKnots.Count == other.UKnots.Count
                && surface.VKnots.Count == other.VKnots.Count
                && surface.ControlPoints.Zip(other.ControlPoints, (a, b) => a.IsEqual(b, tolerance)).All(x => x)
                && surface.Weights.Zip(other.Weights, (a, b) => Math.Abs(a - b) < tolerance).All(x => x)
                && surface.UKnots.Zip(other.UKnots, (a, b) => Math.Abs(a - b) < tolerance).All(x => x)
                && surface.VKnots.Zip(other.VKnots, (a, b) => Math.Abs(a - b) < tolerance).All(x => x);
        }

        /***************************************************/

        public static bool IsEqual(this Pipe surface, Pipe other, double tolerance = Tolerance.Distance)
        {
            return surface.Capped == other.Capped
               && Math.Abs(surface.Radius - other.Radius) < tolerance
               && surface.Centreline.IIsEqual(other.Centreline, tolerance);
        }

        /***************************************************/

        public static bool IsEqual(this PolySurface surface, PolySurface other, double tolerance = Tolerance.Distance)
        {
            return surface.Surfaces.Count == other.Surfaces.Count
                 && surface.Surfaces.Zip(other.Surfaces, (a, b) => a.IIsEqual(b, tolerance)).All(x => x);
        }


        /***************************************************/
        /**** public Methods - Others                   ****/
        /***************************************************/

        public static bool IsEqual(this BoundingBox box, BoundingBox other, double tolerance = Tolerance.Distance)
        {
            return box.Min.IsEqual(other.Min, tolerance)
                && box.Max.IsEqual(other.Max, tolerance);
        }

        /***************************************************/

        public static bool IsEqual(this Face face, Face other, double tolerance = Tolerance.Distance)
        {
            return face.A == other.A && face.B == other.B && face.C == other.C && face.D == other.D;
        }

        /***************************************************/

        public static bool IsEqual(this Mesh mesh, Mesh other, double tolerance = Tolerance.Distance)
        {
            return mesh.Vertices.Count == other.Vertices.Count
               && mesh.Faces.Count == other.Faces.Count
               && mesh.Vertices.Zip(other.Vertices, (a, b) => a.IsEqual(b, tolerance)).All(x => x)
               && mesh.Faces.Zip(other.Faces, (a, b) => a.IsEqual(b, tolerance)).All(x => x);
        }

        /***************************************************/

        public static bool IsEqual(this CompositeGeometry group, CompositeGeometry other, double tolerance = Tolerance.Distance)
        {
            return group.Elements.Count == other.Elements.Count
                 && group.Elements.Zip(other.Elements, (a, b) => a.IIsEqual(b, tolerance)).All(x => x);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static bool IIsEqual(this IGeometry geometry, IGeometry other, double tolerance = Tolerance.Distance)
        {
            if (geometry.GetType() != other.GetType())
                return false;
            else
             return IsEqual(geometry as dynamic, other as dynamic, tolerance);
        }

        /***************************************************/
    }
}
