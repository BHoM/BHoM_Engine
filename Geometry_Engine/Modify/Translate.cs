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

        public static Point Translate(this Point pt, Vector transform)
        {
            return pt + transform;
        }

        /***************************************************/

        public static Vector Translate(this Vector vector, Vector transform)
        {
            return new Vector { X = vector.X, Y = vector.Y, Z = vector.Z };
        }

        /***************************************************/

        public static Plane Translate(this Plane plane, Vector transform)
        {
            return new Plane { Origin = plane.Origin + transform, Normal = plane.Normal };
        }

        /***************************************************/

        public static Cartesian Translate(this Cartesian coordinateSystem, Vector transform)
        {
            return new Cartesian(coordinateSystem.Origin + transform, coordinateSystem.X, coordinateSystem.Y, coordinateSystem.Z);
        }

        /***************************************************/
        /**** Public Methods - Curves                  ****/
        /***************************************************/

        public static Arc Translate(this Arc curve, Vector transform)
        {
            return new Arc
            {
                CoordinateSystem = curve.CoordinateSystem.Translate(transform),
                Radius = curve.Radius,
                StartAngle = curve.StartAngle,
                EndAngle = curve.EndAngle
            };
        }

        /***************************************************/

        public static Circle Translate(this Circle curve, Vector transform)
        {
            return new Circle { Centre = curve.Centre + transform, Normal = curve.Normal, Radius = curve.Radius };
        }

        /***************************************************/

        public static ICurve Translate(this Ellipse curve, Vector transform)
        {
            TransformMatrix translationMatrix = Create.TranslationMatrix(transform);
            return Transform(curve, translationMatrix);
        }

        /***************************************************/

        public static Line Translate(this Line curve, Vector transform)
        {
            return new Line { Start = curve.Start + transform, End = curve.End + transform };
        }

        /***************************************************/

        public static NurbsCurve Translate(this NurbsCurve curve, Vector transform)
        {
            TransformMatrix translationMatrix = Create.TranslationMatrix(transform);
            return Transform(curve, translationMatrix);
        }


        /***************************************************/

        public static PolyCurve Translate(this PolyCurve curve, Vector transform)
        {
            return new PolyCurve { Curves = curve.Curves.Select(x => x.ITranslate(transform)).ToList() };
        }

        /***************************************************/

        public static Polyline Translate(this Polyline curve, Vector transform)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.Select(x => x + transform).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Surfaces                ****/
        /***************************************************/

        public static Extrusion Translate(this Extrusion surface, Vector transform)
        {
            return new Extrusion { Curve = surface.Curve.ITranslate(transform), Direction = surface.Direction, Capped = surface.Capped };
        }

        /***************************************************/

        public static Loft Translate(this Loft surface, Vector transform)
        {
            return new Loft { Curves = surface.Curves.Select(x => x.ITranslate(transform)).ToList() };
        }

        /***************************************************/

        public static NurbsSurface Translate(this NurbsSurface surface, Vector transform)
        {
            TransformMatrix translationMatrix = Create.TranslationMatrix(transform);
            return Transform(surface, translationMatrix);
        }

        /***************************************************/

        public static Pipe Translate(this Pipe surface, Vector transform)
        {
            return new Pipe { Centreline = surface.Centreline.ITranslate(transform), Radius = surface.Radius, Capped = surface.Capped };
        }

        /***************************************************/

        public static PlanarSurface Translate(this PlanarSurface surface, Vector transform)
        {
            return new PlanarSurface(surface.ExternalBoundary.ITranslate(transform), surface.InternalBoundaries.Select(x => x.ITranslate(transform)).ToList());
        }

        /***************************************************/

        public static PolySurface Translate(this PolySurface surface, Vector transform)
        {
            return new PolySurface { Surfaces = surface.Surfaces.Select(x => x.ITranslate(transform)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Others                  ****/
        /***************************************************/

        public static Mesh Translate(this Mesh mesh, Vector transform)
        {
            return new Mesh { Vertices = mesh.Vertices.Select(x => x + transform).ToList(), Faces = mesh.Faces.ToList() };
        }

        /***************************************************/

        public static CompositeGeometry Translate(this CompositeGeometry group, Vector transform)
        {
            return new CompositeGeometry { Elements = group.Elements.Select(x => x.ITranslate(transform)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IGeometry ITranslate(this IGeometry geometry, Vector transform)
        {
            return Translate(geometry as dynamic, transform);
        }

        /***************************************************/

        public static ICurve ITranslate(this ICurve geometry, Vector transform)
        {
            return Translate(geometry as dynamic, transform);
        }

        /***************************************************/

        public static ISurface ITranslate(this ISurface geometry, Vector transform)
        {
            return Translate(geometry as dynamic, transform);
        }

        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        private static IGeometry Translate(this IGeometry geometry, Vector transform)
        {
            Base.Compute.RecordError("Translate method has not been implemented for type " + geometry.GetType().Name);
            return null;
        }

        /***************************************************/
    }
}





