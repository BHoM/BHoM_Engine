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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Diffing;
using BH.oM.Base;
using System.Reflection;
 

namespace BH.Engine.Diffing
{
    public static partial class Query
    {
        [Description("Can be used to 'explode' the result of the Diffing method called `DifferentProperties`.")]
        [Input("diffProps", "Result of the method `DifferentProperties`. A dictionary whose key is the name of the property, and value is a tuple with the different values found in obj1 and obj2.")]
        [MultiOutput(0, "propNames", "List of properties changed per each object.")]
        [MultiOutput(1, "value_obj1", "List of current values of the properties.")]
        [MultiOutput(2, "value_obj2", "List of past values of the properties.")]
        public static Output<List<string>, List<object>, List<object>> ListDifferentProperties(this Dictionary<string, Tuple<object, object>> diffProps)
        {
            var output = new Output<List<string>, List<object>, List<object>>();

            List<string> propNameList = new List<string>();
            List<object> propValue_CurrentList = new List<object>();
            List<object> propValue_ReadList = new List<object>();

            // These first empty assignments are needed to avoid UI to throw error "Object not set to an instance of an object" when input is null.
            output.Item1 = propNameList;
            output.Item2 = propValue_CurrentList;
            output.Item3 = propValue_ReadList;

            if (diffProps == null)
                return output;

            foreach (var item in diffProps)
            {
                propNameList.Add(item.Key);
                propValue_CurrentList.Add(item.Value.Item1);
                propValue_ReadList.Add(item.Value.Item2);
            }

            output.Item1 = propNameList;
            output.Item2 = propValue_CurrentList;
            output.Item3 = propValue_ReadList;

            return output;
        }
    }
}





