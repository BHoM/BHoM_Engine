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
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** public Methods - Vectors                  ****/
        /***************************************************/

        [Description("Mirrors the point about the specified plane.")]
        [Input("pt", "The point to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("pt", "The mirrored point.")]
        public static Point Mirror(this Point pt, Plane p)
        {
            return pt - 2 * p.Normal.DotProduct(pt - p.Origin) * p.Normal;
        }

        /***************************************************/

        [Description("Mirrors the vector about the specified plane.")]
        [Input("vector", "The vector to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("vector", "The mirrored vector.")]
        public static Vector Mirror(this Vector vector, Plane p)
        {
            return vector - 2 * vector.DotProduct(p.Normal) * p.Normal;
        }

        /***************************************************/

        [Description("Mirrors the plane about the specified plane.")]
        [Input("plane", "The plane to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("plane", "The mirrored plane.")]
        public static Plane Mirror(this Plane plane, Plane p)
        {
            return new Plane { Origin = plane.Origin.Mirror(p), Normal = plane.Normal.Mirror(p) };
        }

        /***************************************************/

        [Description("Mirrors the basis about the specified plane.")]
        [Input("basis", "The basis to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("basis", "The mirrored basis.")]
        public static Basis Mirror(this Basis basis, Plane p)
        {
            return new Basis(basis.X.Mirror(p), basis.Y.Mirror(p), basis.Z.Mirror(p));
        }

        /***************************************************/

        [Description("Mirrors the cartesian coordinate system about the specified plane.")]
        [Input("coordinateSystem", "The cartesian coordinate system to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("coordinateSystem", "The mirrored cartesian coordinate system.")]
        public static Cartesian Mirror(this Cartesian coordinateSystem, Plane p)
        {
            return Create.CartesianCoordinateSystem(coordinateSystem.Origin.Mirror(p), coordinateSystem.X.Mirror(p), coordinateSystem.Y.Mirror(p));
        }


        /***************************************************/
        /**** public Methods - Curves                  ****/
        /***************************************************/

        [Description("Mirrors the arc about the specified plane.")]
        [Input("arc", "The arc to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("arc", "The mirrored arc.")]
        public static Arc Mirror(this Arc arc, Plane p)
        {
            return new Arc { CoordinateSystem = arc.CoordinateSystem.Mirror(p), StartAngle = arc.StartAngle, EndAngle = arc.EndAngle, Radius = arc.Radius };
        }

        /***************************************************/

        [Description("Mirrors the circle about the specified plane.")]
        [Input("circle", "The circle to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("circle", "The mirrored circle.")]
        public static Circle Mirror(this Circle circle, Plane p)
        {
            return new Circle { Centre = circle.Centre.Mirror(p), Normal = circle.Normal.Mirror(p), Radius = circle.Radius };
        }

        /***************************************************/

        [Description("Mirrors the ellipse about the specified plane.")]
        [Input("ellipse", "The ellipse to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("ellipse", "The mirrored ellipse.")]
        public static Ellipse Mirror(this Ellipse ellipse, Plane p)
        {
            return new Ellipse
            {
                Axis1 = ellipse.Axis1.Mirror(p),
                Axis2 = ellipse.Axis2.Mirror(p),
                Centre = ellipse.Centre.Mirror(p),
                Radius1 = ellipse.Radius1,
                Radius2 = ellipse.Radius2,
            };
        }

        /***************************************************/

        [Description("Mirrors the line about the specified plane.")]
        [Input("line", "The line to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("line", "The mirrored line.")]
        public static Line Mirror(this Line line, Plane p)
        {
            return new Line { Start = line.Start.Mirror(p), End = line.End.Mirror(p) };
        }

        /***************************************************/

        [Description("Mirrors the NurbsCurve about the specified plane.")]
        [Input("curve", "The NurbsCurve to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("curve", "The mirrored NurbsCurve.")]
        public static NurbsCurve Mirror(this NurbsCurve curve, Plane p)
        {
            return new NurbsCurve()
            {
                ControlPoints = curve.ControlPoints.Select(x => x.Mirror(p)).ToList(),
                Weights = curve.Weights,
                Knots = curve.Knots
            };
        }


        /***************************************************/

        [Description("Mirrors the PolyCurve about the specified plane.")]
        [Input("curve", "The PolyCurve to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("curve", "The mirrored PolyCurve.")]
        public static PolyCurve Mirror(this PolyCurve curve, Plane p)
        {
            return new PolyCurve { Curves = curve.Curves.Select(x => x.IMirror(p)).ToList() };
        }

        /***************************************************/

        [Description("Mirrors the Polyline about the specified plane.")]
        [Input("curve", "The Polyline to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("curve", "The mirrored Polyline.")]
        public static Polyline Mirror(this Polyline curve, Plane p)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.Select(x => x.Mirror(p)).ToList() };
        }


        /***************************************************/
        /**** public Methods - Surfaces                 ****/
        /***************************************************/

        [Description("Mirrors the extrusion about the specified plane.")]
        [Input("surface", "The extrusion to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("surface", "The mirrored extrusion.")]
        public static Extrusion Mirror(this Extrusion surface, Plane p)
        {
            return new Extrusion { Curve = surface.Curve.IMirror(p), Direction = surface.Direction.Mirror(p), Capped = surface.Capped };
        }

        /***************************************************/

        [Description("Mirrors the loft about the specified plane.")]
        [Input("surface", "The loft to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("surface", "The mirrored loft.")]
        public static Loft Mirror(this Loft surface, Plane p)
        {
            return new Loft { Curves = surface.Curves.Select(x => x.IMirror(p)).ToList() };
        }

        /***************************************************/

        [Description("Mirrors the NurbsSurface about the specified plane.")]
        [Input("surface", "The NurbsSurface to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("surface", "The mirrored NurbsSurface.")]
        public static NurbsSurface Mirror(this NurbsSurface surface, Plane p)
        {
            List<SurfaceTrim> innerTrims = surface.InnerTrims.Select(x => new SurfaceTrim(IMirror(x.Curve3d, p), x.Curve2d)).ToList();

            List<SurfaceTrim> outerTrims = surface.OuterTrims.Select(x => new SurfaceTrim(IMirror(x.Curve3d, p), x.Curve2d)).ToList();

            return new NurbsSurface(
                surface.ControlPoints.Select(x => Mirror(x, p)),
                surface.Weights,
                surface.UKnots,
                surface.VKnots,
                surface.UDegree,
                surface.VDegree,
                innerTrims,
                outerTrims);
        }

        /***************************************************/

        [Description("Mirrors the pipe about the specified plane.")]
        [Input("surface", "The pipe to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("surface", "The mirrored pipe.")]
        public static Pipe Mirror(this Pipe surface, Plane p)
        {
            return new Pipe { Centreline = surface.Centreline.IMirror(p), Radius = surface.Radius, Capped = surface.Capped };
        }

        /***************************************************/

        [Description("Mirrors the PlanarSurface about the specified plane.")]
        [Input("surface", "The PlanarSurface to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("surface", "The mirrored PlanarSurface.")]
        public static PlanarSurface Mirror(this PlanarSurface surface, Plane p)
        {
            return new PlanarSurface(surface.ExternalBoundary.IMirror(p), surface.InternalBoundaries.Select(x => x.IMirror(p)).ToList());
        }

        /***************************************************/

        [Description("Mirrors the PolySurface about the specified plane.")]
        [Input("surface", "The PolySurface to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("surface", "The mirrored PolySurface.")]
        public static PolySurface Mirror(this PolySurface surface, Plane p)
        {
            return new PolySurface { Surfaces = surface.Surfaces.Select(x => x.IMirror(p)).ToList() };
        }


        /***************************************************/
        /**** public Methods - Others                   ****/
        /***************************************************/

        [Description("Mirrors the mesh about the specified plane.")]
        [Input("mesh", "The mesh to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("mesh", "The mirrored mesh.")]
        public static Mesh Mirror(this Mesh mesh, Plane p)
        {
            return new Mesh { Vertices = mesh.Vertices.Select(x => x.Mirror(p)).ToList(), Faces = mesh.Faces.ToList() };
        }

        /***************************************************/

        [Description("Mirrors the composite geometry about the specified plane.")]
        [Input("group", "The composite geometry to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("group", "The mirrored composite geometry.")]
        public static CompositeGeometry Mirror(this CompositeGeometry group, Plane p)
        {
            return new CompositeGeometry { Elements = group.Elements.Select(x => x.IMirror(p)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Mirrors the geometry about the specified plane. Used for interface-based dispatch.")]
        [Input("geometry", "The geometry to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("geometry", "The mirrored geometry.")]
        public static IGeometry IMirror(this IGeometry geometry, Plane p)
        {
            return Mirror(geometry as dynamic, p);
        }

        /***************************************************/

        [Description("Mirrors the curve about the specified plane. Used for interface-based dispatch.")]
        [Input("geometry", "The curve to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("curve", "The mirrored curve.")]
        public static ICurve IMirror(this ICurve geometry, Plane p)
        {
            return Mirror(geometry as dynamic, p);
        }

        /***************************************************/

        [Description("Mirrors the surface about the specified plane. Used for interface-based dispatch.")]
        [Input("geometry", "The surface to mirror.")]
        [Input("p", "The plane to mirror about.")]
        [Output("surface", "The mirrored surface.")]
        public static ISurface IMirror(this ISurface geometry, Plane p)
        {
            return Mirror(geometry as dynamic, p);
        }


        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        private static IGeometry Mirror(this IGeometry geometry, Plane p)
        {
            Base.Compute.RecordError("Mirror method has not been implemented for type " + geometry.GetType().Name);
            return null;
        }

        /***************************************************/
    }
}
