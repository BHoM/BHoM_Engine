﻿using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static bool IsInPlane(this IEnumerable<Point> points, Plane plane, double tolerance = Tolerance.Distance)
        {
            foreach (Point pt in points)
            {
                if (pt.Distance(plane) > tolerance)
                    return false;
            }
            return true;
        }

        /***************************************************/

        public static bool IsInPlane(this Point point, Plane plane, double tolerance = Tolerance.Distance)
        {
            return (point.Distance(plane) <= tolerance);
        }

        /***************************************************/

        public static bool IsInPlane(this Vector vector, Plane plane, double tolerance = Tolerance.Distance)
        {
            return Math.Abs(vector.DotProduct(plane.Normal)) <= tolerance;
        }

        /***************************************************/

        public static bool IsInPlane(this Plane plane1, Plane plane2, double tolerance = Tolerance.Distance, double angTolerance = Tolerance.Angle)
        {
            return plane1.Normal.IsParallel(plane2.Normal, angTolerance) != 0 && plane1.Origin.Distance(plane2) <= tolerance;
        }

        /***************************************************/

        public static bool IsInPlane(this Cartesian coordinateSystem, Plane plane, double tolerance = Tolerance.Distance, double angTolerance = Tolerance.Angle)
        {
            return ((Plane)coordinateSystem).IsInPlane(plane, tolerance, angTolerance);
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static bool IsInPlane(this Arc arc, Plane plane, double tolerance = Tolerance.Distance, double angTolerance = Tolerance.Angle)
        {
            return arc.CoordinateSystem.IsInPlane(plane, tolerance, angTolerance); //TODO: Is this check enough?
        }

        /***************************************************/

        public static bool IsInPlane(this Circle circle, Plane plane, double tolerance = Tolerance.Distance, double angTolerance = Tolerance.Angle)
        {
            //TODO: Is this check enough?
            return circle.Normal.IsParallel(plane.Normal, angTolerance) != 0 && Math.Abs(plane.Normal.DotProduct(circle.Centre - plane.Origin)) <= tolerance;
        }

        /***************************************************/

        public static bool IsInPlane(this Line line, Plane plane, double tolerance = Tolerance.Distance)
        {
            return line.Start.IsInPlane(plane, tolerance) && line.End.IsInPlane(plane, tolerance);
        }

        /***************************************************/

        [NotImplemented]
        public static bool IsInPlane(this NurbsCurve curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }
        
        /***************************************************/

        public static bool IsInPlane(this PolyCurve curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            foreach (ICurve c in curve.Curves)
            {
                if (!c.IIsInPlane(plane, tolerance))
                    return false;
            }

            return true;
        }

        /***************************************************/

        public static bool IsInPlane(this Polyline curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.IsInPlane(plane, tolerance);
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static bool IsInPlane(this Extrusion surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            return surface.Direction.IsInPlane(plane, tolerance) && surface.Curve.IIsInPlane(plane, tolerance);
        }

        /***************************************************/

        public static bool IsInPlane(this Loft surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            foreach (ICurve c in surface.Curves)
            {
                if (!c.IIsInPlane(plane, tolerance))
                    return false;
            }

            return true;
        }

        /***************************************************/

        [NotImplemented]
        public static bool IsInPlane(this NurbsSurface surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static bool IsInPlane(this Pipe surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        public static bool IsInPlane(this PolySurface surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            foreach (ISurface s in surface.Surfaces)
            {
                if (!s.IIsInPlane(plane, tolerance))
                    return false;
            }

            return true;
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static bool IsInPlane(this Mesh mesh, Plane plane, double tolerance = Tolerance.Distance)
        {
            return mesh.Vertices.IsInPlane(plane, tolerance);
        }

        /***************************************************/

        public static bool IsInPlane(this CompositeGeometry group, Plane plane, double tolerance = Tolerance.Distance)
        {
            foreach (IGeometry g in group.Elements)
            {
                if (!g.IIsInPlane(plane, tolerance))
                    return false;
            }

            return true;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static bool IIsInPlane(this IGeometry geometry, Plane plane, double tolerance = Tolerance.Distance)
        {
            return IsInPlane(geometry as dynamic, plane, tolerance);
        }

        /***************************************************/
    }
}
