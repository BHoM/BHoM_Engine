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
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        [Description("Translates a Point by the provided Vector.")]
        [Input("pt", "The Point to translate.")]
        [Input("transform", "The Vector to translate the Point by.")]
        [Output("point", "The translated Point.")]
        public static Point Translate(this Point pt, Vector transform)
        {
            return pt + transform;
        }

        /***************************************************/

        [Description("Translates a Vector by the provided Vector. Note: This method returns the original Vector unchanged as translation does not affect vectors.")]
        [Input("vector", "The Vector to translate.")]
        [Input("transform", "The translation Vector (not applied to vectors).")]
        [Output("vector", "The original Vector unchanged.")]
        public static Vector Translate(this Vector vector, Vector transform)
        {
            return new Vector { X = vector.X, Y = vector.Y, Z = vector.Z };
        }

        /***************************************************/

        [Description("Translates a Plane by the provided Vector.")]
        [Input("plane", "The Plane to translate.")]
        [Input("transform", "The Vector to translate the Plane by.")]
        [Output("plane", "The translated Plane.")]
        public static Plane Translate(this Plane plane, Vector transform)
        {
            return new Plane { Origin = plane.Origin + transform, Normal = plane.Normal };
        }

        /***************************************************/

        [Description("Translates a Cartesian coordinate system by the provided Vector.")]
        [Input("coordinateSystem", "The Cartesian coordinate system to translate.")]
        [Input("transform", "The Vector to translate the coordinate system by.")]
        [Output("coordinateSystem", "The translated Cartesian coordinate system.")]
        public static Cartesian Translate(this Cartesian coordinateSystem, Vector transform)
        {
            return new Cartesian(coordinateSystem.Origin + transform, coordinateSystem.X, coordinateSystem.Y, coordinateSystem.Z);
        }

        /***************************************************/
        /**** Public Methods - Curves                  ****/
        /***************************************************/

        [Description("Translates an Arc by the provided Vector.")]
        [Input("curve", "The Arc to translate.")]
        [Input("transform", "The Vector to translate the Arc by.")]
        [Output("curve", "The translated Arc.")]
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

        [Description("Translates a Circle by the provided Vector.")]
        [Input("curve", "The Circle to translate.")]
        [Input("transform", "The Vector to translate the Circle by.")]
        [Output("curve", "The translated Circle.")]
        public static Circle Translate(this Circle curve, Vector transform)
        {
            return new Circle { Centre = curve.Centre + transform, Normal = curve.Normal, Radius = curve.Radius };
        }

        /***************************************************/

        [Description("Translates an Ellipse by the provided Vector.")]
        [Input("curve", "The Ellipse to translate.")]
        [Input("transform", "The Vector to translate the Ellipse by.")]
        [Output("curve", "The translated Ellipse as an ICurve.")]
        public static ICurve Translate(this Ellipse curve, Vector transform)
        {
            TransformMatrix translationMatrix = Create.TranslationMatrix(transform);
            return Transform(curve, translationMatrix);
        }

        /***************************************************/

        [Description("Translates a Line by the provided Vector.")]
        [Input("curve", "The Line to translate.")]
        [Input("transform", "The Vector to translate the Line by.")]
        [Output("curve", "The translated Line.")]
        public static Line Translate(this Line curve, Vector transform)
        {
            return new Line { Start = curve.Start + transform, End = curve.End + transform };
        }

        /***************************************************/

        [Description("Translates a NurbsCurve by the provided Vector.")]
        [Input("curve", "The NurbsCurve to translate.")]
        [Input("transform", "The Vector to translate the NurbsCurve by.")]
        [Output("curve", "The translated NurbsCurve.")]
        public static NurbsCurve Translate(this NurbsCurve curve, Vector transform)
        {
            TransformMatrix translationMatrix = Create.TranslationMatrix(transform);
            return Transform(curve, translationMatrix);
        }


        /***************************************************/

        [Description("Translates a PolyCurve by the provided Vector.")]
        [Input("curve", "The PolyCurve to translate.")]
        [Input("transform", "The Vector to translate the PolyCurve by.")]
        [Output("curve", "The translated PolyCurve.")]
        public static PolyCurve Translate(this PolyCurve curve, Vector transform)
        {
            return new PolyCurve { Curves = curve.Curves.Select(x => x.ITranslate(transform)).ToList() };
        }

        /***************************************************/

        [Description("Translates a Polyline by the provided Vector.")]
        [Input("curve", "The Polyline to translate.")]
        [Input("transform", "The Vector to translate the Polyline by.")]
        [Output("curve", "The translated Polyline.")]
        public static Polyline Translate(this Polyline curve, Vector transform)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.Select(x => x + transform).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Surfaces                ****/
        /***************************************************/

        [Description("Translates an Extrusion surface by the provided Vector.")]
        [Input("surface", "The Extrusion surface to translate.")]
        [Input("transform", "The Vector to translate the Extrusion by.")]
        [Output("surface", "The translated Extrusion surface.")]
        public static Extrusion Translate(this Extrusion surface, Vector transform)
        {
            return new Extrusion { Curve = surface.Curve.ITranslate(transform), Direction = surface.Direction, Capped = surface.Capped };
        }

        /***************************************************/

        [Description("Translates a Loft surface by the provided Vector.")]
        [Input("surface", "The Loft surface to translate.")]
        [Input("transform", "The Vector to translate the Loft by.")]
        [Output("surface", "The translated Loft surface.")]
        public static Loft Translate(this Loft surface, Vector transform)
        {
            return new Loft { Curves = surface.Curves.Select(x => x.ITranslate(transform)).ToList() };
        }

        /***************************************************/

        [Description("Translates a NurbsSurface by the provided Vector.")]
        [Input("surface", "The NurbsSurface to translate.")]
        [Input("transform", "The Vector to translate the NurbsSurface by.")]
        [Output("surface", "The translated NurbsSurface.")]
        public static NurbsSurface Translate(this NurbsSurface surface, Vector transform)
        {
            TransformMatrix translationMatrix = Create.TranslationMatrix(transform);
            return Transform(surface, translationMatrix);
        }

        /***************************************************/

        [Description("Translates a Pipe surface by the provided Vector.")]
        [Input("surface", "The Pipe surface to translate.")]
        [Input("transform", "The Vector to translate the Pipe by.")]
        [Output("surface", "The translated Pipe surface.")]
        public static Pipe Translate(this Pipe surface, Vector transform)
        {
            return new Pipe { Centreline = surface.Centreline.ITranslate(transform), Radius = surface.Radius, Capped = surface.Capped };
        }

        /***************************************************/

        [Description("Translates a PlanarSurface by the provided Vector.")]
        [Input("surface", "The PlanarSurface to translate.")]
        [Input("transform", "The Vector to translate the PlanarSurface by.")]
        [Output("surface", "The translated PlanarSurface.")]
        public static PlanarSurface Translate(this PlanarSurface surface, Vector transform)
        {
            return new PlanarSurface(surface.ExternalBoundary.ITranslate(transform), surface.InternalBoundaries.Select(x => x.ITranslate(transform)).ToList());
        }

        /***************************************************/

        [Description("Translates a PolySurface by the provided Vector.")]
        [Input("surface", "The PolySurface to translate.")]
        [Input("transform", "The Vector to translate the PolySurface by.")]
        [Output("surface", "The translated PolySurface.")]
        public static PolySurface Translate(this PolySurface surface, Vector transform)
        {
            return new PolySurface { Surfaces = surface.Surfaces.Select(x => x.ITranslate(transform)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Others                  ****/
        /***************************************************/

        [Description("Translates a Mesh by the provided Vector.")]
        [Input("mesh", "The Mesh to translate.")]
        [Input("transform", "The Vector to translate the Mesh by.")]
        [Output("mesh", "The translated Mesh.")]
        public static Mesh Translate(this Mesh mesh, Vector transform)
        {
            return new Mesh { Vertices = mesh.Vertices.Select(x => x + transform).ToList(), Faces = mesh.Faces.ToList() };
        }

        /***************************************************/

        [Description("Translates a CompositeGeometry by the provided Vector.")]
        [Input("group", "The CompositeGeometry to translate.")]
        [Input("transform", "The Vector to translate the CompositeGeometry by.")]
        [Output("group", "The translated CompositeGeometry.")]
        public static CompositeGeometry Translate(this CompositeGeometry group, Vector transform)
        {
            return new CompositeGeometry { Elements = group.Elements.Select(x => x.ITranslate(transform)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Translates any IGeometry by the provided Vector.")]
        [Input("geometry", "The IGeometry to translate.")]
        [Input("transform", "The Vector to translate the geometry by.")]
        [Output("geometry", "The translated IGeometry.")]
        public static IGeometry ITranslate(this IGeometry geometry, Vector transform)
        {
            return Translate(geometry as dynamic, transform);
        }

        /***************************************************/

        [Description("Translates any ICurve by the provided Vector.")]
        [Input("geometry", "The ICurve to translate.")]
        [Input("transform", "The Vector to translate the curve by.")]
        [Output("geometry", "The translated ICurve.")]
        public static ICurve ITranslate(this ICurve geometry, Vector transform)
        {
            return Translate(geometry as dynamic, transform);
        }

        /***************************************************/

        [Description("Translates any ISurface by the provided Vector.")]
        [Input("geometry", "The ISurface to translate.")]
        [Input("transform", "The Vector to translate the surface by.")]
        [Output("geometry", "The translated ISurface.")]
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
