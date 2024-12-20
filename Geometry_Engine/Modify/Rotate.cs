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

using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Base.Attributes;
using System;
using System.ComponentModel;
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
            if (vector == null || axis == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot rotate vector as it and/or the axis vector is null.");
                return null;
            }

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

        public static Basis Rotate(this Basis basis, double rad, Vector axis)
        {
            return Create.Basis(basis.X.Rotate(rad, axis), basis.Y.Rotate(rad, axis));
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Arc Rotate(this Arc curve, Point origin, Vector axis, double rad)
        {
            return new Arc
            {
                CoordinateSystem = curve.CoordinateSystem.Rotate(origin, axis, rad),
                Radius = curve.Radius,
                StartAngle = curve.StartAngle,
                EndAngle = curve.EndAngle
            };
        }

        /***************************************************/

        public static Circle Rotate(this Circle curve, Point origin, Vector axis, double rad)
        {
            return new Circle { Centre = curve.Centre.Rotate(origin, axis, rad), Normal = curve.Normal.Rotate(rad, axis), Radius = curve.Radius };
        }

        /***************************************************/

        public static ICurve Rotate(this Ellipse curve, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(curve, rotationMatrix);
        }

        /***************************************************/

        public static Line Rotate(this Line curve, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(curve, rotationMatrix);
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

        public static PlanarSurface Rotate(this PlanarSurface surface, Point origin, Vector axis, double rad)
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
            return Rotate(geometry as dynamic, origin, axis, rad);
        }

        /***************************************************/

        public static ICurve IRotate(this ICurve geometry, Point origin, Vector axis, double rad)
        {
            return Rotate(geometry as dynamic, origin, axis, rad);
        }

        /***************************************************/

        public static ISurface IRotate(this ISurface geometry, Point origin, Vector axis, double rad)
        {
            return Rotate(geometry as dynamic, origin, axis, rad);
        }

        /***************************************************/
        /**** Private Methods - Interfaces              ****/
        /***************************************************/

        [Description("Some objects have no use for origin, this method will make them calleble from the interface method.")]
        private static IGeometry Rotate(this IGeometry geometry, Point origin, Vector axis, double rad)
        {
            return Rotate(geometry as dynamic, rad, axis);
        }

        /***************************************************/

        private static IGeometry Rotate(this IGeometry geometry, double rad, Vector axis)
        {
            Engine.Base.Compute.RecordError("Rotate method has not been implemented for type " + geometry.GetType().Name);
            return null;
        }

        /***************************************************/
    }
}





