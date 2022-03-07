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
        public static Func<IResultItem, double> ResultItemValueProperty(this IResultItem result, string propertyName)
        {
            if (result == null)
                return null;

            return ResultItemValuePropertyGeneric(result as dynamic, propertyName);
        }

        /***************************************************/

        [Description("Returns a Fuction for extracting a value from a result item in the provided collection. The first item in the IResultCollection will be used to find the property to extract.")]
        [Input("result", "The ResultCollection from with the first Result item is used to extract the type to generate the Func for value extraction.")]
        [Input("propertyName", "The name of the property to get the function for.")]
        [Output("func", "The result extraction function.")]
        public static Func<IResultItem, double> ResultItemValueProperty(this IResultCollection<IResultItem> result, string propertyName)
        {
            if (result == null)
                return null;

            return ResultItemValueProperty(result.Results.FirstOrDefault(), propertyName);
        }

        /***************************************************/

        [Description("Returns a Fuction for extracting a value from a result item of type T.")]
        [Input("result", "The result used to extract the type to generate the Func for value extraction.")]
        [Input("propertyName", "The name of the property to get the function for.")]
        [Output("func", "The result extraction function.")]
        public static Func<IResultItem, double> ResultItemValuePropertyGeneric<T>(this T result, string propertyName) where T : IResultItem
        {
            if (result == null)
                return null;

            //Get all properties of the type
            Dictionary<string, Func<T, double>> props = ResultItemValueProperties(result);

            if (props == null || props.Count == 0)
            {
                Compute.RecordError($"No properties available for result of type {result.GetType()}");
                return null;
            }

            Func<T, double> func;

            if (string.IsNullOrEmpty(propertyName))
            {
                var first = props.First();
                if(props.Count != 1)
                    Compute.RecordNote($"No property provided. {first.Key} will be used. Available properties for the type are: {props.Keys.Cast<string>().Aggregate((a, b) => a + ", " + b)}.");
                func = first.Value;
            }
            //Try find the property in the dictionary
            else if (!props.TryGetValue(propertyName, out func))
            {
                //If not found, raise error
                Base.Compute.RecordError($"Property {propertyName} is not a valid property for results of type {result.GetType().Name}." +
                    $"Try one of the following: {props.Keys.Cast<string>().Aggregate((a, b) => a + ", " + b)}");
                return null;
            }

            //Return new function where the Func<T,double> is called with the ResultItem being cast into T.
            return r => func((T)r);
        }

        /***************************************************/
    }
}
