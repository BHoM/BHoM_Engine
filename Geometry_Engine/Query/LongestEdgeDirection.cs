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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Direction in XY of the longest polyline edge.")]
        [Input("polyline", "Outline polyline.")]
        [Input("tolerance", "Minimum edge length and parallel-direction tolerance.")]
        [Output("direction", "Longest edge direction in XY.")]
        public static Vector LongestEdgeDirection(this Polyline polyline, double tolerance)
        {
            List<Line> edges = polyline.SubParts().Where(x => x != null && x.Length() > tolerance).ToList();
            Line longest = edges.OrderByDescending(x => x.Length()).First();
            Vector longestDir = longest.Direction();
            longestDir.Z = 0;

            Dictionary<Vector, double> dirLen = new Dictionary<Vector, double>();
            foreach (Line edge in edges)
            {
                Vector direction = edge.End - edge.Start;
                direction.Z = 0;

                Vector dir = dirLen.Keys.FirstOrDefault(x => 1 - Math.Abs(x.DotProduct(direction)) <= tolerance);
                if (dir != null)
                    dirLen[dir] += edge.Length();
                else
                    dirLen[direction] = edge.Length();
            }

            return longestDir;
        }
    }
}
