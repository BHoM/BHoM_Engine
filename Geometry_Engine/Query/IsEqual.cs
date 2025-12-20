/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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

using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** public Methods - Vectors                 ****/
        /***************************************************/

        [Description("Checks if the two Planes are geometrically equal, i.e. if all of their properties match within the given tolerance.")]
        [Input("plane", "The first Plane to compare.")]
        [Input("other", "The second Plane to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the Planes are equal within the tolerance, false otherwise.")]
        public static bool IsEqual(this Plane plane, Plane other, double tolerance = Tolerance.Distance)
        {
            return plane.Normal.IsEqual(other.Normal, tolerance)
                && plane.Origin.IsEqual(other.Origin, tolerance);
        }

        /***************************************************/

        [Description("Checks if the two Points are geometrically equal, i.e. if they are within the tolerance distance of each other.")]
        [Input("pt", "The first Point to compare.")]
        [Input("other", "The second Point to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the Points are equal within the tolerance, false otherwise.")]
        public static bool IsEqual(this Point pt, Point other, double tolerance = Tolerance.Distance)
        {
            return pt.SquareDistance(other) < tolerance * tolerance;
        }

        /***************************************************/

        [Description("Checks if the two Vector are geometrically equal, i.e. if they are within the tolerance distance of each other.")]
        [Input("vector", "The first Vector to compare.")]
        [Input("other", "The second Vector to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the Vectors are equal within the tolerance, false otherwise.")]
        public static bool IsEqual(this Vector vector, Vector other, double tolerance = Tolerance.Distance)
        {
            return vector.Distance(other) < tolerance;
        }

        /***************************************************/

        [Description("Checks if the two Bases are geometrically equal, i.e. if all of their properties match within the given tolerance.")]
        [Input("basis", "The first Basis to compare.")]
        [Input("other", "The second Basis to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the Bases are equal within the tolerance, false otherwise.")]
        public static bool IsEqual(this Basis basis, Basis other, double tolerance = Tolerance.Distance)
        {
            return basis.X.IsEqual(other.X, tolerance)
                && basis.Y.IsEqual(other.Y, tolerance)
                && basis.Z.IsEqual(other.Z, tolerance);
        }

        /***************************************************/

        [Description("Checks if the two Cartesian Coordinate Systems are geometrically equal, i.e. if all of their properties match within the given tolerance.")]
        [Input("coordinateSystem", "The first Cartesian coordinate system to compare.")]
        [Input("other", "The second Cartesian coordinate system to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the coordinate systems are equal within the tolerance, false otherwise.")]
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

        [Description("Checks if the two Arcs are geometrically equal, i.e. if all of their properties match within the given tolerance.")]
        [Input("arc", "The first Arc to compare.")]
        [Input("other", "The second Arc to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the Arcs are equal within the tolerance, false otherwise.")]
        public static bool IsEqual(this Arc arc, Arc other, double tolerance = Tolerance.Distance)
        {
            return arc.CoordinateSystem.IsEqual(other.CoordinateSystem, tolerance)
                && Math.Abs(arc.Radius - other.Radius) < tolerance
                && Math.Abs(arc.StartAngle - other.StartAngle) < tolerance //TODO: Using the distance tolerance here for now.
                && Math.Abs(arc.EndAngle - other.EndAngle) < tolerance;
        }

        /***************************************************/

        [Description("Checks if the two Circles are geometrically equal, i.e. if all of their properties match within the given tolerance.")]
        [Input("circle", "The first Circle to compare.")]
        [Input("other", "The second Circle to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the Circles are equal within the tolerance, false otherwise.")]
        public static bool IsEqual(this Circle circle, Circle other, double tolerance = Tolerance.Distance)
        {
            return Math.Abs(circle.Radius - other.Radius) < tolerance
                && circle.Centre.IsEqual(other.Centre, tolerance)
                && circle.Normal.IsEqual(other.Normal, tolerance);
        }

        /***************************************************/

        [Description("Checks if the two Ellipses are geometrically equal, i.e. if all of their properties match within the given tolerance.")]
        [Input("ellipse", "The first Ellipse to compare.")]
        [Input("other", "The second Ellipse to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the Ellipses are equal within the tolerance, false otherwise.")]
        public static bool IsEqual(this Ellipse ellipse, Ellipse other, double tolerance = Tolerance.Distance)
        {
            return Math.Abs(ellipse.Radius1 - other.Radius1) < tolerance
                && Math.Abs(ellipse.Radius2 - other.Radius2) < tolerance
                && ellipse.Centre.IsEqual(other.Centre, tolerance)
                && ellipse.Axis1.IsEqual(other.Axis1, tolerance)
                && ellipse.Axis2.IsEqual(other.Axis2, tolerance);
        }

        /***************************************************/

        [Description("Checks if the two Lines are geometrically equal, i.e. if all of their properties match within the given tolerance.")]
        [Input("line", "The first Line to compare.")]
        [Input("other", "The second Line to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the Lines are equal within the tolerance, false otherwise.")]
        public static bool IsEqual(this Line line, Line other, double tolerance = Tolerance.Distance)
        {
            return line.Start.IsEqual(other.Start, tolerance)
                && line.End.IsEqual(other.End, tolerance);
        }

        /***************************************************/
        [Description("Checks if the two NurbsCurves are geometrically equal, i.e. if all of their properties match within the given tolerance.")]
        [Input("curve", "The first NurbsCurve to compare.")]
        [Input("other", "The second NurbsCurve to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the NurbsCurves are equal within the tolerance, false otherwise.")]
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

        [Description("Checks if the two PolyCurves are geometrically equal, i.e. if all of their properties match within the given tolerance.")]
        [Input("curve", "The first PolyCurve to compare.")]
        [Input("other", "The second PolyCurve to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the PolyCurves are equal within the tolerance, false otherwise.")]
        public static bool IsEqual(this PolyCurve curve, PolyCurve other, double tolerance = Tolerance.Distance)
        {
            return curve.Curves.Count == other.Curves.Count
                 && curve.Curves.Zip(other.Curves, (a, b) => a.IIsEqual(b, tolerance)).All(x => x);
        }

        /***************************************************/

        [Description("Checks if the two Polylines are geometrically equal, i.e. if all of their properties match within the given tolerance.")]
        [Input("curve", "The first Polyline to compare.")]
        [Input("other", "The second Polyline to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the Polylines are equal within the tolerance, false otherwise.")]
        public static bool IsEqual(this Polyline curve, Polyline other, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.Count == other.ControlPoints.Count
                 && curve.ControlPoints.Zip(other.ControlPoints, (a, b) => a.IsEqual(b, tolerance)).All(x => x);
        }


        /***************************************************/
        /**** public Computation - Surfaces            ****/
        /***************************************************/

        [Description("Checks if the two Extrusions are geometrically equal, i.e. if all of their properties match within the given tolerance.")]
        [Input("surface", "The first Extrusion to compare.")]
        [Input("other", "The second Extrusion to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the Extrusions are equal within the tolerance, false otherwise.")]
        public static bool IsEqual(this Extrusion surface, Extrusion other, double tolerance = Tolerance.Distance)
        {
            return surface.Capped == other.Capped
                && surface.Direction.IsEqual(other.Direction, tolerance)
                && surface.Curve.IIsEqual(other.Curve, tolerance);
        }

        /***************************************************/

        [Description("Checks if the two Lofts are geometrically equal, i.e. if all of their properties match within the given tolerance.")]
        [Input("surface", "The first Loft to compare.")]
        [Input("other", "The second Loft to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the Lofts are equal within the tolerance, false otherwise.")]
        public static bool IsEqual(this Loft surface, Loft other, double tolerance = Tolerance.Distance)
        {
            return surface.Curves.Count == other.Curves.Count
                  && surface.Curves.Zip(other.Curves, (a, b) => a.IIsEqual(b, tolerance)).All(x => x);
        }

        /***************************************************/

        [Description("Checks if the two PlanarSurfaces are geometrically equal, i.e. if all of their properties match within the given tolerance.")]
        [Input("surface", "The first PlanarSurface to compare.")]
        [Input("other", "The second PlanarSurface to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the PlanarSurfaces are equal within the tolerance, false otherwise.")]
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

        [Description("Checks if the two Pipes are geometrically equal, i.e. if all of their properties match within the given tolerance.")]
        [Input("surface", "The first Pipe to compare.")]
        [Input("other", "The second Pipe to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the Pipes are equal within the tolerance, false otherwise.")]
        public static bool IsEqual(this Pipe surface, Pipe other, double tolerance = Tolerance.Distance)
        {
            return surface.Capped == other.Capped
               && Math.Abs(surface.Radius - other.Radius) < tolerance
               && surface.Centreline.IIsEqual(other.Centreline, tolerance);
        }

        /***************************************************/

        [Description("Checks if the two PolySurfaces are geometrically equal, i.e. if all of their properties match within the given tolerance.")]
        [Input("surface", "The first PolySurface to compare.")]
        [Input("other", "The second PolySurface to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the PolySurfaces are equal within the tolerance, false otherwise.")]
        public static bool IsEqual(this PolySurface surface, PolySurface other, double tolerance = Tolerance.Distance)
        {
            return surface.Surfaces.Count == other.Surfaces.Count
                 && surface.Surfaces.Zip(other.Surfaces, (a, b) => a.IIsEqual(b, tolerance)).All(x => x);
        }


        /***************************************************/
        /**** public Methods - Others                   ****/
        /***************************************************/

        [Description("Checks if the two TransformMatrices are geometrically equal, i.e. if all of their properties match within the given tolerance.")]
        [Input("transform", "The first TransformMatrix to compare.")]
        [Input("other", "The second TransformMatrix to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the TransformMatrices are equal within the tolerance, false otherwise.")]
        public static bool IsEqual(this TransformMatrix transform, TransformMatrix other, double tolerance = Tolerance.Distance)
        {
            return (transform.Matrix.IsEqual(other.Matrix));
        }

        /***************************************************/

        [Description("Checks if the two double[]s are geometrically equal, i.e. if all of their values are within tolerance of each other.")]
        [Input("matrix", "The first double[,] matrix to compare.")]
        [Input("other", "The second double[,] matrix to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the matrices are equal within the tolerance, false otherwise.")]
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

        [Description("Checks if the two BoundingBoxes are geometrically equal, i.e. if all of their properties match within the given tolerance.")]
        [Input("box", "The first BoundingBox to compare.")]
        [Input("other", "The second BoundingBox to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the BoundingBoxes are equal within the tolerance, false otherwise.")]
        public static bool IsEqual(this BoundingBox box, BoundingBox other, double tolerance = Tolerance.Distance)
        {
            return box.Min.IsEqual(other.Min, tolerance)
                && box.Max.IsEqual(other.Max, tolerance);
        }

        /***************************************************/

        [Description("Checks if the two Face are geometrically equal, i.e. their face indecies are the same.")]
        [Input("face", "The first Face to compare.")]
        [Input("other", "The second Face to compare.")]
        [Input("tolerance", "The tolerance for the comparison (not used for Face comparison).", typeof(Length))]
        [Output("isEqual", "True if the Faces have the same indices, false otherwise.")]
        public static bool IsEqual(this Face face, Face other, double tolerance = Tolerance.Distance)
        {
            return face.A == other.A && face.B == other.B && face.C == other.C && face.D == other.D;
        }

        /***************************************************/

        [Description("Checks if the two Meshes are geometrically equal, i.e. if all of their properties match within the given tolerance.")]
        [Input("mesh", "The first Mesh to compare.")]
        [Input("other", "The second Mesh to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the Meshes are equal within the tolerance, false otherwise.")]
        public static bool IsEqual(this Mesh mesh, Mesh other, double tolerance = Tolerance.Distance)
        {
            return mesh.Vertices.Count == other.Vertices.Count
               && mesh.Faces.Count == other.Faces.Count
               && mesh.Vertices.Zip(other.Vertices, (a, b) => a.IsEqual(b, tolerance)).All(x => x)
               && mesh.Faces.Zip(other.Faces, (a, b) => a.IsEqual(b, tolerance)).All(x => x);
        }

        /***************************************************/

        [Description("Checks if the two CompositeGeometry are geometrically equal, i.e. if all of their properties match within the given tolerance.")]
        [Input("group", "The first CompositeGeometry to compare.")]
        [Input("other", "The second CompositeGeometry to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the CompositeGeometries are equal within the tolerance, false otherwise.")]
        public static bool IsEqual(this CompositeGeometry group, CompositeGeometry other, double tolerance = Tolerance.Distance)
        {
            return group.Elements.Count == other.Elements.Count
                 && group.Elements.Zip(other.Elements, (a, b) => a.IIsEqual(b, tolerance)).All(x => x);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Checks if the two Geometries are of the same type and are geometrically equal, i.e. if all of their properties match within the given tolerance.")]
        [Input("geometry", "The first IGeometry to compare.")]
        [Input("other", "The second IGeometry to compare.")]
        [Input("tolerance", "The tolerance for the comparison.", typeof(Length))]
        [Output("isEqual", "True if the Geometries are of the same type and equal within the tolerance, false otherwise.")]
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
