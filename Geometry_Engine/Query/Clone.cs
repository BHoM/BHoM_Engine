/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static Plane Clone(this Plane plane)
        {
            return new Plane { Origin = plane.Origin.DeepClone(), Normal = plane.Normal.DeepClone() };
        }

        /***************************************************/

        public static Point Clone(this Point point)
        {
            return new Point { X = point.X, Y = point.Y, Z = point.Z };
        }

        /***************************************************/

        public static Vector Clone(this Vector vector)
        {
            return new Vector { X = vector.X, Y = vector.Y, Z = vector.Z };
        }

        /***************************************************/

        public static Cartesian Clone(this Cartesian coordinateSystem)
        {
            return new Cartesian(coordinateSystem.Origin.DeepClone(), coordinateSystem.X.DeepClone(), coordinateSystem.Y.DeepClone(), coordinateSystem.Z.DeepClone());
        }

        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Arc Clone(this Arc arc)
        {
            return new Arc { CoordinateSystem = arc.CoordinateSystem.DeepClone(), StartAngle = arc.StartAngle, EndAngle = arc.EndAngle, Radius = arc.Radius };
        }

        /***************************************************/

        public static Circle Clone(this Circle circle)
        {
            return new Circle { Centre = circle.Centre.DeepClone(), Normal = circle.Normal.DeepClone(), Radius = circle.Radius };
        }

        /***************************************************/

        public static Ellipse Clone(this Ellipse ellipse)
        {
            return new Ellipse { Axis1 = ellipse.Axis1, Axis2 = ellipse.Axis2, Centre = ellipse.Centre, Radius1 = ellipse.Radius1, Radius2 = ellipse.Radius2 };
        }

        /***************************************************/

        public static Line Clone(this Line line)
        {
            return new Line { Start = line.Start.DeepClone(), End = line.End.DeepClone(), Infinite = line.Infinite };
        }

        /***************************************************/

        public static NurbsCurve Clone(this NurbsCurve curve)
        {
            return new NurbsCurve { ControlPoints = curve.ControlPoints.Select(x => x.DeepClone()).ToList(), Weights = curve.Weights.ToList(), Knots = curve.Knots.ToList() };
        }

        /***************************************************/

        public static PolyCurve Clone(this PolyCurve curve)
        {
            return new PolyCurve { Curves = curve.Curves.Select(x => x.DeepClone()).ToList() };
        }

        /***************************************************/

        public static Polyline Clone(this Polyline curve)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.Select(x => x.DeepClone()).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static Extrusion Clone(this Extrusion surface)
        {
            return new Extrusion { Curve = surface.Curve.DeepClone(), Direction = surface.Direction.DeepClone(), Capped = surface.Capped };
        }

        /***************************************************/

        public static Loft Clone(this Loft surface)
        {
            return new Loft { Curves = surface.Curves.Select(x => x.DeepClone()).ToList() };
        }

        /***************************************************/

        public static NurbsSurface Clone(this NurbsSurface surface)
        {
            return new NurbsSurface(surface.ControlPoints.Select(x => x.DeepClone()), new List<double>(surface.Weights), new List<double>(surface.UKnots), new List<double>(surface.VKnots), surface.UDegree, surface.VDegree, surface.InnerTrims.Select(x => x.DeepClone()), surface.OuterTrims.Select(x => x.DeepClone()));
        }

        /***************************************************/

        public static PlanarSurface Clone(this PlanarSurface surface)
        {
            return new PlanarSurface(surface.ExternalBoundary.DeepClone(), surface.InternalBoundaries.Select(x => x.DeepClone()).ToList());
        }

        /***************************************************/

        public static Pipe Clone(this Pipe surface)
        {
            return new Pipe { Centreline = surface.Centreline.DeepClone(), Radius = surface.Radius, Capped = surface.Capped };
        }

        /***************************************************/

        public static PolySurface Clone(this PolySurface surface)
        {
            return new PolySurface { Surfaces = surface.Surfaces.Select(x => x.DeepClone()).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static Mesh Clone(this Mesh mesh)
        {
            return new Mesh { Vertices = mesh.Vertices.Select(x => x.DeepClone()).ToList(), Faces = mesh.Faces.Select(x => x.DeepClone()).ToList() };
        }

        /***************************************************/

        public static Face Clone(this Face face)
        {
            return new Face { A = face.A, B = face.B, C = face.C, D = face.D };
        }

        /***************************************************/

        public static BoundingBox Clone(this BoundingBox box)
        {
            return new BoundingBox { Min = box.Min.DeepClone(), Max = box.Max.DeepClone() };
        }

        /***************************************************/

        public static CompositeGeometry Clone(this CompositeGeometry group)
        {
            return new CompositeGeometry { Elements = group.Elements.Select(x => x.DeepClone()).ToList() };
        }

        /***************************************************/

        public static Quaternion Clone(this Quaternion quaternion)
        {
            return new Quaternion { W = quaternion.W, X = quaternion.X, Y = quaternion.Y, Z = quaternion.Z };
        }

        /***************************************************/

        public static TransformMatrix Clone(this TransformMatrix transform)
        {
            return new TransformMatrix { Matrix = transform.Matrix };
        }

        /***************************************************/

        public static Basis Clone(this Basis basis)
        {
            return new Basis(basis.X, basis.Y, basis.Z);
        }

        /***************************************************/

        public static SurfaceTrim Clone(this SurfaceTrim trim)
        {
            return new SurfaceTrim(trim.Curve3d.DeepClone(), trim.Curve2d.DeepClone());
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static object Clone(this object geometry)
        {
            return geometry;
        }

        /***************************************************/

        private static ICurve Clone(this ICurve geometry)
        {
            return geometry;
        }

        /***************************************************/

        private static ISurface Clone(this ISurface geometry)
        {
            return geometry;
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IGeometry IClone(this IGeometry geometry)
        {
            return Clone(geometry as dynamic);
        }

        /***************************************************/

        public static ICurve IClone(this ICurve curve)
        {
            return Clone(curve as dynamic);
        }

        /***************************************************/

        public static ISurface IClone(this ISurface surface)
        {
            return Clone(surface as dynamic);
        }

        /***************************************************/
    }
}

