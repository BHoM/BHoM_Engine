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

using BH.Engine.Base;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        [Description("Transforms a Point using the provided transformation matrix.")]
        [Input("pt", "The Point to transform.")]
        [Input("transform", "The transformation matrix to apply to the Point.")]
        [Output("point", "The transformed Point.")]
        public static Point Transform(this Point pt, TransformMatrix transform)
        {
            double[,] matrix = transform.Matrix;

            return new Point
            {
                X = matrix[0, 0] * pt.X + matrix[0, 1] * pt.Y + matrix[0, 2] * pt.Z + matrix[0, 3],
                Y = matrix[1, 0] * pt.X + matrix[1, 1] * pt.Y + matrix[1, 2] * pt.Z + matrix[1, 3],
                Z = matrix[2, 0] * pt.X + matrix[2, 1] * pt.Y + matrix[2, 2] * pt.Z + matrix[2, 3]
            };
        }

        /***************************************************/

        [Description("Transforms a Vector using the provided transformation matrix.")]
        [Input("vector", "The Vector to transform.")]
        [Input("transform", "The transformation matrix to apply to the Vector.")]
        [Output("vector", "The transformed Vector.")]
        public static Vector Transform(this Vector vector, TransformMatrix transform)
        {
            double[,] matrix = transform.Matrix;

            return new Vector
            {
                X = matrix[0, 0] * vector.X + matrix[0, 1] * vector.Y + matrix[0, 2] * vector.Z,
                Y = matrix[1, 0] * vector.X + matrix[1, 1] * vector.Y + matrix[1, 2] * vector.Z,
                Z = matrix[2, 0] * vector.X + matrix[2, 1] * vector.Y + matrix[2, 2] * vector.Z
            };
        }

        /***************************************************/

        [Description("Transforms a Plane using the provided transformation matrix.")]
        [Input("plane", "The Plane to transform.")]
        [Input("transform", "The transformation matrix to apply to the Plane.")]
        [Output("plane", "The transformed Plane.")]
        public static Plane Transform(this Plane plane, TransformMatrix transform)
        {
            return new Plane { Origin = plane.Origin.Transform(transform), Normal = plane.Normal.Transform(transform).Normalise() };
        }

        /***************************************************/

        [Description("Transforms a Basis using the provided transformation matrix.")]
        [Input("basis", "The Basis to transform.")]
        [Input("transform", "The transformation matrix to apply to the Basis.")]
        [Output("basis", "The transformed Basis.")]
        public static Basis Transform(this Basis basis, TransformMatrix transform)
        {
            return Create.Basis(basis.X.Transform(transform), basis.Y.Transform(transform));
        }

        /***************************************************/

        [Description("Transforms a Cartesian coordinate system using the provided transformation matrix.")]
        [Input("coordinateSystem", "The Cartesian coordinate system to transform.")]
        [Input("transform", "The transformation matrix to apply to the coordinate system.")]
        [Output("coordinateSystem", "The transformed Cartesian coordinate system.")]
        public static Cartesian Transform(this Cartesian coordinateSystem, TransformMatrix transform)
        {
            Point origin = coordinateSystem.Origin.Transform(transform);
            Vector x = coordinateSystem.X.Transform(transform);

            Plane plane = Create.Plane(origin, x);
            Vector y = coordinateSystem.Y.Transform(transform).Project(plane);

            return Create.CartesianCoordinateSystem(origin, x, y);
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Transforms an Arc using the provided transformation matrix. For non-rigid or non-uniform transformations, the Arc is converted to a NurbsCurve.")]
        [Input("curve", "The Arc to transform.")]
        [Input("transform", "The transformation matrix to apply to the Arc.")]
        [Output("curve", "The transformed curve as an Arc or NurbsCurve.")]
        public static ICurve Transform(this Arc curve, TransformMatrix transform)
        {
            if (transform.IsRigidTransformation() || transform.IsUniformScaling())
                return new Arc
                {
                    Radius = (curve.StartPoint() - curve.CoordinateSystem.Origin).Transform(transform).Length(),
                    StartAngle = curve.StartAngle,
                    EndAngle = curve.EndAngle,
                    CoordinateSystem = curve.CoordinateSystem.Transform(transform)
                };
            else
            {
                Base.Compute.RecordNote("Transformation is not rigid or uniform. Converting into NurbsCurve. Change in shape may occur.");
                return curve.ToNurbsCurve().Transform(transform);
            }
        }

        /***************************************************/

        [Description("Transforms a Circle using the provided transformation matrix. For non-rigid or non-uniform transformations, the Circle is converted to a NurbsCurve.")]
        [Input("curve", "The Circle to transform.")]
        [Input("transform", "The transformation matrix to apply to the Circle.")]
        [Output("curve", "The transformed curve as a Circle or NurbsCurve.")]
        public static ICurve Transform(this Circle curve, TransformMatrix transform)
        {
            if (transform.IsRigidTransformation() || transform.IsUniformScaling())
                return new Circle
                {
                    Centre = curve.Centre.Transform(transform),
                    Radius = (curve.StartPoint() - curve.Centre).Transform(transform).Length(),
                    Normal = curve.Normal.Transform(transform)
                };
            else
            {
                Base.Compute.RecordNote("Transformation is not rigid or uniform. Converting into NurbsCurve. Change in shape may occur.");
                return curve.ToNurbsCurve().Transform(transform);
            }
        }

        /***************************************************/

        [Description("Transforms an Ellipse using the provided transformation matrix. For non-rigid or non-uniform transformations, the Ellipse is converted to a NurbsCurve.")]
        [Input("curve", "The Ellipse to transform.")]
        [Input("transform", "The transformation matrix to apply to the Ellipse.")]
        [Output("curve", "The transformed curve as an Ellipse or NurbsCurve.")]
        public static ICurve Transform(this Ellipse curve, TransformMatrix transform)
        {
            if (transform.IsRigidTransformation() || transform.IsUniformScaling())
                return new Ellipse
                {
                    Centre = curve.Centre.Transform(transform),
                    Axis1 = curve.Axis1.Transform(transform),
                    Axis2 = curve.Axis2.Transform(transform),
                    Radius1 = (curve.Axis1.Normalise() * curve.Radius1).Transform(transform).Length(),
                    Radius2 = (curve.Axis2.Normalise() * curve.Radius2).Transform(transform).Length(),
                };
            else
            {
                Base.Compute.RecordNote("Transformation is not rigid or uniform. Converting into NurbsCurve. Change in shape may occur.");
                return curve.ToNurbsCurve().Transform(transform);
            }
        }

        /***************************************************/

        [Description("Transforms a Line using the provided transformation matrix.")]
        [Input("curve", "The Line to transform.")]
        [Input("transform", "The transformation matrix to apply to the Line.")]
        [Output("line", "The transformed Line.")]
        public static Line Transform(this Line curve, TransformMatrix transform)
        {
            return new Line { Start = curve.Start.Transform(transform), End = curve.End.Transform(transform) };
        }

        /***************************************************/

        [Description("Transforms a NurbsCurve using the provided transformation matrix.")]
        [Input("curve", "The NurbsCurve to transform.")]
        [Input("transform", "The transformation matrix to apply to the NurbsCurve.")]
        [Output("curve", "The transformed NurbsCurve.")]
        public static NurbsCurve Transform(this NurbsCurve curve, TransformMatrix transform)
        {
            return new NurbsCurve()
            {
                ControlPoints = curve.ControlPoints.Select(cp => cp.Transform(transform)).ToList(),
                Weights = curve.Weights,
                Knots = curve.Knots
            };
        }

        /***************************************************/

        [Description("Transforms a PolyCurve using the provided transformation matrix.")]
        [Input("curve", "The PolyCurve to transform.")]
        [Input("transform", "The transformation matrix to apply to the PolyCurve.")]
        [Output("curve", "The transformed PolyCurve.")]
        public static PolyCurve Transform(this PolyCurve curve, TransformMatrix transform)
        {
            return new PolyCurve { Curves = curve.Curves.Select(x => x.ITransform(transform)).ToList() };
        }

        /***************************************************/

        [Description("Transforms a Polyline using the provided transformation matrix.")]
        [Input("curve", "The Polyline to transform.")]
        [Input("transform", "The transformation matrix to apply to the Polyline.")]
        [Output("curve", "The transformed Polyline.")]
        public static Polyline Transform(this Polyline curve, TransformMatrix transform)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.Select(x => x.Transform(transform)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        [Description("Transforms an Extrusion surface using the provided transformation matrix.")]
        [Input("surface", "The Extrusion surface to transform.")]
        [Input("transform", "The transformation matrix to apply to the Extrusion.")]
        [Output("surface", "The transformed Extrusion surface.")]
        public static Extrusion Transform(this Extrusion surface, TransformMatrix transform)
        {
            return new Extrusion { Curve = surface.Curve.ITransform(transform), Direction = surface.Direction.Transform(transform), Capped = surface.Capped };
        }

        /***************************************************/

        [Description("Transforms a Loft surface using the provided transformation matrix.")]
        [Input("surface", "The Loft surface to transform.")]
        [Input("transform", "The transformation matrix to apply to the Loft.")]
        [Output("surface", "The transformed Loft surface.")]
        public static Loft Transform(this Loft surface, TransformMatrix transform)
        {
            return new Loft { Curves = surface.Curves.Select(x => x.ITransform(transform)).ToList() };
        }

        /***************************************************/

        [Description("Transforms a NurbsSurface using the provided transformation matrix.")]
        [Input("surface", "The NurbsSurface to transform.")]
        [Input("transform", "The transformation matrix to apply to the NurbsSurface.")]
        [Output("surface", "The transformed NurbsSurface.")]
        public static NurbsSurface Transform(this NurbsSurface surface, TransformMatrix transform)
        {
            List<SurfaceTrim> innerTrims = surface.InnerTrims.Select(x => new SurfaceTrim(ITransform(x.Curve3d, transform), x.Curve2d)).ToList();

            List<SurfaceTrim> outerTrims = surface.OuterTrims.Select(x => new SurfaceTrim(ITransform(x.Curve3d, transform), x.Curve2d)).ToList();

            return new NurbsSurface(
                surface.ControlPoints.Select(x => Transform(x, transform)),
                surface.Weights,
                surface.UKnots,
                surface.VKnots,
                surface.UDegree,
                surface.VDegree,
                innerTrims,
                outerTrims);
        }

        /***************************************************/

        [Description("Transforms a Pipe surface using the provided transformation matrix.")]
        [Input("surface", "The Pipe surface to transform.")]
        [Input("transform", "The transformation matrix to apply to the Pipe.")]
        [Output("surface", "The transformed Pipe surface.")]
        public static Pipe Transform(this Pipe surface, TransformMatrix transform)
        {
            return new Pipe { Centreline = surface.Centreline.ITransform(transform), Radius = surface.Radius, Capped = surface.Capped };
        }

        /***************************************************/

        [Description("Transforms a PlanarSurface using the provided transformation matrix.")]
        [Input("surface", "The PlanarSurface to transform.")]
        [Input("transform", "The transformation matrix to apply to the PlanarSurface.")]
        [Output("surface", "The transformed PlanarSurface.")]
        public static PlanarSurface Transform(this PlanarSurface surface, TransformMatrix transform)
        {
            return new PlanarSurface(surface.ExternalBoundary.ITransform(transform), surface.InternalBoundaries.Select(x => x.ITransform(transform)).ToList());
        }

        /***************************************************/

        [Description("Transforms a PolySurface using the provided transformation matrix.")]
        [Input("surface", "The PolySurface to transform.")]
        [Input("transform", "The transformation matrix to apply to the PolySurface.")]
        [Output("surface", "The transformed PolySurface.")]
        public static PolySurface Transform(this PolySurface surface, TransformMatrix transform)
        {
            return new PolySurface { Surfaces = surface.Surfaces.Select(x => x.ITransform(transform)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Mesh                     ****/
        /***************************************************/

        [Description("Transforms a Mesh using the provided transformation matrix.")]
        [Input("mesh", "The Mesh to transform.")]
        [Input("transform", "The transformation matrix to apply to the Mesh.")]
        [Output("mesh", "The transformed Mesh.")]
        public static Mesh Transform(this Mesh mesh, TransformMatrix transform)
        {
            return new Mesh { Vertices = mesh.Vertices.Select(x => x.Transform(transform)).ToList(), Faces = mesh.Faces.ToList() };
        }


        /***************************************************/
        /**** Public Methods - Solid                    ****/
        /***************************************************/

        [Description("Transforms a BoundaryRepresentation solid using the provided transformation matrix. Volume will be invalidated for non-rigid transformations.")]
        [Input("solid", "The BoundaryRepresentation solid to transform.")]
        [Input("transform", "The transformation matrix to apply to the solid.")]
        [Output("solid", "The transformed BoundaryRepresentation solid.")]
        public static BoundaryRepresentation Transform(this BoundaryRepresentation solid, TransformMatrix transform)
        {
            double volume = solid.Volume;
            if (transform.IsRigidTransformation())
            {
                Base.Compute.RecordWarning("Transformation is not rigid. Therefore stored BoundaryRepresentation Volume will be invalidated and reset to NaN (not a number)");
                volume = double.NaN;
            }

            return new BoundaryRepresentation(solid.Surfaces.Select(x => x.ITransform(transform)), volume);
        }


        /***************************************************/
        /**** Public Methods - Misc                     ****/
        /***************************************************/

        [Description("Transforms a CompositeGeometry using the provided transformation matrix.")]
        [Input("group", "The CompositeGeometry to transform.")]
        [Input("transform", "The transformation matrix to apply to the CompositeGeometry.")]
        [Output("group", "The transformed CompositeGeometry.")]
        public static CompositeGeometry Transform(this CompositeGeometry group, TransformMatrix transform)
        {
            return new CompositeGeometry { Elements = group.Elements.Select(x => x.ITransform(transform)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Transforms any IGeometry using the provided transformation matrix.")]
        [Input("geometry", "The IGeometry to transform.")]
        [Input("transform", "The transformation matrix to apply to the geometry.")]
        [Output("geometry", "The transformed IGeometry.")]
        public static IGeometry ITransform(this IGeometry geometry, TransformMatrix transform)
        {
            return Transform(geometry as dynamic, transform);
        }

        /***************************************************/

        [Description("Transforms any ICurve using the provided transformation matrix.")]
        [Input("geometry", "The ICurve to transform.")]
        [Input("transform", "The transformation matrix to apply to the curve.")]
        [Output("geometry", "The transformed ICurve.")]
        public static ICurve ITransform(this ICurve geometry, TransformMatrix transform)
        {
            return Transform(geometry as dynamic, transform);
        }

        /***************************************************/

        [Description("Transforms any ISurface using the provided transformation matrix.")]
        [Input("geometry", "The ISurface to transform.")]
        [Input("transform", "The transformation matrix to apply to the surface.")]
        [Output("geometry", "The transformed ISurface.")]
        public static ISurface ITransform(this ISurface geometry, TransformMatrix transform)
        {
            return Transform(geometry as dynamic, transform);
        }

        /***************************************************/

        [Description("Transforms any ISolid using the provided transformation matrix.")]
        [Input("geometry", "The ISolid to transform.")]
        [Input("transform", "The transformation matrix to apply to the solid.")]
        [Output("geometry", "The transformed ISurface.")]
        public static ISurface ITransform(this ISolid geometry, TransformMatrix transform)
        {
            return Transform(geometry as dynamic, transform);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static IGeometry Transform(this IGeometry geometry, TransformMatrix transform)
        {
            Base.Compute.RecordError("Transform method has not been implemented for type " + geometry.GetType().Name);
            return null;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static bool IsUniformScaling(this TransformMatrix transform)
        {
            double tol = 1e-6;

            for (int i = 0; i < transform.Matrix.GetLength(0); i++)
                for (int j = 0; j < transform.Matrix.GetLength(1) - 1; j++)
                    if (i != j && Math.Abs(transform.Matrix[i, j]) > tol)
                        return false;

            if (Math.Abs(transform.Matrix[0, 0] - transform.Matrix[1, 1]) <= tol &&
                Math.Abs(transform.Matrix[1, 1] - transform.Matrix[2, 2]) <= tol &&
                Math.Abs(transform.Matrix[3, 3] - 1) <= tol)
                return true;

            return false;
        }

        /***************************************************/
    }
}
