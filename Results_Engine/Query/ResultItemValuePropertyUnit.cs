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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Analytical.Results;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using BH.Engine.Base;

namespace BH.Engine.Results
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a Fuction for extracting a value from a result item.")]
        [Input("result", "The result used to extract the type to generate the Func for value extraction.")]
        [Input("propertyName", "The name of the property to get the function for.")]
        [Output("func", "The result extraction function.")]
        public static QuantityAttribute ResultItemValuePropertyUnit(this IResultItem result, string propertyName)
        {
            if (result == null || propertyName == null)
                return null;

            Dictionary<string, QuantityAttribute> attributes = ResultItemValuePropertiesUnits(result as dynamic);

            QuantityAttribute attribute = null;
            if (attributes != null)
            {
                if (!attributes.TryGetValue(propertyName, out attribute))
                {
                    Base.Compute.RecordError($"Property {propertyName} is not a valid property for results of type {result.GetType().Name}." +
                   $"Try one of the following: {attributes.Keys.Cast<string>().Aggregate((a, b) => a + ", " + b)}");
                }
            }
            else
                Base.Compute.RecordError($"Unable to extract units for properties for result of type {result.GetType()}");
            return attribute;
        }

        /***************************************************/

        [Description("Returns a Fuction for extracting a value from a result item in the provided collection. The first item in the IResultCollection will be used to find the property to extract.")]
        [Input("result", "The ResultCollection from with the first Result item is used to extract the type to generate the Func for value extraction.")]
        [Input("propertyName", "The name of the property to get the function for.")]
        [Output("func", "The result extraction function.")]
        public static QuantityAttribute ResultItemValuePropertyUnit(this IResultCollection<IResultItem> result, string propertyName)
        {
            if (result == null || propertyName == null)
                return null;

            return ResultItemValuePropertyUnit(result.Results.First(), propertyName);
        }

        /***************************************************/

    }
}
