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

using BH.Engine.Base;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Net;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/

        [Description("Returns the largest component as determined by two provided points, dependent upon which axis is more represented by the difference between the two points.")]
        [Input("startPoint", "The start point of vector.")]
        [Input("endPoint", "The end point of the vector.")]
        [Output("largestComponent", "The largest component vector as determined by two provided points.")]
        public static Vector LargestComponent(this Point startPoint, Point endPoint)
        {
            if(startPoint == null || endPoint == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the largest component of a null point.");
                return null;
            }
            
            return LargestComponent(endPoint - startPoint);
        }

        /***************************************************/

        [Description("Returns the largest component as determined by one vector, dependent upon which axis is more represented vector.")]
        [Input("vector", "The vector to determine the largest component of.")]
        [Output("largestComponent", "The largest component vector as determined by the provided vector.")]
        public static Vector LargestComponent(this Vector vector)
        {
            if (vector == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the largest component of a null vector.");
                return null;
            }

            List<Vector> vectorList = new List<Vector>()
            {
                new Vector(){ X = vector.X, Y = 0, Z = 0},
                new Vector(){ X = 0, Y = vector.Y, Z = 0},
                new Vector(){ X = 0, Y = 0, Z = vector.Z}
            };

            return vectorList.OrderByDescending(v => v.SquareLength()).First();
        }

        /***************************************************/

    }
}
