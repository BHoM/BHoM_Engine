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

        [Description("Subtracts a number from each element in an enumerable")]
        [Input("enumerable", "An enumerable")]
        [Input("number", "A number to subtract from each of the elements in the enumerable")]
        [Output("enumerable", "A modified enumerable")]
        public static List<double> Subtract(this List<double> enumerable, double number)
        {
            List<double> modifiedValues = new List<double>();
            foreach (var x in enumerable)
            {
                modifiedValues.Add(x - number);
            }
            return modifiedValues;
        }

        [Description("Perform an element-wise subtraction between two enumerables")]
        [Input("enumerable1", "An enumerable containing values")]
        [Input("enumerable2", "An enumerable containing values")]
        [Output("enumerable", "An enumerable containing values")]
        public static List<double> Subtract(this List<double> enumerable1, List<double> enumerable2)
        {
            if (enumerable1.Count() != enumerable2.Count())
            {
                BH.Engine.Reflection.Compute.RecordError("Enumerable lengths do not match");
                return null;
            }

            List<double> modifiedValues = new List<double>();
            for (int i = 0; i < enumerable1.Count(); i++)
            {
                modifiedValues.Add(enumerable1[i] - enumerable2[i]);
            }

            return modifiedValues;
        }
    }
}


