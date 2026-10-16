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

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the minimum distance between any two distinct points in the list.")]
        [Input("points", "Points to compare.")]
        [Output("minDistance", "Smallest pairwise distance, or MaxValue if fewer than two points.")]
        public static double MinInternalDistance(this List<Point> points)
        {
            double minDistance = double.MaxValue;

            foreach (Point point1 in points)
            {
                foreach (Point point2 in points)
                {
                    if (point1 == point2)
                        continue;

                    double distance = point1.Distance(point2);
                    minDistance = Math.Min(distance, minDistance);
                }
            }

            return minDistance;
        }

        /***************************************************/
    }
}
