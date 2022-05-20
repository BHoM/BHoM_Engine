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

        [Description("Filters the results based on the provided ResultFilter.\n" +
                     "If ResultCaseFilters contains items, only results of type ICasedResult will be returned that has a ResultCase matching any items in the ResultCaseFilters.\n" +
                     "If TimeStepFilters contains items, only results of type ITimeStepResult will be returned that has a TimeStep matching any items in the TimeStepFilters.\n" +
                     "If ObjectIDFilters contains items, only results of type IObjectIdResult will be returned that has a ObjectId matching any items in the ObjectIDFilters.\n" +
                     "If NodeIDFilters contains items, only results of type IMeshElementResult will be returned that has a NodeId matching any items in the NodeIDFilters.\n" +
                     "If MeshFaceIDFilters contains items, only results of type IMeshElementResult will be returned that has a MeshFaceId matching any items in the MeshFaceIDFilters.")]
        [Input("results", "Results to filter.")]
        [Input("filter", "Filtering to be applied to the result.")]
        [Output("filteredResults", "The filtered results.")]
        public static IEnumerable<T> FilterResults<T>(this IEnumerable<T> results, ResultFilter filter) where T : IResult
        {
            if (results == null || filter == null)
                return results;

            IEnumerable<T> filteredRes = results;

            if (filter.ResultCaseFilters != null && filter.ResultCaseFilters.Count > 0)
                filteredRes = filteredRes.OfType<ICasedResult>().Where(x => filter.ResultCaseFilters.Contains(x.ResultCase?.ToString())).OfType<T>();

            if (filter.TimeStepFilters != null && filter.TimeStepFilters.Count > 0)
                filteredRes = filteredRes.OfType<ITimeStepResult>().Where(x => filter.TimeStepFilters.Contains(x.TimeStep)).OfType<T>();

            if (filter.ObjectIDFilters != null && filter.ObjectIDFilters.Count > 0)
                filteredRes = filteredRes.OfType<IObjectIdResult>().Where(x => filter.ObjectIDFilters.Contains(x.ObjectId?.ToString())).OfType<T>();

            if (filter.NodeIDFilters != null && filter.NodeIDFilters.Count > 0)
                filteredRes = filteredRes.OfType<IMeshElementResult>().Where(x => filter.NodeIDFilters.Contains(x.NodeId?.ToString())).OfType<T>();

            if (filter.MeshFaceIDFilters != null && filter.MeshFaceIDFilters.Count > 0)
                filteredRes = filteredRes.OfType<IMeshElementResult>().Where(x => filter.MeshFaceIDFilters.Contains(x.MeshFaceId?.ToString())).OfType<T>();


            return filteredRes;
        }

        /***************************************************/
    }
}
