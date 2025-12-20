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

using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
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

        [Description("Rotates a Point around the given origin and axis by the specified angle in radians.")]
        [Input("pt", "The Point to rotate.")]
        [Input("origin", "The Point to rotate around.")]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Output("point", "The rotated Point.")]
        public static Point Rotate(this Point pt, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(pt, rotationMatrix);
        }

        /***************************************************/

        [Description("Rotates a Vector around the given axis by the specified angle in radians using Rodrigues' rotation formula.")]
        [Input("vector", "The Vector to rotate.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Output("vector", "The rotated Vector.")]
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

        [Description("Rotates a Plane around the given origin and axis by the specified angle in radians.")]
        [Input("plane", "The Plane to rotate.")]
        [Input("origin", "The Point to rotate around.")]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Output("plane", "The rotated Plane.")]
        public static Plane Rotate(this Plane plane, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(plane, rotationMatrix);
        }

        /***************************************************/

        [Description("Rotates a Basis around the given axis by the specified angle in radians.")]
        [Input("basis", "The Basis to rotate.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Output("basis", "The rotated Basis.")]
        public static Basis Rotate(this Basis basis, double rad, Vector axis)
        {
            return Create.Basis(basis.X.Rotate(rad, axis), basis.Y.Rotate(rad, axis));
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Rotates an Arc around the given origin and axis by the specified angle in radians.")]
        [Input("curve", "The Arc to rotate.")]
        [Input("origin", "The Point to rotate around.")]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Output("curve", "The rotated Arc.")]
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

        [Description("Rotates a Circle around the given origin and axis by the specified angle in radians.")]
        [Input("curve", "The Circle to rotate.")]
        [Input("origin", "The Point to rotate around.")]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Output("curve", "The rotated Circle.")]
        public static Circle Rotate(this Circle curve, Point origin, Vector axis, double rad)
        {
            return new Circle { Centre = curve.Centre.Rotate(origin, axis, rad), Normal = curve.Normal.Rotate(rad, axis), Radius = curve.Radius };
        }

        /***************************************************/

        [Description("Rotates an Ellipse around the given origin and axis by the specified angle in radians.")]
        [Input("curve", "The Ellipse to rotate.")]
        [Input("origin", "The Point to rotate around.")]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Output("curve", "The rotated Ellipse as an ICurve.")]
        public static ICurve Rotate(this Ellipse curve, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(curve, rotationMatrix);
        }

        /***************************************************/

        [Description("Rotates a Line around the given origin and axis by the specified angle in radians.")]
        [Input("curve", "The Line to rotate.")]
        [Input("origin", "The Point to rotate around.")]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Output("curve", "The rotated Line.")]
        public static Line Rotate(this Line curve, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(curve, rotationMatrix);
        }

        /***************************************************/

        [Description("Rotates a NurbsCurve around the given origin and axis by the specified angle in radians.")]
        [Input("curve", "The NurbsCurve to rotate.")]
        [Input("origin", "The Point to rotate around.")]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Output("curve", "The rotated NurbsCurve.")]
        public static NurbsCurve Rotate(this NurbsCurve curve, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(curve, rotationMatrix);
        }


        /***************************************************/

        [Description("Rotates a PolyCurve around the given origin and axis by the specified angle in radians.")]
        [Input("curve", "The PolyCurve to rotate.")]
        [Input("origin", "The Point to rotate around.")]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Output("curve", "The rotated PolyCurve.")]
        public static PolyCurve Rotate(this PolyCurve curve, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(curve, rotationMatrix);
        }

        /***************************************************/

        [Description("Rotates a Polyline around the given origin and axis by the specified angle in radians.")]
        [Input("curve", "The Polyline to rotate.")]
        [Input("origin", "The Point to rotate around.")]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Output("curve", "The rotated Polyline.")]
        public static Polyline Rotate(this Polyline curve, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(curve, rotationMatrix);
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        [Description("Rotates an Extrusion surface around the given origin and axis by the specified angle in radians.")]
        [Input("surface", "The Extrusion surface to rotate.")]
        [Input("origin", "The Point to rotate around.")]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Output("surface", "The rotated Extrusion surface.")]
        public static Extrusion Rotate(this Extrusion surface, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(surface, rotationMatrix);
        }

        /***************************************************/

        [Description("Rotates a Loft surface around the given origin and axis by the specified angle in radians.")]
        [Input("surface", "The Loft surface to rotate.")]
        [Input("origin", "The Point to rotate around.")]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Output("surface", "The rotated Loft surface.")]
        public static Loft Rotate(this Loft surface, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(surface, rotationMatrix);
        }

        /***************************************************/

        [Description("Rotates a NurbsSurface around the given origin and axis by the specified angle in radians.")]
        [Input("surface", "The NurbsSurface to rotate.")]
        [Input("origin", "The Point to rotate around.")]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Output("surface", "The rotated NurbsSurface.")]
        public static NurbsSurface Rotate(this NurbsSurface surface, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(surface, rotationMatrix);
        }

        /***************************************************/

        [Description("Rotates a Pipe surface around the given origin and axis by the specified angle in radians.")]
        [Input("surface", "The Pipe surface to rotate.")]
        [Input("origin", "The Point to rotate around.")]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Output("surface", "The rotated Pipe surface.")]
        public static Pipe Rotate(this Pipe surface, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(surface, rotationMatrix);
        }

        /***************************************************/

        [Description("Rotates a PlanarSurface around the given origin and axis by the specified angle in radians.")]
        [Input("surface", "The PlanarSurface to rotate.")]
        [Input("origin", "The Point to rotate around.")]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Output("surface", "The rotated PlanarSurface.")]
        public static PlanarSurface Rotate(this PlanarSurface surface, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(surface, rotationMatrix);
        }

        /***************************************************/

        [Description("Rotates a PolySurface around the given origin and axis by the specified angle in radians.")]
        [Input("surface", "The PolySurface to rotate.")]
        [Input("origin", "The Point to rotate around.")]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Output("surface", "The rotated PolySurface.")]
        public static PolySurface Rotate(this PolySurface surface, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(surface, rotationMatrix);
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        [Description("Rotates a Mesh around the given origin and axis by the specified angle in radians.")]
        [Input("mesh", "The Mesh to rotate.")]
        [Input("origin", "The Point to rotate around.")]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Output("mesh", "The rotated Mesh.")]
        public static Mesh Rotate(this Mesh mesh, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(mesh, rotationMatrix);
        }

        /***************************************************/

        [Description("Rotates a CompositeGeometry around the given origin and axis by the specified angle in radians.")]
        [Input("group", "The CompositeGeometry to rotate.")]
        [Input("origin", "The Point to rotate around.")]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Output("group", "The rotated CompositeGeometry.")]
        public static CompositeGeometry Rotate(this CompositeGeometry group, Point origin, Vector axis, double rad)
        {
            TransformMatrix rotationMatrix = Create.RotationMatrix(origin, axis, rad);
            return Transform(group, rotationMatrix);
        }

        /***************************************************/

        [Description("Rotates a Cartesian coordinate system around the given origin and axis by the specified angle in radians.")]
        [Input("coordinate", "The Cartesian coordinate system to rotate.")]
        [Input("origin", "The Point to rotate around.")]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Output("coordinate", "The rotated Cartesian coordinate system.")]
        public static Cartesian Rotate(this Cartesian coordinate, Point origin, Vector axis, double rad)
        {
            return new Cartesian(coordinate.Origin.Rotate(origin, axis, rad), coordinate.X.Rotate(rad, axis), coordinate.Y.Rotate(rad, axis), coordinate.Z.Rotate(rad, axis));
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Rotates any IGeometry around the given origin and axis by the specified angle in radians.")]
        [Input("geometry", "The IGeometry to rotate.")]
        [Input("origin", "The Point to rotate around.")]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Output("geometry", "The rotated IGeometry.")]
        public static IGeometry IRotate(this IGeometry geometry, Point origin, Vector axis, double rad)
        {
            return Rotate(geometry as dynamic, origin, axis, rad);
        }

        /***************************************************/

        [Description("Rotates any ICurve around the given origin and axis by the specified angle in radians.")]
        [Input("geometry", "The ICurve to rotate.")]
        [Input("origin", "The Point to rotate around.")]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Output("geometry", "The rotated ICurve.")]
        public static ICurve IRotate(this ICurve geometry, Point origin, Vector axis, double rad)
        {
            return Rotate(geometry as dynamic, origin, axis, rad);
        }

        /***************************************************/

        [Description("Rotates any ISurface around the given origin and axis by the specified angle in radians.")]
        [Input("geometry", "The ISurface to rotate.")]
        [Input("origin", "The Point to rotate around.")]
        [Input("axis", "The Vector defining the axis of rotation.")]
        [Input("rad", "The angle of rotation in radians.", typeof(Angle))]
        [Output("geometry", "The rotated ISurface.")]
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
