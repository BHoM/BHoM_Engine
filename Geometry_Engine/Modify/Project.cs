/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using System;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static Point Project(this Point pt, Plane p)
        {
            return pt - p.Normal.DotProduct(pt - p.Origin) * p.Normal;
        }

        /***************************************************/

        public static Point Project(this Point pt, Line line)
        {
            return line.ClosestPoint(pt, true);
        }

        /***************************************************/

        public static Vector Project(this Vector vector, Plane p)
        {
            return vector - vector.DotProduct(p.Normal) * p.Normal;
        }

        /***************************************************/

        public static Vector Project(this Vector vector,Vector other)
        {
            other = other.Normalise();
            double dot = vector.DotProduct(other);
            Vector projected = other * dot;
            return projected;
        }

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

        public static ICurve Project(this Circle circle, Plane p)
        {
            if (circle.Normal.IsParallel(p.Normal) != 0)
                return new Circle { Centre = circle.Centre.Project(p), Normal = circle.Normal.Clone(), Radius = circle.Radius };

            Vector axis1 = p.Normal.CrossProduct(circle.Normal);
            Vector axis2 = axis1.CrossProduct(p.Normal);
            double radius2 = circle.Radius * circle.Normal.DotProduct(p.Normal);

            return new Ellipse { Centre = circle.Centre.Project(p), Axis1 = axis1, Axis2 = axis2, Radius1 = circle.Radius, Radius2 = radius2 };
        }

        /***************************************************/

        public static Line Project(this Line line, Plane p)
        {
            return new Line { Start = line.Start.Project(p), End = line.End.Project(p) };
        }

        /***************************************************/

        [NotImplemented]
        public static NurbsCurve Project(this NurbsCurve curve, Plane p)
        {
            throw new NotImplementedException();
        }


        /***************************************************/

        public static PolyCurve Project(this PolyCurve curve, Plane p)
        {
            return new PolyCurve { Curves = curve.Curves.Select(x => x.IProject(p)).ToList() };
        }

        /***************************************************/

        public static Polyline Project(this Polyline curve, Plane p)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.Select(x => x.Project(p)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static Extrusion Project(this Extrusion surface, Plane p)
        {
            return new Extrusion { Curve = surface.Curve.IProject(p), Direction = surface.Direction.Project(p), Capped = surface.Capped };
        }

        /***************************************************/

        public static Loft Project(this Loft surface, Plane p)
        {
            return new Loft { Curves = surface.Curves.Select(x => x.IProject(p)).ToList() };
        }

        /***************************************************/

        [NotImplemented]
        public static NurbsSurface Project(this NurbsSurface surface, Plane p)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static Pipe Project(this Pipe surface, Plane p)
        {
            throw new NotImplementedException(); //TODO: implement projection of a pipe on a plane
        }

        /***************************************************/

        public static PolySurface Project(this PolySurface surface, Plane p)
        {
            return new PolySurface { Surfaces = surface.Surfaces.Select(x => x.IProject(p)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static Mesh Project(this Mesh mesh, Plane p)
        {
            return new Mesh { Vertices = mesh.Vertices.Select(x => x.Project(p)).ToList(), Faces = mesh.Faces.Select(x => x.Clone() as Face).ToList() };
        }

        /***************************************************/

        public static CompositeGeometry Project(this CompositeGeometry group, Plane p)
        {
            return new CompositeGeometry { Elements = group.Elements.Select(x => x.IProject(p)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IGeometry IProject(this IGeometry geometry, Plane p)
        {
            return Project(geometry as dynamic, p);
        }

        /***************************************************/

        public static ICurve IProject(this ICurve geometry, Plane p)
        {
            return Project(geometry as dynamic, p);
        }

        /***************************************************/

        public static ISurface IProject(this ISurface geometry, Plane p)
        {
            return Project(geometry as dynamic, p);
        }

        /***************************************************/
    }
}
