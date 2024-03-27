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
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static bool IsInPlane(this IEnumerable<Point> points, Plane plane, double tolerance = Tolerance.Distance)
        {
            foreach (Point pt in points)
            {
                if (pt.Distance(plane) > tolerance)
                    return false;
            }
            return true;
        }

        /***************************************************/

        public static bool IsInPlane(this Point point, Plane plane, double tolerance = Tolerance.Distance)
        {
            return (point.Distance(plane) <= tolerance);
        }

        /***************************************************/

        public static bool IsInPlane(this Vector vector, Plane plane, double tolerance = Tolerance.Distance)
        {
            return Math.Abs(vector.DotProduct(plane.Normal)) <= tolerance;
        }

        /***************************************************/

        public static bool IsInPlane(this Plane plane1, Plane plane2, double tolerance = Tolerance.Distance, double angTolerance = Tolerance.Angle)
        {
            return plane1.Normal.IsParallel(plane2.Normal, angTolerance) != 0 && plane1.Origin.Distance(plane2) <= tolerance;
        }

        /***************************************************/

        public static bool IsInPlane(this Cartesian coordinateSystem, Plane plane, double tolerance = Tolerance.Distance, double angTolerance = Tolerance.Angle)
        {
            return ((Plane)coordinateSystem).IsInPlane(plane, tolerance, angTolerance);
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static bool IsInPlane(this Arc arc, Plane plane, double tolerance = Tolerance.Distance, double angTolerance = Tolerance.Angle)
        {
            return arc.CoordinateSystem.IsInPlane(plane, tolerance, angTolerance); //TODO: Is this check enough?
        }

        /***************************************************/

        public static bool IsInPlane(this Circle circle, Plane plane, double tolerance = Tolerance.Distance, double angTolerance = Tolerance.Angle)
        {
            //TODO: Is this check enough?
            return circle.Normal.IsParallel(plane.Normal, angTolerance) != 0 && Math.Abs(plane.Normal.DotProduct(circle.Centre - plane.Origin)) <= tolerance;
        }

        /***************************************************/

        public static bool IsInPlane(this Ellipse ellipse, Plane plane, double tolerance = Tolerance.Distance, double angTolerance = Tolerance.Angle)
        {
            //TODO: Is this check enough?
            return ellipse.Normal().IsParallel(plane.Normal, angTolerance) != 0 && Math.Abs(plane.Normal.DotProduct(ellipse.Centre - plane.Origin)) <= tolerance;
        }

        /***************************************************/

        public static bool IsInPlane(this Line line, Plane plane, double tolerance = Tolerance.Distance)
        {
            return line.Start.IsInPlane(plane, tolerance) && line.End.IsInPlane(plane, tolerance);
        }

        /***************************************************/

        public static bool IsInPlane(this NurbsCurve curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.IsInPlane(plane, tolerance);
        }

        /***************************************************/

        public static bool IsInPlane(this PolyCurve curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            foreach (ICurve c in curve.Curves)
            {
                if (!c.IIsInPlane(plane, tolerance))
                    return false;
            }

            return true;
        }

        /***************************************************/

        public static bool IsInPlane(this Polyline curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.IsInPlane(plane, tolerance);
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static bool IsInPlane(this Extrusion surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            return surface.Direction.IsInPlane(plane, tolerance) && surface.Curve.IIsInPlane(plane, tolerance);
        }

        /***************************************************/

        public static bool IsInPlane(this Loft surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            foreach (ICurve c in surface.Curves)
            {
                if (!c.IIsInPlane(plane, tolerance))
                    return false;
            }

            return true;
        }

        /***************************************************/

        public static bool IsInPlane(this PlanarSurface surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            return surface.ExternalBoundary.IIsInPlane(plane, tolerance);
        }

        /***************************************************/

        public static bool IsInPlane(this Pipe surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        public static bool IsInPlane(this PolySurface surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            foreach (ISurface s in surface.Surfaces)
            {
                if (!s.IIsInPlane(plane, tolerance))
                    return false;
            }

            return true;
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static bool IsInPlane(this Mesh mesh, Plane plane, double tolerance = Tolerance.Distance)
        {
            return mesh.Vertices.IsInPlane(plane, tolerance);
        }

        /***************************************************/

        public static bool IsInPlane(this CompositeGeometry group, Plane plane, double tolerance = Tolerance.Distance)
        {
            foreach (IGeometry g in group.Elements)
            {
                if (!g.IIsInPlane(plane, tolerance))
                    return false;
            }

            return true;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static bool IIsInPlane(this IGeometry geometry, Plane plane, double tolerance = Tolerance.Distance)
        {
            return IsInPlane(geometry as dynamic, plane, tolerance);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static bool IsInPlane(this IGeometry geometry, Plane plane, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException($"IsInPlane is not implemented for IGeometry of type: {geometry.GetType().Name}.");
        }

        /***************************************************/
    }
}



