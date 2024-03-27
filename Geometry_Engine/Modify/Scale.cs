/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static Point Scale(this Point pt, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(pt, scaleMatrix);
        }

        /***************************************************/

        public static Vector Scale(this Vector vector, Point origin, Vector scaleVector)
        {
            return new Vector { X = vector.X * scaleVector.X, Y = vector.Y * scaleVector.Y, Z = vector.Z * scaleVector.Z };
        }

        /***************************************************/

        public static Plane Scale(this Plane plane, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(plane, scaleMatrix);
        }
        

        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static ICurve Scale(this Arc curve, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(curve, scaleMatrix);
        }

        /***************************************************/

        public static ICurve Scale(this Circle curve, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(curve, scaleMatrix);
        }

        /***************************************************/

        public static ICurve Scale(this Ellipse curve, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(curve, scaleMatrix);
        }

        /***************************************************/
        
        public static Line Scale(this Line curve, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(curve, scaleMatrix);
        }

        /***************************************************/

        public static NurbsCurve Scale(this NurbsCurve curve, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(curve, scaleMatrix);
        }


        /***************************************************/

        public static PolyCurve Scale(this PolyCurve curve, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(curve, scaleMatrix);
        }

        /***************************************************/

        public static Polyline Scale(this Polyline curve, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(curve, scaleMatrix);
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static Extrusion Scale(this Extrusion surface, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(surface, scaleMatrix);
        }

        /***************************************************/

        public static Loft Scale(this Loft surface, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(surface, scaleMatrix);
        }

        /***************************************************/

        public static NurbsSurface Scale(this NurbsSurface surface, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return surface.Transform(scaleMatrix);
        }

        /***************************************************/

        public static Pipe Scale(this Pipe surface, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(surface, scaleMatrix);
        }

        /***************************************************/

        public static PlanarSurface Scale(this PlanarSurface surface, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return surface.Transform(scaleMatrix);
        }

        /***************************************************/

        public static PolySurface Scale(this PolySurface surface, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(surface, scaleMatrix);
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static Mesh Scale(this Mesh mesh, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(mesh, scaleMatrix);
        }

        /***************************************************/

        public static Cartesian Scale(this Cartesian coordinate, Point origin, Vector scaleVector)
        {
            return new Cartesian(coordinate.Origin.Scale(origin, scaleVector), coordinate.X, coordinate.Y, coordinate.Z);
        }

        /***************************************************/

        public static CompositeGeometry Scale(this CompositeGeometry group, Point origin, Vector scaleVector)
        {
            TransformMatrix scaleMatrix = Create.ScaleMatrix(origin, scaleVector);
            return Transform(group, scaleMatrix);
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IGeometry IScale(this IGeometry geometry, Point origin, Vector scaleVector)
        {
            return Scale(geometry as dynamic, origin, scaleVector);
        }

        /***************************************************/

        public static ICurve IScale(this ICurve geometry, Point origin, Vector scaleVector)
        {
            return Scale(geometry as dynamic, origin, scaleVector);
        }

        /***************************************************/

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




