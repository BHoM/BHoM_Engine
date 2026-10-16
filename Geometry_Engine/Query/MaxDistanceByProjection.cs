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
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the max distance along the projection vector between all input lines and the dividing line.")]
        [Input("lines", "Lines to measure against the dividing line.")]
        [Input("dividingLine", "A line crossing the list of input lines.")]
        [Input("projectionVector", "Direction along which the max distance is measured.")]
        [Input("useInfiniteLines", "If true, intersections use infinite lines.")]
        [Output("maxDistanceByProjection", "The max distance along the projection vector between input lines and the dividing line.")]
        public static double MaxDistanceByProjection(this IEnumerable<Line> lines, Line dividingLine, Vector projectionVector, bool useInfiniteLines = false)
        {
            double distance = double.NaN;
            dividingLine.Infinite = true;
            BoundingBox lineBox = lines.Select(x => x.Bounds()).ToList().Bounds();
            double extensionLength = lineBox.Min.Distance(lineBox.Max) * 5;

            foreach (Line edge in lines)
            {
                Point rayOrigin = edge.Start;
                Line ray = new Line { Start = rayOrigin, End = rayOrigin + (projectionVector * extensionLength) };

                Point intersection = ray.LineIntersection(dividingLine, useInfiniteLines);
                if (intersection == null)
                    continue;

                double newDist = intersection.Distance(rayOrigin);
                if (double.IsNaN(distance) || newDist > distance)
                    distance = newDist;
            }

            return distance;
        }

        /***************************************************/
    }
}
