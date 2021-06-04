/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a random 2D matrix of values")]
        [Input("width", "The width of the 2D matrix")]
        [Input("height", "The height of the 2D matrix")]
        [Output("array2D", "A rectangular 2D array of values")]
        public static List<List<double>> RandomArray2D(int width = 2, int height = 3)
        {
            Random rnd = new Random();
            List<List<double>> array = new List<List<double>>();
            for (int i = 0; i < width; i++)
            {
                List<double> subArray = new List<double>();
                for (int j = 0; j < height; j++)
                {
                    subArray.Add(rnd.NextDouble());
                }
                array.Add(subArray);
            }
            return array;
        }

        /***************************************************/
    }
}


