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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        [Description("Projects the point onto the specified plane.")]
        [Input("pt", "The point to project.")]
        [Input("p", "The plane to project onto.")]
        [Output("pt", "The projected point.")]
        public static Point Project(this Point pt, Plane p)
        {
            Vector normal = p.Normal.Normalise();
            return pt - normal.DotProduct(pt - p.Origin) * normal;
        }

        /***************************************************/

        [Description("Projects the point onto the closest point on the specified line.")]
        [Input("pt", "The point to project.")]
        [Input("line", "The line to project onto.")]
        [Output("pt", "The projected point.")]
        public static Point Project(this Point pt, Line line)
        {
            return line.ClosestPoint(pt, true);
        }

        /***************************************************/

        [Description("Projects the vector onto the specified plane.")]
        [Input("vector", "The vector to project.")]
        [Input("p", "The plane to project onto.")]
        [Output("vector", "The projected vector.")]
        public static Vector Project(this Vector vector, Plane p)
        {
            return vector - vector.DotProduct(p.Normal) * p.Normal;
        }

        /***************************************************/

        [Description("Projects the vector onto another vector.")]
        [Input("vector", "The vector to project.")]
        [Input("other", "The vector to project onto.")]
        [Output("vector", "The projected vector.")]
        public static Vector Project(this Vector vector, Vector other)
        {
            other = other.Normalise();
            double dot = vector.DotProduct(other);
            Vector projected = other * dot;
            return projected;
        }

        /***************************************************/

        [Description("Projects the plane onto another plane.")]
        [Input("plane", "The plane to project.")]
        [Input("p", "The plane to project onto.")]
        [Output("plane", "The projected plane.")]
        public static Plane Project(this Plane plane, Plane p)
        {
            double dp = plane.Normal.DotProduct(p.Normal);
            if (Math.Abs(dp) <= Tolerance.Angle)
                return null;

            Vector normal = dp > 0 ? p.Normal : p.Normal.Reverse();
            return new Plane { Origin = plane.Origin.Project(p), Normal = normal };
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Projects the arc onto the specified plane. If the arc's plane is parallel to the target plane, a new arc is created on the projected plane; otherwise, the arc is converted to a NurbsCurve and projected.")]
        [Input("arc", "The arc to project.")]
        [Input("p", "The plane to project onto.")]
        [Output("curve", "The projected arc or curve.")]
        public static ICurve Project(this Arc arc, Plane p)
        {
            if (arc.CoordinateSystem.Z.IsParallel(p.Normal) != 0)
            {
                return new Arc
                {
                    CoordinateSystem = new oM.Geometry.CoordinateSystem.Cartesian(arc.CoordinateSystem.Origin.Project(p), arc.CoordinateSystem.X, arc.CoordinateSystem.Y, arc.CoordinateSystem.Z),
                    Radius = arc.Radius,
                    StartAngle = arc.StartAngle,
                    EndAngle = arc.EndAngle
                };
            }
            else
                return arc.ToNurbsCurve().Project(p);
        }

        /***************************************************/

        [Description("Projects the circle onto the specified plane. If the circle's normal is parallel to the plane's normal, a new circle is created; otherwise, an ellipse is returned.")]
        [Input("circle", "The circle to project.")]
        [Input("p", "The plane to project onto.")]
        [Output("curve", "The projected circle or ellipse.")]
        public static ICurve Project(this Circle circle, Plane p)
        {
            if (circle.Normal.IsParallel(p.Normal) != 0)
                return new Circle { Centre = circle.Centre.Project(p), Normal = circle.Normal, Radius = circle.Radius };

            Vector axis1 = p.Normal.CrossProduct(circle.Normal);
            Vector axis2 = axis1.CrossProduct(p.Normal);
            double radius2 = circle.Radius * circle.Normal.DotProduct(p.Normal);

            return new Ellipse { Centre = circle.Centre.Project(p), Axis1 = axis1, Axis2 = axis2, Radius1 = circle.Radius, Radius2 = radius2 };
        }

        /***************************************************/

        [Description("Projects the ellipse onto the specified plane using a projection matrix.")]
        [Input("ellipse", "The ellipse to project.")]
        [Input("p", "The plane to project onto.")]
        [Output("curve", "The projected ellipse.")]
        public static ICurve Project(this Ellipse ellipse, Plane p)
        {
            TransformMatrix project = Create.ProjectionMatrix(p, p.Normal);
            return ellipse.Transform(project);
        }

        /***************************************************/

        [Description("Projects the line onto the specified plane.")]
        [Input("line", "The line to project.")]
        [Input("p", "The plane to project onto.")]
        [Output("line", "The projected line.")]
        public static Line Project(this Line line, Plane p)
        {
            return new Line { Start = line.Start.Project(p), End = line.End.Project(p) };
        }

        /***************************************************/

        [Description("Projects the NurbsCurve onto the specified plane.")]
        [Input("curve", "The NurbsCurve to project.")]
        [Input("p", "The plane to project onto.")]
        [Output("curve", "The projected NurbsCurve.")]
        public static NurbsCurve Project(this NurbsCurve curve, Plane p)
        {
            return new NurbsCurve()
            {
                ControlPoints = curve.ControlPoints.Select(x => x.Project(p)).ToList(),
                Knots = curve.Knots,
                Weights = curve.Weights,
            };
        }

        /***************************************************/

        [Description("Projects the PolyCurve onto the specified plane.")]
        [Input("curve", "The PolyCurve to project.")]
        [Input("p", "The plane to project onto.")]
        [Output("curve", "The projected PolyCurve.")]
        public static PolyCurve Project(this PolyCurve curve, Plane p)
        {
            return new PolyCurve { Curves = curve.Curves.Select(x => x.IProject(p)).ToList() };
        }

        /***************************************************/

        [Description("Projects the Polyline onto the specified plane.")]
        [Input("curve", "The Polyline to project.")]
        [Input("p", "The plane to project onto.")]
        [Output("curve", "The projected Polyline.")]
        public static Polyline Project(this Polyline curve, Plane p)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.Select(x => x.Project(p)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        [Description("Projects the extrusion onto the specified plane.")]
        [Input("surface", "The extrusion to project.")]
        [Input("p", "The plane to project onto.")]
        [Output("surface", "The projected extrusion.")]
        public static Extrusion Project(this Extrusion surface, Plane p)
        {
            return new Extrusion { Curve = surface.Curve.IProject(p), Direction = surface.Direction.Project(p), Capped = surface.Capped };
        }

        /***************************************************/

        [Description("Projects the loft onto the specified plane.")]
        [Input("surface", "The loft to project.")]
        [Input("p", "The plane to project onto.")]
        [Output("surface", "The projected loft.")]
        public static Loft Project(this Loft surface, Plane p)
        {
            return new Loft { Curves = surface.Curves.Select(x => x.IProject(p)).ToList() };
        }

        /***************************************************/

        [Description("Projects the NurbsSurface onto the specified plane.")]
        [Input("surface", "The NurbsSurface to project.")]
        [Input("p", "The plane to project onto.")]
        [Output("surface", "The projected NurbsSurface.")]
        public static NurbsSurface Project(this NurbsSurface surface, Plane p)
        {
            List<SurfaceTrim> innerTrims = surface.InnerTrims.Select(x => new SurfaceTrim(IProject(x.Curve3d, p), x.Curve2d)).ToList();

            List<SurfaceTrim> outerTrims = surface.OuterTrims.Select(x => new SurfaceTrim(IProject(x.Curve3d, p), x.Curve2d)).ToList();

            return new NurbsSurface(
                surface.ControlPoints.Select(x => Project(x, p)),
                surface.Weights,
                surface.UKnots,
                surface.VKnots,
                surface.UDegree,
                surface.VDegree,
                innerTrims,
                outerTrims);
        }

        /***************************************************/

        [Description("Projects the PlanarSurface onto the specified plane.")]
        [Input("surface", "The PlanarSurface to project.")]
        [Input("p", "The plane to project onto.")]
        [Output("surface", "The projected PlanarSurface.")]
        public static PlanarSurface Project(this PlanarSurface surface, Plane p)
        {
            return new PlanarSurface(surface.ExternalBoundary.IProject(p), surface.InternalBoundaries.Select(x => x.IProject(p)).ToList());
        }

        /***************************************************/

        [Description("Projects the PolySurface onto the specified plane.")]
        [Input("surface", "The PolySurface to project.")]
        [Input("p", "The plane to project onto.")]
        [Output("surface", "The projected PolySurface.")]
        public static PolySurface Project(this PolySurface surface, Plane p)
        {
            return new PolySurface { Surfaces = surface.Surfaces.Select(x => x.IProject(p)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        [Description("Projects the mesh onto the specified plane.")]
        [Input("mesh", "The mesh to project.")]
        [Input("p", "The plane to project onto.")]
        [Output("mesh", "The projected mesh.")]
        public static Mesh Project(this Mesh mesh, Plane p)
        {
            return new Mesh { Vertices = mesh.Vertices.Select(x => x.Project(p)).ToList(), Faces = mesh.Faces.ToList() };
        }

        /***************************************************/

        [Description("Projects the composite geometry onto the specified plane.")]
        [Input("group", "The composite geometry to project.")]
        [Input("p", "The plane to project onto.")]
        [Output("group", "The projected composite geometry.")]
        public static CompositeGeometry Project(this CompositeGeometry group, Plane p)
        {
            return new CompositeGeometry { Elements = group.Elements.Select(x => x.IProject(p)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Projects the geometry onto the specified plane. Used for interface-based dispatch.")]
        [Input("geometry", "The geometry to project.")]
        [Input("p", "The plane to project onto.")]
        [Output("geometry", "The projected geometry.")]
        public static IGeometry IProject(this IGeometry geometry, Plane p)
        {
            return Project(geometry as dynamic, p);
        }

        /***************************************************/

        [Description("Projects the curve onto the specified plane. Used for interface-based dispatch.")]
        [Input("geometry", "The curve to project.")]
        [Input("p", "The plane to project onto.")]
        [Output("curve", "The projected curve.")]
        public static ICurve IProject(this ICurve geometry, Plane p)
        {
            return Project(geometry as dynamic, p);
        }

        /***************************************************/

        [Description("Projects the surface onto the specified plane. Used for interface-based dispatch.")]
        [Input("geometry", "The surface to project.")]
        [Input("p", "The plane to project onto.")]
        [Output("surface", "The projected surface.")]
        public static ISurface IProject(this ISurface geometry, Plane p)
        {
            return Project(geometry as dynamic, p);
        }


        /***************************************************/
        /**** Private Methods - Fallback                ****/
        /***************************************************/

        private static IGeometry Project(this IGeometry geometry, Plane p)
        {
            Base.Compute.RecordError("Project method has not been implemented for type " + geometry.GetType().Name);
            return null;
        }

        /***************************************************/
    }
}
