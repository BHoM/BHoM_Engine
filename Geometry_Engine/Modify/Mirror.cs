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
using BH.oM.Geometry.CoordinateSystem;
using BH.oM.Reflection.Attributes;
using System;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** public Methods - Vectors                  ****/
        /***************************************************/

        public static Point Mirror(this Point pt, Plane p)
        {
            return pt - 2 * p.Normal.DotProduct(pt - p.Origin) * p.Normal;
        }

        /***************************************************/

        public static Vector Mirror(this Vector vector, Plane p)
        {
            return vector - 2 * vector.DotProduct(p.Normal) * p.Normal;
        }

        /***************************************************/

        public static Plane Mirror(this Plane plane, Plane p)
        {
            return new Plane { Origin = plane.Origin.Mirror(p), Normal = plane.Normal.Mirror(p) };
        }

        /***************************************************/

        public static Cartesian Mirror(this Cartesian coordinateSystem, Plane p)
        {
            return Create.CartesianCoordinateSystem(coordinateSystem.Origin.Mirror(p), coordinateSystem.X.Mirror(p), coordinateSystem.Y.Mirror(p));
        }


        /***************************************************/
        /**** public Methods - Curves                  ****/
        /***************************************************/

        public static Arc Mirror(this Arc arc, Plane p)
        {
            return new Arc { CoordinateSystem = arc.CoordinateSystem.Mirror(p), StartAngle = arc.StartAngle, EndAngle = arc.EndAngle, Radius = arc.Radius };
        }

        /***************************************************/

        public static Circle Mirror(this Circle circle, Plane p)
        {
            return new Circle { Centre = circle.Centre.Mirror(p), Normal = circle.Normal.Mirror(p), Radius = circle.Radius };
        }

        /***************************************************/

        public static Line Mirror(this Line line, Plane p)
        {
            return new Line { Start = line.Start.Mirror(p), End = line.End.Mirror(p) };
        }

        /***************************************************/

        [NotImplemented]
        public static NurbsCurve Mirror(this NurbsCurve curve, Plane p)
        {
            throw new NotImplementedException();
        }


        /***************************************************/

        public static PolyCurve Mirror(this PolyCurve curve, Plane p)
        {
            return new PolyCurve { Curves = curve.Curves.Select(x => x.IMirror(p)).ToList() };
        }

        /***************************************************/

        public static Polyline Mirror(this Polyline curve, Plane p)
        {
            return new Polyline { ControlPoints = curve.ControlPoints.Select(x => x.Mirror(p)).ToList() };
        }


        /***************************************************/
        /**** public Methods - Surfaces                 ****/
        /***************************************************/

        public static Extrusion Mirror(this Extrusion surface, Plane p)
        {
            return new Extrusion { Curve = surface.Curve.IMirror(p), Direction = surface.Direction.Mirror(p), Capped = surface.Capped };
        }

        /***************************************************/

        public static Loft Mirror(this Loft surface, Plane p)
        {
            return new Loft { Curves = surface.Curves.Select(x => x.IMirror(p)).ToList() };
        }

        /***************************************************/

        [NotImplemented]
        public static NurbsSurface Mirror(this NurbsSurface surface, Plane p)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static Pipe Mirror(this Pipe surface, Plane p)
        {
            return new Pipe { Centreline = surface.Centreline.IMirror(p), Radius = surface.Radius, Capped = surface.Capped };
        }

        /***************************************************/

        public static PolySurface Mirror(this PolySurface surface, Plane p)
        {
            return new PolySurface { Surfaces = surface.Surfaces.Select(x => x.IMirror(p)).ToList() };
        }


        /***************************************************/
        /**** public Methods - Others                   ****/
        /***************************************************/

        public static Mesh Mirror(this Mesh mesh, Plane p)
        {
            return new Mesh { Vertices = mesh.Vertices.Select(x => x.Mirror(p)).ToList(), Faces = mesh.Faces.Select(x => x.Clone()).ToList() };
        }

        /***************************************************/

        public static CompositeGeometry Mirror(this CompositeGeometry group, Plane p)
        {
            return new CompositeGeometry { Elements = group.Elements.Select(x => x.IMirror(p)).ToList() };
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static IGeometry IMirror(this IGeometry geometry, Plane p)
        {
            return Mirror(geometry as dynamic, p);
        }

        /***************************************************/

        public static ICurve IMirror(this ICurve geometry, Plane p)
        {
            return Mirror(geometry as dynamic, p);
        }

        /***************************************************/

        public static ISurface IMirror(this ISurface geometry, Plane p)
        {
            return Mirror(geometry as dynamic, p);
        }

        /***************************************************/
    }
}
