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

using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks whether a polyline intersects or contains a curve. Lines and polylines use their own intersection tests. Any other curve is inside only when it does not cross the boundary and its start point is inside.")]
        [Input("region", "Polyline region.")]
        [Input("curve", "Line, polyline, or other curve to test.")]
        [Input("tolerance", "Distance tolerance.", typeof(Length))]
        [Output("intersectsOrContains", "True if the curve crosses the polyline or lies inside it.")]
        public static bool IntersectsOrContains(this Polyline region, ICurve curve, double tolerance = Tolerance.Distance)
        {
            if (region == null || curve == null)
                return false;

            if (curve is Line line)
            {
                if (region.LineIntersections(line, false, tolerance)?.Count > 0)
                    return true;

                return region.IsContaining(new List<Point> { line.Start, line.End }, true, tolerance);
            }

            if (curve is Polyline other)
            {
                if (region.LineIntersections(other, tolerance)?.Count > 0)
                    return true;

                if (other.ControlPoints != null && other.ControlPoints.Count > 0 && region.IsContaining(other.ControlPoints, true, tolerance))
                    return true;

                return region.ControlPoints != null && region.ControlPoints.Count > 0 && other.IsContaining(region.ControlPoints, true, tolerance);
            }

            if (region.ICurveIntersections(curve, tolerance)?.Count > 0)
                return true;

            Point sample = curve.IStartPoint();
            return sample != null && region.IsContaining(new List<Point> { sample }, true, tolerance);
        }

        /***************************************************/
    }
}
