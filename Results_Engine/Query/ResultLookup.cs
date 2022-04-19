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
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Results
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a Lookup (similar to a Dictionary<string, List<T>>) of the result based on the provided identifier.")]
        [Input("results", "Collection of results to be turned into a Lookup.")]
        [Input("resultIdentifier", "Property of the object to use as lookup key.")]
        [Output("lookup", "The created lookup. The key will correspond to the property and value will be all results matching theis key.")]
        public static ILookup<string, T> ResultLookup<T>(this IEnumerable<T> results, string resultIdentifier) where T : IResult
        {
            if (results == null || string.IsNullOrWhiteSpace(resultIdentifier))
                return null;

            Func<T, string> resultIdFunction = ResultIdentifier(results.First(), resultIdentifier);
            return results.ToLookup(resultIdFunction);
        }

        /***************************************************/
    }
}
