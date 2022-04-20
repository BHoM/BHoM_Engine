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

        [PreviousVersion("5.2", "BH.Engine.Results.Query.ResultItemValueProperty(BH.oM.Analytical.Results.IResultItem, System.String)")]
        [PreviousVersion("5.2", "BH.Engine.Results.Query.ResultItemValueProperty(BH.oM.Analytical.Results.IResultCollection<BH.oM.Analytical.Results.IResultItem>, System.String)")]
        [Description("Returns a Fuction for extracting a value from a result item.")]
        [Input("result", "The result used to extract the type to generate the Func for value extraction.")]
        [Input("propertyName", "The name of the property to get the function for.")]
        [MultiOutput(0, "propertyName", "The name of the property or method function.")]
        [MultiOutput(1, "func", "The result extraction function.")]
        [MultiOutput(2, "quantity", "The quantity corresponding to the function and property.")]
        public static Output<string, Func<IResult, double>, QuantityAttribute> ResultValueProperty(this IResult result, string propertyName, ResultFilter filter = null)
        {
            if (result == null)
                return null;

            if (result is IResultCollection<IResult>)
            {
                //For collections, recursively call the method to find inner identifiers
                return ResultValueProperty((result as IResultCollection<IResult>).Results.FirstOrDefault(), propertyName, filter);
            }
            else if (result is IResultSeries)
            {
                if (filter == null)
                {
                    filter = new ResultFilter();
                    Engine.Base.Compute.RecordNote($"No filter provided. Default index {filter.ResultSeriesIndex} for filter will be used. To control which index to use, please provide a result filter with index set.");
                }
                return CastFunctionToIResult(ResultValuePropertySeries(result as dynamic, propertyName, filter.ResultSeriesIndex) as dynamic);
            }
            else if (result is IResultItem)
            {
                return CastFunctionToIResult(ResultValuePropertyItem(result as dynamic, propertyName) as dynamic);
            }
            else
            {
                Engine.Base.Compute.RecordError("Unsupported result type.");
                return null;
            }
        }

        /***************************************************/

        [PreviousVersion("5.2", "BH.Engine.Results.Query.ResultItemValuePropertyGeneric(BH.oM.Analytical.Results.IResultItem, System.String)")]
        [Description("Returns a Fuction for extracting a value from a result item.")]
        [Input("result", "The result used to extract the type to generate the Func for value extraction.")]
        [Input("propertyName", "The name of the property to get the function for.")]
        [MultiOutput(0, "propertyName", "The name of the property or method function.")]
        [MultiOutput(1, "func", "The result extraction function.")]
        [MultiOutput(2, "quantity", "The quantity corresponding to the function and property.")]
        public static Output<string, Func<T, double>, QuantityAttribute> ResultValuePropertyItem<T>(this T result, string propertyName) where T : IResultItem
        {
            if (result == null)
                return null;

            return ResultValuePropertyGeneric<T, double>(result, propertyName);
        }

        /***************************************************/

        [Description("Returns a Fuction for extracting a value from a result item.")]
        [Input("result", "The result used to extract the type to generate the Func for value extraction.")]
        [Input("propertyName", "The name of the property to get the function for.")]
        [MultiOutput(0, "propertyName", "The name of the property or method function.")]
        [MultiOutput(1, "func", "The result extraction function.")]
        [MultiOutput(2, "quantity", "The quantity corresponding to the function and property.")]
        public static Output<string, Func<T, double>, QuantityAttribute> ResultValuePropertySeries<T>(this T result, string propertyName, int seriesIndex) where T : IResultSeries
        {
            if (result == null)
                return null;

            Output<string, Func<T, IReadOnlyList<double>>, QuantityAttribute> listFunction = ResultValuePropertyGeneric<T, IReadOnlyList<double>>(result, propertyName);

            //Return function that takes the nth item in the extracted IReadOnlyList<double>
            Func<T, double> func = x =>
             {
                 IReadOnlyList<double> values = listFunction.Item2(x);
                 if (values.Count > seriesIndex)
                     return values[seriesIndex];
                 else
                 {
                     Engine.Base.Compute.RecordError($"Provided {nameof(ResultFilter.ResultSeriesIndex)} is larger than the number of results on the provided {nameof(IResultSeries)}. {nameof(double.NaN)} value returned.");
                     return double.NaN;
                 }
 
             };
            return new Output<string, Func<T, double>, QuantityAttribute>
            {
                Item1 = listFunction.Item1,
                Item2 = func,    
                Item3 = listFunction.Item3
            };
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Method for adding a cast to T from IResult for the the function. Required to make the functions simpler to use and avoid generics to be used in visualisation methods for example.")]
        private static Output<string, Func<IResult, double>, QuantityAttribute> CastFunctionToIResult<T>(this Output<string, Func<T, double>, QuantityAttribute> func) where T : IResult
        {
            if (func == null)
                return null;

            return new Output<string, Func<IResult, double>, QuantityAttribute>
            {
                Item1 = func.Item1,
                Item2 = x => func.Item2((T)x),  //Return new function where the Func<T,double> is called with the IResult being cast into T.
                Item3 = func.Item3
            };
        }

        /***************************************************/

        [Description("Extracts the property selector with adjoining quantity of type P and provided name. If no property name is provided, the first property is returned.")]
        private static Output<string, Func<T, P>, QuantityAttribute> ResultValuePropertyGeneric<T, P>(this T result, string propertyName) where T : IResult
        {
            if (result == null)
                return null;

            //Get all properties of the type
            Dictionary<string, Tuple<Func<T, P>, QuantityAttribute>> props = ResultValueProperties<T, P>(result);

            if (props == null || props.Count == 0)
            {
                Compute.RecordError($"No properties available for result of type {result.GetType()}");
                return null;
            }

            Func<T, P> func;
            QuantityAttribute attr;

            Tuple<Func<T, P>, QuantityAttribute> propTuple;
            if (string.IsNullOrEmpty(propertyName))
            {
                var first = props.First();
                if (props.Count != 1)
                    Compute.RecordNote($"No property provided. {first.Key} will be used. Available properties for the type are: {props.Keys.Cast<string>().Aggregate((a, b) => a + ", " + b)}.");
                propertyName = first.Key;
                func = first.Value.Item1;
                attr = first.Value.Item2;
            }
            //Try find the property in the dictionary
            else if (props.TryGetValue(propertyName, out propTuple))
            {
                func = propTuple.Item1;
                attr = propTuple.Item2;
            }
            else
            {
                //If not found, raise error
                Base.Compute.RecordError($"Property {propertyName} is not a valid property for results of type {result.GetType().Name}." +
                    $"Try one of the following: {props.Keys.Cast<string>().Aggregate((a, b) => a + ", " + b)}");
                return null;
            }

            return new Output<string, Func<T, P>, QuantityAttribute>
            {
                Item1 = propertyName,    //Return the property name used. Returned to ensure default values can be extracted (for case where empty string is provided)
                Item2 = func,
                Item3 = attr
            };
        }

        /***************************************************/

    }
}
