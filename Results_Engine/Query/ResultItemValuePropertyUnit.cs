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

        [Description("Returns a QuantityAttribute corresponding to the ValueProperty of the provided result. This will be either a primary property of type double or a Method returning a double with only the type as an input.")]
        [Input("result", "The result type to be used for extraction of the quantity attribute.")]
        [Input("propertyName", "The name of the property or method to get the Qunatity attribute of.")]
        [Output("qantity", "The the extracted quantity attribute.")]
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
           
            return attribute;
        }

        /***************************************************/

        [Description("Returns a QuantityAttribute corresponding to the ValueProperty of the inner ResultItem of the provided ResultCollection. This will be either a primary property of type double or a Method returning a double with only the type as an input.")]
        [Input("result", "The ResultCollection from with the first Result item is used to extract the QUantityAttribute.")]
        [Input("propertyName", "The name of the property or method to get the Qunatity attribute of.")]
        [Output("qantity", "The the extracted quantity attribute.")]
        public static QuantityAttribute ResultItemValuePropertyUnit(this IResultCollection<IResultItem> result, string propertyName)
        {
            if (result == null || propertyName == null)
                return null;

            return ResultItemValuePropertyUnit(result.Results.First(), propertyName);
        }

        /***************************************************/

    }
}
