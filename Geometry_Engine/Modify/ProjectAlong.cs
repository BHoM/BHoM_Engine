/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
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

        public static Point ProjectAlong(this Point point, Plane plane, Vector vector)
        {
            if (Math.Abs(vector.DotProduct(plane.Normal)) <= Tolerance.Angle)
                return null;

            double t = (plane.Normal * (plane.Origin - point)) / (plane.Normal * vector);
            return point + t * vector;
        }

        /***************************************************/

        public static Point ProjectAlong(this Point point, Line line, Vector vector, double tolerance = Tolerance.Distance)
        {
            Line l = new Line { Start = point, End = point + vector };
            return line.LineIntersection(l, true, tolerance);
        }

        /***************************************************/

        [PreviousInputNames("ToPlane", "toPlane")]
        public static Plane ProjectAlong(this Plane plane, Plane toPlane, Vector vector)
        {
            double dp = plane.Normal.DotProduct(toPlane.Normal);
            if (Math.Abs(dp) <= Tolerance.Angle)
                return null;

            Vector normal = dp > 0 ? toPlane.Normal : toPlane.Normal.Reverse();
            return new Plane { Origin = plane.Origin.ProjectAlong(toPlane, vector), Normal = normal };
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static ICurve ProjectAlong(this Arc arc, Plane plane, Vector vector)
        {
            return arc.ToNurbsCurve().ProjectAlong(plane, vector);
        }

        /***************************************************/

        public static ICurve ProjectAlong(this Circle circle, Plane plane, Vector vector)
        {
            if (circle.Normal.IsParallel(plane.Normal) != 0)
                return new Circle { Centre = circle.Centre.ProjectAlong(plane, vector), Normal = circle.Normal.DeepClone(), Radius = circle.Radius };

            TransformMatrix project = Create.ProjectionMatrix(plane, vector);
            return circle.Transform(project);
        }

        /***************************************************/

        public static ICurve ProjectAlong(this Ellipse ellipse, Plane plane, Vector vector)
        {
            TransformMatrix project = Create.ProjectionMatrix(plane, vector);
            return ellipse.Transform(project);
        }

        /***************************************************/

        public static Line ProjectAlong(this Line line, Plane plane, Vector vector)
        {
            return new Line { Start = line.Start.ProjectAlong(plane, vector), End = line.End.ProjectAlong(plane, vector) };
        }

        /***************************************************/

        public static NurbsCurve ProjectAlong(this NurbsCurve curve, Plane plane, Vector vector)
        {
            return new NurbsCurve()
            {
                ControlPoints = curve.ControlPoints.Select(x => x.ProjectAlong(plane, vector)).ToList(),
                Knots = curve.Knots,
                Weights = curve.Weights
            };
        }

        /***************************************************/

        public static PolyCurve ProjectAlong(this PolyCurve curve, Plane plane, Vector vector)
        {
            return new PolyCurve { Curves = curve.Curves.Select(x => x.IProjectAlong(plane, vector)).ToList() };
        }

        /***************************************************/

        public static Polyline ProjectAlong(this Polyline curve, Plane plane, Vector vector)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.Select(x => x.ProjectAlong(plane, vector)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static Extrusion ProjectAlong(this Extrusion surface, Plane plane, Vector vector)
        {
            return new Extrusion { Curve = surface.Curve.IProjectAlong(plane, vector), Direction = surface.Direction.Project(plane), Capped = surface.Capped };
        }

        /***************************************************/

        public static Loft ProjectAlong(this Loft surface, Plane plane, Vector vector)
        {
            return new Loft { Curves = surface.Curves.Select(x => x.IProjectAlong(plane, vector)).ToList() };
        }

        /***************************************************/

        public static NurbsSurface ProjectAlong(this NurbsSurface surface, Plane plane, Vector vector)
        {
            List<SurfaceTrim> innerTrims = surface.InnerTrims.Select(x => new SurfaceTrim(x.Curve3d.IProjectAlong(plane, vector), x.Curve2d)).ToList();

            List<SurfaceTrim> outerTrims = surface.OuterTrims.Select(x => new SurfaceTrim(x.Curve3d.IProjectAlong(plane, vector), x.Curve2d)).ToList();

            return new NurbsSurface(
                surface.ControlPoints.Select(x => x.ProjectAlong(plane, vector)),
                surface.Weights,
                surface.UKnots,
                surface.VKnots,
                surface.UDegree,
                surface.VDegree,
                innerTrims,
                outerTrims);
        }

        /***************************************************/

        public static PlanarSurface ProjectAlong(this PlanarSurface surface, Plane plane, Vector vector)
        {
            return new PlanarSurface(
                surface.ExternalBoundary.IProjectAlong(plane, vector),
                surface.InternalBoundaries.Select(x => x.IProjectAlong(plane, vector)).ToList());
        }

        /***************************************************/

        public static PolySurface ProjectAlong(this PolySurface surface, Plane plane, Vector vector)
        {
            return new PolySurface { Surfaces = surface.Surfaces.Select(x => x.IProjectAlong(plane, vector)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static Mesh ProjectAlong(this Mesh mesh, Plane plane, Vector vector)
        {
            return new Mesh { Vertices = mesh.Vertices.Select(x => x.ProjectAlong(plane, vector)).ToList(), Faces = mesh.Faces.Select(x => x.DeepClone()).ToList() };
        }

        /***************************************************/

        public static CompositeGeometry ProjectAlong(this CompositeGeometry group, Plane plane, Vector vector)
        {
            return new CompositeGeometry { Elements = group.Elements.Select(x => x.IProjectAlong(plane, vector)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IGeometry IProjectAlong(this IGeometry geometry, Plane plane, Vector vector)
        {
            return ProjectAlong(geometry as dynamic, plane, vector);
        }

        /***************************************************/

        public static ICurve IProjectAlong(this ICurve geometry, Plane plane, Vector vector)
        {
            return ProjectAlong(geometry as dynamic, plane, vector);
        }

        /***************************************************/

        public static ISurface IProjectAlong(this ISurface geometry, Plane plane, Vector vector)
        {
            return ProjectAlong(geometry as dynamic, plane, vector);
        }


        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        private static IGeometry ProjectAlong(this IGeometry geometry, Plane plane, Vector vector)
        {
            Base.Compute.RecordError("ProjectAlong method has not been implemented for type " + geometry.GetType().Name);
            return null;
        }

        /***************************************************/
    }
}


