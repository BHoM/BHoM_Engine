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

        [Description("Checks if two polylines have the same control points in the same order. Either list may be shifted, so a different starting point still matches. If plane is null, the control points are compared directly. If plane is not null, they are projected onto the plane before comparison.")]
        [Input("pLine1", "First polyline.")]
        [Input("pLine2", "Second polyline.")]
        [Input("plane", "If null, control points are compared directly. If not null, control points are projected onto this plane before comparison.")]
        [Input("tolerance", "Distance tolerance.")]
        [Output("isSame", "True if the control points are equal in order, allowing a cyclic shift.")]
        public static bool IsSame(this Polyline pLine1, Polyline pLine2, Plane plane = null, double tolerance = Tolerance.Distance)
        {
            if (pLine1 == null || pLine2 == null)
                return false;

            if (pLine1.ControlPoints.Count != pLine2.ControlPoints.Count)
                return false;

            List<Point> points1 = pLine1.ControlPoints;
            List<Point> points2 = pLine2.ControlPoints;

            if (plane != null)
            {
                points1 = points1.Select(p => p.Project(plane)).ToList();
                points2 = points2.Select(p => p.Project(plane)).ToList();
            }

            int count = points1.Count;

            for (int i = 0; i < count; i++)
            {
                if (!points2[i].IsEqual(points1[0], tolerance))
                    continue;

                bool matches = true;
                for (int j = 0; j < count; j++)
                {
                    if (!points1[j].IsEqual(points2[(i + j) % count], tolerance))
                    {
                        matches = false;
                        break;
                    }
                }

                if (matches)
                    return true;
            }

            return false;
        }

        /***************************************************/
    }
}
