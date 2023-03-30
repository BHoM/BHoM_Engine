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

using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System;
using System.ComponentModel;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Gets the distance between a point and a line along the input vector's direction. Input line and point must be on the same plane.")]
        [Input("line", "A line to compute distance to the input point.")]
        [Input("point", "A point to compute distance to the input line.")]
        [Input("vector", "A vector along which we will measure the distance from the point to the line.")]
        [Output("distance", "The distance between the input point and line along the input vector's direction.")]
        public static double DistanceAlongVector(this Line line, Point point, Vector directionVector)
        {
            Vector projectionVector = point - point.Project(line);

            return Math.Abs(projectionVector.DotProduct(directionVector.Normalise()));
        }

        /***************************************************/
    }
}




