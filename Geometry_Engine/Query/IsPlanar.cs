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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        [Description("Checks if the Point is planar, i.e. all of its parts fit in a single plane. For a Point this is always the case, why this method always returns true.")]
        [Input("pt", "The Point to check for planarity.")]
        [Input("tolerance", "Distance tolerance for planarity validation.", typeof(Length))]
        [Output("isPlanar", "A Point is always planar, why this method always returns true.")]
        public static bool IsPlanar(this Point pt, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        [Description("Checks if the Vector is planar, i.e. all of its parts fit in a single plane. For a Vector this is always the case, why this method always returns true.")]
        [Input("vector", "The Vector to check for planarity.")]
        [Input("tolerance", "Distance tolerance for planarity validation.", typeof(Length))]
        [Output("isPlanar", "A Vector is always planar, why this method always returns true.")]
        public static bool IsPlanar(this Vector vector, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        [Description("Checks if the Plane is planar, i.e. all of its parts fit in a single plane. For a Plane this is always the case, why this method always returns true.")]
        [Input("plane", "The Plane to check for planarity.")]
        [Input("tolerance", "Distance tolerance for planarity validation.", typeof(Length))]
        [Output("isPlanar", "A Plane is always planar, why this method always returns true.")]
        public static bool IsPlanar(this Plane plane, double tolerance = Tolerance.Distance)
        {
            return true;
        }


        /***************************************************/
        /**** public Methods - Curves                   ****/
        /***************************************************/

        [Description("Checks if the Line is planar, i.e. all of its parts fit in a single plane. For a Line this is always the case, why this method always returns true.")]
        [Input("line", "The Plane to check for planarity.")]
        [Input("tolerance", "Distance tolerance for planarity validation.", typeof(Length))]
        [Output("isPlanar", "A Line is always planar, why this method always returns true.")]
        public static bool IsPlanar(this Line line, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        [Description("Checks if the Arc is planar, i.e. all of its parts fit in a single plane. For a Arc this is always the case, why this method always returns true.")]
        [Input("arc", "The Arc to check for planarity.")]
        [Input("tolerance", "Distance tolerance for planarity validation.", typeof(Length))]
        [Output("isPlanar", "A Arc is always planar, why this method always returns true.")]
        public static bool IsPlanar(this Arc arc, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        [Description("Checks if the Circle is planar, i.e. all of its parts fit in a single plane. For a Circle this is always the case, why this method always returns true.")]
        [Input("circle", "The Circle to check for planarity.")]
        [Input("tolerance", "Distance tolerance for planarity validation.", typeof(Length))]
        [Output("isPlanar", "A Circle is always planar, why this method always returns true.")]
        public static bool IsPlanar(this Circle circle, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        [Description("Checks if the Ellipse is planar, i.e. all of its parts fit in a single plane. For a Ellipse this is always the case, why this method always returns true.")]
        [Input("ellipse", "The Ellipse to check for planarity.")]
        [Input("tolerance", "Distance tolerance for planarity validation.", typeof(Length))]
        [Output("isPlanar", "A Ellipse is always planar, why this method always returns true.")]
        public static bool IsPlanar(this Ellipse ellipse, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        [Description("Checks if the NurbsCurve is planar, i.e. all of its parts fit in a single plane. Check is done by evaluating if all the controlpoints are co-planar within the provided tolerance.")]
        [Input("curve", "The NurbsCurve to check for planarity.")]
        [Input("tolerance", "Distance tolerance for planarity validation.", typeof(Length))]
        [Output("isPlanar", "Returns true if the provided NurbsCurve is planar, i.e. all of its controlpoints are in the same plane within tolerance.")]
        public static bool IsPlanar(this NurbsCurve curve, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.IsCoplanar(tolerance);
        }

        /***************************************************/

        [Description("Checks if the Polyline is planar, i.e. all of its parts fit in a single plane. Check is done by evaluating if all the controlpoints are co-planar within the provided tolerance.")]
        [Input("curve", "The Polyline to check for planarity.")]
        [Input("tolerance", "Distance tolerance for planarity validation.", typeof(Length))]
        [Output("isPlanar", "Returns true if the provided Polyline is planar, i.e. all of its controlpoints are in the same plane within tolerance.")]
        public static bool IsPlanar(this Polyline curve, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints.IsCoplanar(tolerance);
        }

        /***************************************************/

        [Description("Checks if the PolyCurve is planar, i.e. all of its parts fit in a single plane. Check is done by evaluating if all the controlpoints are co-planar within the provided tolerance.")]
        [Input("curve", "The PolyCurve to check for planarity.")]
        [Input("tolerance", "Distance tolerance for planarity validation.", typeof(Length))]
        [Output("isPlanar", "Returns true if the provided PolyCurve is planar, i.e. all of its controlpoints are in the same plane within tolerance.")]
        public static bool IsPlanar(this PolyCurve curve, double tolerance = Tolerance.Distance)
        {
            return curve.ControlPoints().IsCoplanar(tolerance);
        }

        /***************************************************/

        [Description("Checks if the Polygon is planar, i.e. all of its parts fit in a single plane. Polygons are ensured to be planar at creation, why this method always returns true.")]
        [Input("polygon", "The Polygon to check for planarity.")]
        [Input("tolerance", "Distance tolerance for planarity validation.", typeof(Length))]
        [Output("isPlanar", "A Polygon is ensured to be planar at creation, why this method always returns true.")]
        public static bool IsPlanar(this Polygon polygon, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        [Description("Checks if the BoundaryCurve is planar, i.e. all of its parts fit in a single plane. BoundaryCurve are ensured to be planar at creation, why this method always returns true.")]
        [Input("curve", "The BoundaryCurve to check for planarity.")]
        [Input("tolerance", "Distance tolerance for planarity validation.", typeof(Length))]
        [Output("isPlanar", "A BoundaryCurve is ensured to be planar at creation, why this method always returns true.")]
        public static bool IsPlanar(this BoundaryCurve curve, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        [Description("Checks if the PlanarSurface is planar, i.e. all of its parts fit in a single plane. A PlanarSurface is ensured to be planar at creation, why this method always returns true if the default tolerance is provided. For other tolerances the method returns true if the controlpoints of external and internal boundaries are co-planar within tolerance.")]
        [Input("surface", "The PlanarSurface to check for planarity.")]
        [Input("tolerance", "Distance tolerance for planarity validation.", typeof(Length))]
        [Output("isPlanar", "Returns true if the provided PlanarSurface is planar, i.e. if the control points of the external and internal boundaries are in the same plane within tolerance.")]
        public static bool IsPlanar(this PlanarSurface surface, double tolerance = Tolerance.Distance)
        {
            // This is not a bug: PlanarSurface is IImmutable with all necessary planarity checks being made by the Create method.
            // It is recommended to use Create method instead of the constructor to ensure planarity within the default tolerance.
            if (tolerance == Tolerance.Distance)
                return true;
            else
            {
                List<Point> controlPoints = surface.ExternalBoundary.IControlPoints();
                foreach (ICurve outline in surface.InternalBoundaries)
                {
                    controlPoints.AddRange(outline.IControlPoints());
                }

                return controlPoints.IsCoplanar(tolerance);
            }
        }

        /***************************************************/

        [Description("Checks if the Extrusion is planar, i.e. all of its parts fit in a single plane. This is true if the direction vector has a length lower than the tolerance, or if the base curve is linear.")]
        [Input("surface", "The Extrusion to check for planarity.")]
        [Input("tolerance", "Distance tolerance for planarity validation.", typeof(Length))]
        [Output("isPlanar", "Returns true if the provided Extrusion is planar.")]
        public static bool IsPlanar(this Extrusion surface, double tolerance = Tolerance.Distance)
        {
            return (surface.Direction.Length() <= tolerance || surface.Curve.IIsLinear());
        }

        /***************************************************/

        [Description("Checks if the Loft is planar, i.e. all of its parts fit in a single plane, by checking if the controlpoints of all the curves are co-planar.")]
        [Input("surface", "The Loft to check for planarity.")]
        [Input("tolerance", "Distance tolerance for planarity validation.", typeof(Length))]
        [Output("isPlanar", "Returns true if the provided Loft is planar, i.e. if the control points of all of its curves are all in the same plane within tolerance.")]
        public static bool IsPlanar(this Loft surface, double tolerance = Tolerance.Distance)
        {
            List<Point> controlPts = new List<Point>();
            foreach (ICurve curve in surface.Curves)
            {
                controlPts.AddRange(curve.IControlPoints());
            }
            return controlPts.IsCoplanar(tolerance);
        }

        /***************************************************/

        [Description("Checks if the Pipe is planar, i.e. all of its parts fit in a single plane. This is true if the Centreline has a length lower than the tolerance, or if the radiues is smaller than the tolerance.")]
        [Input("surface", "The Pipe to check for planarity.")]
        [Input("tolerance", "Distance tolerance for planarity validation.", typeof(Length))]
        [Output("isPlanar", "Returns true if the provided Pipe is planar.")]
        public static bool IsPlanar(this Pipe surface, double tolerance = Tolerance.Distance)
        {
            return surface.Centreline.ILength() <= tolerance || surface.Radius == 0;
        }

        /***************************************************/

        public static bool IsPlanar(this PolySurface surface, double tolerance = Tolerance.Distance)
        {
            foreach (ISurface s in surface.Surfaces)
            {
                if (!s.IIsPlanar(tolerance))
                    return false;
            }
            return true;
        }


        /***************************************************/
        /**** Public Methods - Others                  ****/
        /***************************************************/

        [Description("Checks if the Mesh is planar, i.e. all of its parts fit in a single plane. Check is done by evaluating if all the vertices are co-planar within the provided tolerance.")]
        [Input("mesh", "The Mesh to check for planarity.")]
        [Input("tolerance", "Distance tolerance for planarity validation.", typeof(Length))]
        [Output("isPlanar", "Returns true if the provided Mesh is planar, i.e. all of its vertices are in the same plane within tolerance.")]
        public static bool IsPlanar(this Mesh mesh, double tolerance = Tolerance.Distance)
        {
            return mesh.Vertices.IsCoplanar(tolerance);
        }

        /***************************************************/

        public static bool IsPlanar(this CompositeGeometry group, double tolerance = Tolerance.Distance)
        {
            foreach (IGeometry element in group.Elements)
            {
                if (!element.IIsPlanar(tolerance))
                    return false;
            }
            return true;
        }


        /***************************************************/
        /**** Public Methods = Interfaces               ****/
        /***************************************************/

        [Description("Checks if the IGeometry is planar, i.e. all of its parts fit in a single plane.")]
        [Input("geometry", "The IGeometry to check for planarity.")]
        [Input("tolerance", "Distance tolerance for planarity validation.", typeof(Length))]
        [Output("isPlanar", "Returns true if the provided IGeometry is planar.")]
        public static bool IIsPlanar(this IGeometry geometry, double tolerance = Tolerance.Distance)
        {
            return IsPlanar(geometry as dynamic, tolerance);
        }


        /***************************************************/
        /**** Private Fallback Methods                  ****/
        /***************************************************/

        private static bool IsPlanar(this IGeometry geometry, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException($"IsPlanar is not implemented for IGeometry of type: {geometry.GetType().Name}.");
        }

        /***************************************************/
    }
}


