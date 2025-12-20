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

        [Description("Scales a Point by the provided scale vector from the given origin.")]
        [Input("pt", "The Point to scale.")]
        [Input("origin", "The Point to scale from.")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("point", "The scaled Point.")]
        public static Point Scale(this Point pt, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(pt, scaleMatrix);
        }

        /***************************************************/

        [Description("Scales a Vector by the provided scale vector. The origin is not used as vectors are direction-only.")]
        [Input("vector", "The Vector to scale.")]
        [Input("origin", "The origin point (not used for vectors).")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("vector", "The scaled Vector.")]
        public static Vector Scale(this Vector vector, Point origin, Vector scaleVector)
        {
            return new Vector { X = vector.X * scaleVector.X, Y = vector.Y * scaleVector.Y, Z = vector.Z * scaleVector.Z };
        }

        /***************************************************/

        [Description("Scales a Plane by the provided scale vector from the given origin.")]
        [Input("plane", "The Plane to scale.")]
        [Input("origin", "The Point to scale from.")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("plane", "The scaled Plane.")]
        public static Plane Scale(this Plane plane, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(plane, scaleMatrix);
        }
        

        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Scales an Arc by the provided scale vector from the given origin. For non-uniform scaling, the Arc may be converted to a NurbsCurve.")]
        [Input("curve", "The Arc to scale.")]
        [Input("origin", "The Point to scale from.")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("curve", "The scaled curve as an Arc or NurbsCurve.")]
        public static ICurve Scale(this Arc curve, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(curve, scaleMatrix);
        }

        /***************************************************/

        [Description("Scales a Circle by the provided scale vector from the given origin. For non-uniform scaling, the Circle may be converted to a NurbsCurve.")]
        [Input("curve", "The Circle to scale.")]
        [Input("origin", "The Point to scale from.")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("curve", "The scaled curve as a Circle or NurbsCurve.")]
        public static ICurve Scale(this Circle curve, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(curve, scaleMatrix);
        }

        /***************************************************/

        [Description("Scales an Ellipse by the provided scale vector from the given origin.")]
        [Input("curve", "The Ellipse to scale.")]
        [Input("origin", "The Point to scale from.")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("curve", "The scaled Ellipse as an ICurve.")]
        public static ICurve Scale(this Ellipse curve, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(curve, scaleMatrix);
        }

        /***************************************************/

        [Description("Scales a Line by the provided scale vector from the given origin.")]
        [Input("curve", "The Line to scale.")]
        [Input("origin", "The Point to scale from.")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("curve", "The scaled Line.")]
        public static Line Scale(this Line curve, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(curve, scaleMatrix);
        }

        /***************************************************/

        [Description("Scales a NurbsCurve by the provided scale vector from the given origin.")]
        [Input("curve", "The NurbsCurve to scale.")]
        [Input("origin", "The Point to scale from.")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("curve", "The scaled NurbsCurve.")]
        public static NurbsCurve Scale(this NurbsCurve curve, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(curve, scaleMatrix);
        }


        /***************************************************/

        [Description("Scales a PolyCurve by the provided scale vector from the given origin.")]
        [Input("curve", "The PolyCurve to scale.")]
        [Input("origin", "The Point to scale from.")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("curve", "The scaled PolyCurve.")]
        public static PolyCurve Scale(this PolyCurve curve, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(curve, scaleMatrix);
        }

        /***************************************************/

        [Description("Scales a Polyline by the provided scale vector from the given origin.")]
        [Input("curve", "The Polyline to scale.")]
        [Input("origin", "The Point to scale from.")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("curve", "The scaled Polyline.")]
        public static Polyline Scale(this Polyline curve, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(curve, scaleMatrix);
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        [Description("Scales an Extrusion surface by the provided scale vector from the given origin.")]
        [Input("surface", "The Extrusion surface to scale.")]
        [Input("origin", "The Point to scale from.")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("surface", "The scaled Extrusion surface.")]
        public static Extrusion Scale(this Extrusion surface, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(surface, scaleMatrix);
        }

        /***************************************************/

        [Description("Scales a Loft surface by the provided scale vector from the given origin.")]
        [Input("surface", "The Loft surface to scale.")]
        [Input("origin", "The Point to scale from.")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("surface", "The scaled Loft surface.")]
        public static Loft Scale(this Loft surface, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(surface, scaleMatrix);
        }

        /***************************************************/

        [Description("Scales a NurbsSurface by the provided scale vector from the given origin.")]
        [Input("surface", "The NurbsSurface to scale.")]
        [Input("origin", "The Point to scale from.")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("surface", "The scaled NurbsSurface.")]
        public static NurbsSurface Scale(this NurbsSurface surface, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return surface.Transform(scaleMatrix);
        }

        /***************************************************/

        [Description("Scales a Pipe surface by the provided scale vector from the given origin.")]
        [Input("surface", "The Pipe surface to scale.")]
        [Input("origin", "The Point to scale from.")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("surface", "The scaled Pipe surface.")]
        public static Pipe Scale(this Pipe surface, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(surface, scaleMatrix);
        }

        /***************************************************/

        [Description("Scales a PlanarSurface by the provided scale vector from the given origin.")]
        [Input("surface", "The PlanarSurface to scale.")]
        [Input("origin", "The Point to scale from.")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("surface", "The scaled PlanarSurface.")]
        public static PlanarSurface Scale(this PlanarSurface surface, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return surface.Transform(scaleMatrix);
        }

        /***************************************************/

        [Description("Scales a PolySurface by the provided scale vector from the given origin.")]
        [Input("surface", "The PolySurface to scale.")]
        [Input("origin", "The Point to scale from.")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("surface", "The scaled PolySurface.")]
        public static PolySurface Scale(this PolySurface surface, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(surface, scaleMatrix);
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        [Description("Scales a Mesh by the provided scale vector from the given origin.")]
        [Input("mesh", "The Mesh to scale.")]
        [Input("origin", "The Point to scale from.")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("mesh", "The scaled Mesh.")]
        public static Mesh Scale(this Mesh mesh, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(mesh, scaleMatrix);
        }

        /***************************************************/

        [Description("Scales a Cartesian coordinate system by the provided scale vector from the given origin.")]
        [Input("coordinate", "The Cartesian coordinate system to scale.")]
        [Input("origin", "The Point to scale from.")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("coordinate", "The scaled Cartesian coordinate system.")]
        public static Cartesian Scale(this Cartesian coordinate, Point origin, Vector scaleVector)
        {
            return new Cartesian(coordinate.Origin.Scale(origin, scaleVector), coordinate.X, coordinate.Y, coordinate.Z);
        }

        /***************************************************/

        [Description("Scales a CompositeGeometry by the provided scale vector from the given origin.")]
        [Input("group", "The CompositeGeometry to scale.")]
        [Input("origin", "The Point to scale from.")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("group", "The scaled CompositeGeometry.")]
        public static CompositeGeometry Scale(this CompositeGeometry group, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(group, scaleMatrix);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Scales any IGeometry by the provided scale vector from the given origin.")]
        [Input("geometry", "The IGeometry to scale.")]
        [Input("origin", "The Point to scale from.")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("geometry", "The scaled IGeometry.")]
        public static IGeometry IScale(this IGeometry geometry, Point origin, Vector scaleVector)
        {
            return Scale(geometry as dynamic, origin, scaleVector);
        }

        /***************************************************/

        [Description("Scales any ICurve by the provided scale vector from the given origin.")]
        [Input("geometry", "The ICurve to scale.")]
        [Input("origin", "The Point to scale from.")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("geometry", "The scaled ICurve.")]
        public static ICurve IScale(this ICurve geometry, Point origin, Vector scaleVector)
        {
            return Scale(geometry as dynamic, origin, scaleVector);
        }

        /***************************************************/

        [Description("Scales any ISurface by the provided scale vector from the given origin.")]
        [Input("geometry", "The ISurface to scale.")]
        [Input("origin", "The Point to scale from.")]
        [Input("scaleVector", "The Vector defining the scale factors in X, Y, and Z directions.")]
        [Output("geometry", "The scaled ISurface.")]
        public static ISurface IScale(this ISurface geometry, Point origin, Vector scaleVector)
        {
            return Scale(geometry as dynamic, origin, scaleVector);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static IGeometry Scale(this IGeometry geometry, Point origin, Vector scaleVector)
        {
            Base.Compute.RecordError("Scale method has not been implemented for type " + geometry.GetType().Name);
            return null;
        }

        /***************************************************/
    }
}
