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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.Linq;
using System;
using System.ComponentModel;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        [Description("Checks if the Arc is closed, i.e. forms a closed loop by checking if the end points are within tolerance of each other.")]
        [Input("arc", "The Arc to check for closedness.")]
        [Input("tolerance", "Distance tolerance for closedness validation.", typeof(Length))]
        [Output("isClosed", "Returns true if the Arc is closed, i.e. a circle.")]
        public static bool IsClosed(this Arc arc, double tolerance = Tolerance.Distance)
        {
            return (arc.Angle() - Math.PI * 2) * arc.Radius > -tolerance;
        }

        /***************************************************/

        [Description("Checks if the Circle is closed, i.e. forms a closed loop. A Circle is by definition always closed, why this method always returns true.")]
        [Input("circle", "The Circle to check for closedness.")]
        [Input("tolerance", "Distance tolerance for closedness validation.", typeof(Length))]
        [Output("isClosed", "Returns true for all cases.")]
        public static bool IsClosed(this Circle circle, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        [Description("Checks if the Line is closed, i.e. forms a closed loop. A Line is by definition never closed, why this method always returns false.")]
        [Input("line", "The Circle to check for closedness.")]
        [Input("tolerance", "Distance tolerance for closedness validation.", typeof(Length))]
        [Output("isClosed", "Returns false for all cases.")]
        public static bool IsClosed(this Line line, double tolerance = Tolerance.Distance)
        {
            return false;
        }

        /***************************************************/

        [Description("Checks if the Ellipse is closed, i.e. forms a closed loop. An Ellipse is by definition always closed, why this method always returns true.")]
        [Input("ellipse", "The Ellipse to check for closedness.")]
        [Input("tolerance", "Distance tolerance for closedness validation.", typeof(Length))]
        [Output("isClosed", "Returns true for all cases.")]
        public static bool IsClosed(this Ellipse ellipse, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        [Description("Checks if the NurbsCurve is closed, i.e. forms a closed loop, by checking if the end points are within tolerance of each other or if it is a periodic curve.")]
        [Input("curve", "The NurbsCurve to check for closedness.")]
        [Input("tolerance", "Distance tolerance for closedness validation.", typeof(Length))]
        [Output("isClosed", "Returns true if the NurbsCurve is closed.")]
        public static bool IsClosed(this NurbsCurve curve, double tolerance = Tolerance.Distance)
        {
            if (curve == null || curve.ControlPoints == null || curve.ControlPoints.Count < 2)
                return false;

            return curve.ControlPoints.First().SquareDistance(curve.ControlPoints.Last()) <= tolerance * tolerance || curve.IsPeriodic();
        }

        /***************************************************/

        [Description("Checks if the PolyCurve is closed, i.e. forms a closed loop, by checking if the end points are within tolerance of each other or if it is made up of a single inner Curve that is itself closed.")]
        [Input("curve", "The PolyCurve to check for closedness.")]
        [Input("tolerance", "Distance tolerance for closedness validation.", typeof(Length))]
        [Output("isClosed", "Returns true if the PolyCurve is closed.")]
        public static bool IsClosed(this PolyCurve curve, double tolerance = Tolerance.Distance)
        {
            if (curve.Curves.Count == 1)
                return IIsClosed(curve.Curves[0], tolerance);

            List<ICurve> curves = curve.Curves;
            double sqTol = tolerance * tolerance;
            if (curves[0].IStartPoint().SquareDistance(curves.Last().IEndPoint()) > sqTol)
                return false;

            for (int i = 1; i < curves.Count; i++)
            {
                if (curves[i - 1].IEndPoint().SquareDistance(curves[i].IStartPoint()) > sqTol)
                    return false;
            }

            return true;
        }

        /***************************************************/

        [Description("Checks if the PolyCurve is closed, i.e. forms a closed loop, by checking if the first and last control points are within tolerance of each other.")]
        [Input("curve", "The Polyline to check for closedness.")]
        [Input("tolerance", "Distance tolerance for closedness validation.", typeof(Length))]
        [Output("isClosed", "Returns true if the Polyline is closed.")]
        public static bool IsClosed(this Polyline curve, double tolerance = Tolerance.Distance)
        {
            List<Point> pts = curve.ControlPoints;
            if (pts.Count == 0)
                return false;

            return pts.First().SquareDistance(pts.Last()) < tolerance * tolerance;
        }

        /***************************************************/

        [Description("Checks if the Polygon is closed, i.e. forms a closed loop. A Polygon is always ensured to be closed at creation, why this method always returns true.")]
        [Input("curve", "The Polygon to check for closedness.")]
        [Input("tolerance", "Distance tolerance for closedness validation.", typeof(Length))]
        [Output("isClosed", "A Polygon is checked for closedness at creation, why this method always returns true.")]
        public static bool IsClosed(this Polygon curve, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/

        [Description("Checks if the BoundaryCurve is closed, i.e. forms a closed loop. A BoundaryCurve is always ensured to be closed at creation, why this method always returns true.")]
        [Input("curve", "The BoundaryCurve to check for closedness.")]
        [Input("tolerance", "Distance tolerance for closedness validation.", typeof(Length))]
        [Output("isClosed", "A BoundaryCurve is checked for closedness at creation, why this method always returns true.")]
        public static bool IsClosed(this BoundaryCurve curve, double tolerance = Tolerance.Distance)
        {
            return true;
        }

        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        [Description("Checks if the ICurve is closed, i.e. forms a closed loop.")]
        [Input("curve", "The ICurve to check for closedness.")]
        [Input("tolerance", "Distance tolerance for closedness validation.", typeof(Length))]
        [Output("isClosed", "Returns true if the ICurve is closed.")]
        public static bool IIsClosed(this ICurve curve, double tolerance = Tolerance.Distance)
        {
            return IsClosed(curve as dynamic, tolerance);
        }


        /***************************************************/
        /**** Public Methods - Nurbs Surfaces           ****/
        /***************************************************/

        [Description("Checks if the NurbsSurface is closed, i.e. forms a closed loop, in at least one direction, i.e. if one of the edges overlap fully with the edge on the oposite side in the u/v domain.")]
        [Input("surface", "The NurbsSurface to check for closedness.")]
        [Input("tolerance", "Distance tolerance for closedness validation.", typeof(Length))]
        [Output("isClosed", "Returns true if the NurbsSurface is closed in at least one direction.")]
        public static bool IsClosed(this NurbsSurface surface, double tolerance = Tolerance.Distance)
        {
            if (surface.IsPeriodic())
                return true;

            double sqTolerance = tolerance * tolerance;
            List<int> uvCount = surface.UVCount();
            return Enumerable.Range(0, uvCount[1]).All(i => surface.ControlPoints[i].SquareDistance(surface.ControlPoints[surface.ControlPoints.Count - uvCount[1] + i]) <= sqTolerance)
                || Enumerable.Range(0, uvCount[0]).All(i => surface.ControlPoints[i * uvCount[1]].SquareDistance(surface.ControlPoints[(i + 1) * uvCount[1] - 1]) <= sqTolerance);
        }

        /***************************************************/
    }
}






