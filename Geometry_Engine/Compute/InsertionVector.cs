/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2022, the respective contributors. All rights reserved.
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

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/

        [Description("Returns the insertion vector for element placement, dependent upon the number of elements and the start and end points.")]
        [Input("elementCount", "Number of elements to place between the two points.")]
        [Input("insertionStartPoint", "The start point of the insertion vector to create the Symbols.")]
        [Input("insertionEndPoint", "The end point of the insertion vector to create the Symbols.")]
        [Output("insertionVector", "The insertion vector with the correct length to create the Symbols.")]
        public static Vector InsertionVector(Point insertionStartPoint, Point insertionEndPoint, int elementsCount = 1)
        {
            return (insertionStartPoint - insertionEndPoint) / (elementsCount == 1 ? 1 : elementsCount - 1);
        }

        /***************************************************/

        [Description("Returns the orthogonal insertion vector as determined by two provided points, dependent upon which axis is more represented by the difference between the two points.")]
        [Input("insertionStartPoint", "The start point of the insertion vector.")]
        [Input("insertionEndPoint", "The end point of the insertion vector.")]
        [Output("orthogonalInsertionVector", "The orthogonal insertion vector as determined by two provided points.")]
        public static Vector OrthogonalInsertionVector(Point insertionStartPoint, Point insertionEndPoint, int elementsCount = 1)
        {
            //If points equal, place in the same exact location
            if (insertionStartPoint == insertionEndPoint)
                return new Vector()
                {
                    X = 0,
                    Y = 0,
                    Z = 0
                };

            //Largest component vector is preferred direction and length
            List<Vector> vectorList = new List<Vector>()
            {
                new Vector(){X = (insertionEndPoint - insertionStartPoint).X, Y = 0, Z = 0},
                new Vector(){X =0, Y = (insertionEndPoint - insertionStartPoint).Y, Z = 0},
                new Vector(){X =0, Y = 0, Z = (insertionEndPoint - insertionStartPoint).Z}
            };

            return vectorList.OrderBy(v => v.Length()).Last() / (elementsCount == 1 ? 1 : elementsCount - 1);
        }

        /***************************************************/

    }
}



