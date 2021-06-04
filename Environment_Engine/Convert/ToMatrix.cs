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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using BH.oM.Reflection.Attributes;

namespace BH.Engine.Environment
{
    public static partial class Convert
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a 2D matrix from a rectangular 2D array")]
        [Input("array2D", "A rectangular 2D array")]
        [Output("matrix", "A 2D array")]
        public static T[,] ToMatrix<T>(this IEnumerable<IEnumerable<T>> array2D)
        {
            List<int> shape = (List<int>)array2D.GetShape();
            T[,] ret = new T[shape[0], shape[1]];
            for (int i = 0; i < shape[0]; i++)
            {
                List<T> array = (List<T>)array2D.ToList()[i];
                if (array.Count != shape[1])
                {
                    BH.Engine.Reflection.Compute.RecordError("All arrays must be the same length");
                }
                for (int j = 0; j < shape[1]; j++)
                {
                    ret[i, j] = array[j];
                }
            }
            return ret;
        }
    }
}

