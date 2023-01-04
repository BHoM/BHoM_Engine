/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Analytical.Results;
using BH.oM.Structure.Loads;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using BH.Engine.Base;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Filters out results from a specific case given an identifier, Loadcase or LoadCombination. For Loadcase and LoadCombination the number property will be used as identifier for filtering.")]
        [Input("results", "The list of results to filter.")]
        [Input("loadcase", "The case or combination to filter by. Should either be a string/int as identifier of the case or a Loadcase/LoadCombination where the number will be used as the identifier. If identifier can be extracted, all results are returned.")]
        [Output("results", "The filtered results. If no filtering param could be extracted, all results are returned.")]
        public static List<T> SelectCase<T>(this List<T> results, object loadcase) where T : ICasedResult
        {
            if (results.IsNullOrEmpty())
                return null;

            if (loadcase != null)
            {
                string loadCaseId = null;

                if (loadcase is string)
                    loadCaseId = loadcase as string;
                else if (loadcase is ICase)
                    loadCaseId = (loadcase as ICase).Number.ToString();
                else if (loadcase is int || loadcase is double)
                    loadCaseId = loadcase.ToString();

                if (!string.IsNullOrWhiteSpace(loadCaseId))
                    return results.Where(x => x.ResultCase.ToString() == loadCaseId).ToList();
                else
                    Base.Compute.RecordWarning("Could not extract filter identifier from the provided loadcase filter. All results are returned.");
            }
            else
                Base.Compute.RecordWarning("Loadcase filter is null. No filtering is applied. All results are returned.");

            return results;
        }

        /***************************************************/
    }
}




