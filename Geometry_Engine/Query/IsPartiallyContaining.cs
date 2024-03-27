/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using System;
using System.Linq;
using System.Collections.Generic;
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        [Description("Determines whether a polyline contains any of the points provided. True if at least 1 point is contained by the polyline.")]
        [Input("polyline", "A polyline container to determine the boundaries of being contained.")]
        [Input("points", "A collection of points to check whether any of them are contained by the polyline.")]
        [Input("acceptOnEdge", "Determine whether to accept a point on the edge of the polyline as being contained or not. Default false")]
        [Output("partiallyContained", "True if at least one of the provided points is contained by the polyline, false if none of the points are.")]
        public static bool IsPartiallyContaining(this Polyline polyline, List<Point> points, bool acceptOnEdge = false)
        {
            foreach (Point p in points)
            {
                if (polyline.IsContaining(new List<Point> { p }, acceptOnEdge))
                    return true;
            }

            return false;
        }
    }
}




