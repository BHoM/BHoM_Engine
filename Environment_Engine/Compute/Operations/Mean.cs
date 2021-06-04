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

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using BH.oM.Reflection.Attributes;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        /***************************************************/
        /****          Public Methods                   ****/
        /***************************************************/

        [Description("Get the mean for all values in the given axis in a rectangular 2D array")]
        [Input("array2D", "A rectangular 2D array of values")]
        [Input("axis", "The axis over which mean should be calculated. 0 is column-wise, and 1 is row-wise")]
        [Output("values", "A list of values summed about the given axis")]
        public static List<double> Mean(this List<List<double>> array2D, int axis = 0)
        {
            if (array2D.IsJagged())
            {
                BH.Engine.Reflection.Compute.RecordError("The input array is not rectangular and row/column mean cannot be completed.");
            }

            if (axis == 0)
            {
                List<double> ret = new List<double>();
                foreach (List<double> item in array2D)
                {
                    ret.Add(item.Average());
                }
                return ret;
            }
            else if (axis == 1)
            {
                List<int> shape = (List<int>)array2D.GetShape();
                List<double> ret = new List<double>();
                for (int i = 0; i < shape[1]; i++)
                {
                    ret.Add(array2D.GetRow(i).Average());
                }
                return ret;
            }
            else
            {
                BH.Engine.Reflection.Compute.RecordError("The axis specified is not present in a 2D array.");
                return null;
            }
        }
    }
}


