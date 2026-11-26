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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public methods               ****/
        /***************************************************/

        [Description("Checks if two polylines have the same control points, regardless of starting point and direction. Closed polylines are compared cyclically (any shifted start and either orientation match); open polylines match when their points are equal in forward or reversed order. If plane is null, the control points are compared directly. If plane is not null, they are projected onto the plane before comparison.")]
        [Input("pLine1", "First polyline.")]
        [Input("pLine2", "Second polyline.")]
        [Input("plane", "If null, control points are compared directly. If not null, control points are projected onto this plane before comparison.")]
        [Input("tolerance", "Distance tolerance.")]
        [Output("isSame", "True if the polylines share the same points, allowing a cyclic shift and reversed direction for closed polylines, or reversed direction for open polylines.")]
        public static bool IsSame(this Polyline pLine1, Polyline pLine2, Plane plane = null, double tolerance = Tolerance.Distance)
        {
            if (pLine1 == null || pLine2 == null)
                return false;

            if (pLine1.ControlPoints.Count != pLine2.ControlPoints.Count)
                return false;

            bool isClosed = pLine1.IsClosed(tolerance);
            if (isClosed != pLine2.IsClosed(tolerance))
                return false;

            List<Point> points1 = pLine1.ControlPoints;
            List<Point> points2 = pLine2.ControlPoints;

            if (plane != null)
            {
                points1 = points1.Select(p => p.Project(plane)).ToList();
                points2 = points2.Select(p => p.Project(plane)).ToList();
            }

            //For closed polylines the last point duplicates the first one, so exclude it to allow comparison of cyclic shifts.
            if (isClosed)
            {
                points1 = points1.Take(points1.Count - 1).ToList();
                points2 = points2.Take(points2.Count - 1).ToList();
            }

            int count = points1.Count;
            if (count == 0)
                return true;

            if (MatchesFrom(points1, points2, 0, false, tolerance))
                return true;

            List<Point> reversed = Enumerable.Reverse(points2).ToList();
            if (!isClosed)
                return MatchesFrom(points1, reversed, 0, false, tolerance);

            for (int shift = 0; shift < count; shift++)
            {
                if (MatchesFrom(points1, points2, shift, true, tolerance) || MatchesFrom(points1, reversed, shift, true, tolerance))
                    return true;
            }

            return false;
        }

        /***************************************************/
        /****              Private methods              ****/
        /***************************************************/

        private static bool MatchesFrom(List<Point> points1, List<Point> points2, int shift, bool cyclic, double tolerance)
        {
            int count = points1.Count;
            for (int i = 0; i < count; i++)
            {
                int j = cyclic ? (i + shift) % count : i;
                if (!points1[i].IsEqual(points2[j], tolerance))
                    return false;
            }

            return true;
        }

        /***************************************************/
    }
}
