/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Base.Attributes;
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
            return pt.SquareDistance(other) < tolerance * tolerance;
        }

        /***************************************************/

        public static bool IsEqual(this Vector vector, Vector other, double tolerance = Tolerance.Distance)
        {
            return vector.Distance(other) < tolerance;
        }

        /***************************************************/

        public static bool IsEqual(this Basis basis, Basis other, double tolerance = Tolerance.Distance)
        {
            return basis.X.IsEqual(other.X, tolerance)
                && basis.Y.IsEqual(other.Y, tolerance)
                && basis.Z.IsEqual(other.Z, tolerance);
        }

        /***************************************************/

        public static bool IsEqual(this Cartesian coordinateSystem, Cartesian other, double tolerance = Tolerance.Distance)
        {
            return coordinateSystem.Origin.IsEqual(other.Origin, tolerance)
                && coordinateSystem.X.IsEqual(other.X, tolerance)
                && coordinateSystem.Y.IsEqual(other.Y, tolerance)
                && coordinateSystem.Z.IsEqual(other.Z, tolerance);
        }


        /***************************************************/
        /**** public Computation - Curves              ****/
        /***************************************************/

        public static bool IsEqual(this Arc arc, Arc other, double tolerance = Tolerance.Distance)
        {
            return arc.CoordinateSystem.IsEqual(other.CoordinateSystem, tolerance)
                && Math.Abs(arc.Radius - other.Radius) < tolerance
                && Math.Abs(arc.StartAngle - other.StartAngle) < tolerance //TODO: Using the distance tolerance here for now.
                && Math.Abs(arc.EndAngle - other.EndAngle) < tolerance;
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

        public static bool IsEqual(this NurbsCurve curve, NurbsCurve other, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.Count == other.ControlPoints.Count
                && curve.Knots.Count == other.Knots.Count
                && curve.Weights.Count == other.Weights.Count
                && curve.ControlPoints.Zip(other.ControlPoints, (a, b) => a.IsEqual(b, tolerance)).All(x => x)
                && curve.Knots.Zip(other.Knots, (a, b) => Math.Abs(a - b) < tolerance).All(x => x)          //TODO: Using distance tolerance. Find out what kind of tolerance should be used.
                && curve.Weights.Zip(other.Weights, (a, b) => Math.Abs(a - b) < tolerance).All(x => x);     //TODO: Using distance tolerance. Find out what kind of tolerance should be used.
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

        public static bool IsEqual(this PlanarSurface surface, PlanarSurface other, double tolerance = Tolerance.Distance)
        {
            if ((surface.InternalBoundaries != null ^ other.InternalBoundaries != null) &&
                surface.InternalBoundaries.Count != other.InternalBoundaries.Count)
                return false;

            foreach (ICurve curve in surface.InternalBoundaries)
            {
                if (!other.InternalBoundaries.Any(x => x.IIsEqual(curve, tolerance)))
                    return false;
            }

            return surface.ExternalBoundary.IIsEqual(other.ExternalBoundary, tolerance);
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

        public static bool IsEqual(this TransformMatrix transform, TransformMatrix other, double tolerance = Tolerance.Distance)
        {
            return (transform.Matrix.IsEqual(other.Matrix));
        }

        /***************************************************/

        public static bool IsEqual(this double[,] matrix, double[,] other, double tolerance = Tolerance.Distance)
        {
            int h = matrix.GetLength(0);
            int w = matrix.GetLength(1);
            if (h != other.GetLength(0) || w != other.GetLength(1))
                return false;

            for (int m = 0; m < h; m++)
            {
                for (int n = 0; n < w; n++)
                {
                    if (Math.Abs(matrix[m, n] - other[m, n]) > tolerance)
                        return false;
                }
            }

            return true;
        }

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
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static bool IsEqual(this IGeometry geometry, IGeometry other, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException($"IsEqual is not implemented for IGeometry of type: {geometry.GetType().Name}."); // Takes only first input because this method is supposed to be called only from the interface which culls out every case with inputs of different classes.
        }

        /***************************************************/
    }
}

