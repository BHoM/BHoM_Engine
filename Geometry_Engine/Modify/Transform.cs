/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

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

        public static Plane Transform(this Plane plane, TransformMatrix transform)
        {
            return new Plane { Origin = plane.Origin.Transform(transform), Normal = plane.Normal.Transform(transform).Normalise() };
        }

        /***************************************************/

        public static Basis Transform(this Basis basis, TransformMatrix transform)
        {
            return Create.Basis(basis.X.Transform(transform), basis.Y.Transform(transform));
        }

        /***************************************************/

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

        public static Line Transform(this Line curve, TransformMatrix transform)
        {
            return new Line { Start = curve.Start.Transform(transform), End = curve.End.Transform(transform) };
        }

        /***************************************************/

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

        public static PolyCurve Transform(this PolyCurve curve, TransformMatrix transform)
        {
            return new PolyCurve { Curves = curve.Curves.Select(x => x.ITransform(transform)).ToList() };
        }

        /***************************************************/

        public static Polyline Transform(this Polyline curve, TransformMatrix transform)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.Select(x => x.Transform(transform)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static Extrusion Transform(this Extrusion surface, TransformMatrix transform)
        {
            return new Extrusion { Curve = surface.Curve.ITransform(transform), Direction = surface.Direction.Transform(transform), Capped = surface.Capped };
        }

        /***************************************************/

        public static Loft Transform(this Loft surface, TransformMatrix transform)
        {
            return new Loft { Curves = surface.Curves.Select(x => x.ITransform(transform)).ToList() };
        }

        /***************************************************/

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

        public static Pipe Transform(this Pipe surface, TransformMatrix transform)
        {
            return new Pipe { Centreline = surface.Centreline.ITransform(transform), Radius = surface.Radius, Capped = surface.Capped };
        }

        /***************************************************/

        public static PlanarSurface Transform(this PlanarSurface surface, TransformMatrix transform)
        {
            return new PlanarSurface(surface.ExternalBoundary.ITransform(transform), surface.InternalBoundaries.Select(x => x.ITransform(transform)).ToList());
        }

        /***************************************************/

        public static PolySurface Transform(this PolySurface surface, TransformMatrix transform)
        {
            return new PolySurface { Surfaces = surface.Surfaces.Select(x => x.ITransform(transform)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Mesh                     ****/
        /***************************************************/

        public static Mesh Transform(this Mesh mesh, TransformMatrix transform)
        {
            return new Mesh { Vertices = mesh.Vertices.Select(x => x.Transform(transform)).ToList(), Faces = mesh.Faces.ToList() };
        }


        /***************************************************/
        /**** Public Methods - Solid                    ****/
        /***************************************************/

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

        public static CompositeGeometry Transform(this CompositeGeometry group, TransformMatrix transform)
        {
            return new CompositeGeometry { Elements = group.Elements.Select(x => x.ITransform(transform)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IGeometry ITransform(this IGeometry geometry, TransformMatrix transform)
        {
            return Transform(geometry as dynamic, transform);
        }

        /***************************************************/

        public static ICurve ITransform(this ICurve geometry, TransformMatrix transform)
        {
            return Transform(geometry as dynamic, transform);
        }

        /***************************************************/

        public static ISurface ITransform(this ISurface geometry, TransformMatrix transform)
        {
            return Transform(geometry as dynamic, transform);
        }

        /***************************************************/

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




