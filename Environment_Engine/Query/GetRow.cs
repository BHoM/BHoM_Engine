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

using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

using BH.oM.Reflection.Attributes;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the element from a 2D array of elements")]
        [Input("array2D", "A 2D array of values")]
        [Input("row", "The row index to access within the provided array")]
        [Output("result", "A list of elements at the queried indices from the input array")]
        public static List<T> GetRow<T>(this List<List<T>> array2D, int row)
        {
            if (array2D.IsJagged())
            {
                BH.Engine.Reflection.Compute.RecordError("The input array is not rectangular");
                return null;
            }

            List<T> ret = new List<T>();
            foreach (List<T> l in array2D)
            {
                ret.Add(l[row]);
            }
            return ret;
        }
    }
}

