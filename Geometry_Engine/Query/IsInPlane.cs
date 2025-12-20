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
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        [Description("Checks if all Points in the collection lie within the specified Plane within the given tolerance.")]
        [Input("points", "The collection of Points to check.")]
        [Input("plane", "The Plane to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Output("result", "True if all Points are in the Plane, false otherwise.")]
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

        [Description("Checks if a Point lies within the specified Plane within the given tolerance.")]
        [Input("point", "The Point to check.")]
        [Input("plane", "The Plane to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Output("result", "True if the Point is in the Plane, false otherwise.")]
        public static bool IsInPlane(this Point point, Plane plane, double tolerance = Tolerance.Distance)
        {
            return (point.Distance(plane) <= tolerance);
        }

        /***************************************************/

        [Description("Checks if a Vector is parallel to the specified Plane (perpendicular to the plane's normal) within the given tolerance.")]
        [Input("vector", "The Vector to check.")]
        [Input("plane", "The Plane to check against.")]
        [Input("tolerance", "The tolerance for the perpendicularity check.")]
        [Output("result", "True if the Vector is parallel to the Plane, false otherwise.")]
        public static bool IsInPlane(this Vector vector, Plane plane, double tolerance = Tolerance.Distance)
        {
            return Math.Abs(vector.DotProduct(plane.Normal)) <= tolerance;
        }

        /***************************************************/

        [Description("Checks if one Plane lies within another Plane (i.e., they are coplanar) within the given tolerances.")]
        [Input("plane1", "The first Plane to check.")]
        [Input("plane2", "The second Plane to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Input("angTolerance", "The angle tolerance for checking normal alignment.", typeof(Angle))]
        [Output("result", "True if the Planes are coplanar, false otherwise.")]
        public static bool IsInPlane(this Plane plane1, Plane plane2, double tolerance = Tolerance.Distance, double angTolerance = Tolerance.Angle)
        {
            return plane1.Normal.IsParallel(plane2.Normal, angTolerance) != 0 && plane1.Origin.Distance(plane2) <= tolerance;
        }

        /***************************************************/

        [Description("Checks if a Cartesian coordinate system lies within the specified Plane within the given tolerances.")]
        [Input("coordinateSystem", "The Cartesian coordinate system to check.")]
        [Input("plane", "The Plane to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Input("angTolerance", "The angle tolerance for checking axis alignment.", typeof(Angle))]
        [Output("result", "True if the coordinate system is in the Plane, false otherwise.")]
        public static bool IsInPlane(this Cartesian coordinateSystem, Plane plane, double tolerance = Tolerance.Distance, double angTolerance = Tolerance.Angle)
        {
            return ((Plane)coordinateSystem).IsInPlane(plane, tolerance, angTolerance);
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Checks if an Arc lies within the specified Plane within the given tolerances.")]
        [Input("arc", "The Arc to check.")]
        [Input("plane", "The Plane to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Input("angTolerance", "The angle tolerance for checking normal alignment.", typeof(Angle))]
        [Output("result", "True if the Arc is in the Plane, false otherwise.")]
        public static bool IsInPlane(this Arc arc, Plane plane, double tolerance = Tolerance.Distance, double angTolerance = Tolerance.Angle)
        {
            return arc.CoordinateSystem.IsInPlane(plane, tolerance, angTolerance); //TODO: Is this check enough?
        }

        /***************************************************/

        [Description("Checks if a Circle lies within the specified Plane within the given tolerances.")]
        [Input("circle", "The Circle to check.")]
        [Input("plane", "The Plane to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Input("angTolerance", "The angle tolerance for checking normal alignment.", typeof(Angle))]
        [Output("result", "True if the Circle is in the Plane, false otherwise.")]
        public static bool IsInPlane(this Circle circle, Plane plane, double tolerance = Tolerance.Distance, double angTolerance = Tolerance.Angle)
        {
            //TODO: Is this check enough?
            return circle.Normal.IsParallel(plane.Normal, angTolerance) != 0 && Math.Abs(plane.Normal.DotProduct(circle.Centre - plane.Origin)) <= tolerance;
        }

        /***************************************************/

        [Description("Checks if an Ellipse lies within the specified Plane within the given tolerances.")]
        [Input("ellipse", "The Ellipse to check.")]
        [Input("plane", "The Plane to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Input("angTolerance", "The angle tolerance for checking normal alignment.", typeof(Angle))]
        [Output("result", "True if the Ellipse is in the Plane, false otherwise.")]
        public static bool IsInPlane(this Ellipse ellipse, Plane plane, double tolerance = Tolerance.Distance, double angTolerance = Tolerance.Angle)
        {
            //TODO: Is this check enough?
            return ellipse.Normal().IsParallel(plane.Normal, angTolerance) != 0 && Math.Abs(plane.Normal.DotProduct(ellipse.Centre - plane.Origin)) <= tolerance;
        }

        /***************************************************/

        [Description("Checks if a Line lies within the specified Plane within the given tolerance.")]
        [Input("line", "The Line to check.")]
        [Input("plane", "The Plane to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Output("result", "True if the Line is in the Plane, false otherwise.")]
        public static bool IsInPlane(this Line line, Plane plane, double tolerance = Tolerance.Distance)
        {
            return line.Start.IsInPlane(plane, tolerance) && line.End.IsInPlane(plane, tolerance);
        }

        /***************************************************/

        [Description("Checks if a NurbsCurve lies within the specified Plane within the given tolerance.")]
        [Input("curve", "The NurbsCurve to check.")]
        [Input("plane", "The Plane to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Output("result", "True if the NurbsCurve is in the Plane, false otherwise.")]
        public static bool IsInPlane(this NurbsCurve curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.IsInPlane(plane, tolerance);
        }

        /***************************************************/

        [Description("Checks if a PolyCurve lies within the specified Plane within the given tolerance.")]
        [Input("curve", "The PolyCurve to check.")]
        [Input("plane", "The Plane to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Output("result", "True if the PolyCurve is in the Plane, false otherwise.")]
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

        [Description("Checks if a Polyline lies within the specified Plane within the given tolerance.")]
        [Input("curve", "The Polyline to check.")]
        [Input("plane", "The Plane to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Output("result", "True if the Polyline is in the Plane, false otherwise.")]
        public static bool IsInPlane(this Polyline curve, Plane plane, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.IsInPlane(plane, tolerance);
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        [Description("Checks if an Extrusion surface lies within the specified Plane within the given tolerance.")]
        [Input("surface", "The Extrusion surface to check.")]
        [Input("plane", "The Plane to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Output("result", "True if the Extrusion is in the Plane, false otherwise.")]
        public static bool IsInPlane(this Extrusion surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            return surface.Direction.IsInPlane(plane, tolerance) && surface.Curve.IIsInPlane(plane, tolerance);
        }

        /***************************************************/

        [Description("Checks if a Loft surface lies within the specified Plane within the given tolerance.")]
        [Input("surface", "The Loft surface to check.")]
        [Input("plane", "The Plane to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Output("result", "True if the Loft is in the Plane, false otherwise.")]
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

        [Description("Checks if a PlanarSurface lies within the specified Plane within the given tolerance.")]
        [Input("surface", "The PlanarSurface to check.")]
        [Input("plane", "The Plane to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Output("result", "True if the PlanarSurface is in the Plane, false otherwise.")]
        public static bool IsInPlane(this PlanarSurface surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            return surface.ExternalBoundary.IIsInPlane(plane, tolerance);
        }

        /***************************************************/

        [Description("Checks if a Pipe surface lies within the specified Plane within the given tolerance.")]
        [Input("surface", "The Pipe surface to check.")]
        [Input("plane", "The Plane to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Output("result", "True if the Pipe is in the Plane, false otherwise.")]
        public static bool IsInPlane(this Pipe surface, Plane plane, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        [Description("Checks if a PolySurface lies within the specified Plane within the given tolerance.")]
        [Input("surface", "The PolySurface to check.")]
        [Input("plane", "The Plane to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Output("result", "True if the PolySurface is in the Plane, false otherwise.")]
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

        [Description("Checks if a Mesh lies within the specified Plane within the given tolerance.")]
        [Input("mesh", "The Mesh to check.")]
        [Input("plane", "The Plane to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Output("result", "True if the Mesh is in the Plane, false otherwise.")]
        public static bool IsInPlane(this Mesh mesh, Plane plane, double tolerance = Tolerance.Distance)
        {
            return mesh.Vertices.IsInPlane(plane, tolerance);
        }

        /***************************************************/

        [Description("Checks if a CompositeGeometry lies within the specified Plane within the given tolerance.")]
        [Input("group", "The CompositeGeometry to check.")]
        [Input("plane", "The Plane to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Output("result", "True if the CompositeGeometry is in the Plane, false otherwise.")]
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

        [Description("Checks if any IGeometry lies within the specified Plane within the given tolerance.")]
        [Input("geometry", "The IGeometry to check.")]
        [Input("plane", "The Plane to check against.")]
        [Input("tolerance", "The distance tolerance for the check.", typeof(Length))]
        [Output("result", "True if the geometry is in the Plane, false otherwise.")]
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
