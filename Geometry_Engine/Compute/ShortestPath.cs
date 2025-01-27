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

using System;
using System.Collections.Generic;
using System.Text;
using BH.oM.Geometry;
using System.Linq;
using System.ComponentModel;
using BH.oM.Base.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        [Description("A simplified method of sorting a list of points based on how close a given point is to the previous point. Based on the Travelling Salesperson Problem (TSP) but simpler and non-globally-optimal in the general case.")]
        [Input("points", "The list of points which to sort.")]
        [Output("sortedPoints", "The sorted list of points.")]
        public static List<Point> ShortestPath(List<Point> points)
        {
            if (points == null || points.Count == 0) 
                return null;

            List<Point> sortedPoints = new List<Point>();

            List<Point> ringPoints = points.ToList();

            sortedPoints.Add(ringPoints[0]);
            ringPoints.RemoveAt(0);

            while (ringPoints.Count > 0)
            {
                Point np = Query.ClosestPoint(sortedPoints.Last(), ringPoints);
                if (np == null)
                    break;

                sortedPoints.Add(np);
                ringPoints.Remove(np);     
            }

            return sortedPoints;
        }
    }
}


