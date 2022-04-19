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

namespace BH.Engine.Results
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the result value carrying properties available for the result type provided. Currently only supported for IResultItem and IResultCollection<IResultItem> type results.")]
        [Input("result", "The result to fetch applicable result properties from.")]
        [Output("props", "Result value carrying properties for the result type.")]
        public static List<string> ResultPropertyKeys(this IResult result)
        {
            if (result == null)
                return new List<string>();

            if (result is IResultCollection<IResult>)
            {
                return ResultPropertyKeys((result as IResultCollection<IResult>).Results.FirstOrDefault());
            }
            else if (result is IResultSeries)
            {
                IDictionary dict = ResultValuePropertiesSeries(result as dynamic);
                if (dict != null)
                    return dict.Keys.Cast<string>().ToList();
            }
            else if (result is IResultItem)
            {
                IDictionary dict = ResultValuePropertiesItem(result as dynamic);
                if (dict != null)
                    return dict.Keys.Cast<string>().ToList();
            }
            else
            {
                Engine.Base.Compute.RecordError($"Unsupported result type. Currently only suporting {nameof(IResultItem)} and {nameof(IResultCollection<IResultItem>)} type results.");
            }

            return new List<string>();
        }

        /***************************************************/
    }
}
