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

using BH.oM.Analytical.Results;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.Engine.Base;
using System.Reflection;

namespace BH.Engine.Results
{
    public static partial class Query
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [PreviousVersion("5.1", "BH.Engine.Results.Query.MapResults(System.Collections.Generic.IEnumerable<BH.oM.Base.IBHoMObject>, System.Collections.Generic.IEnumerable<BH.oM.Analytical.Results.IResult>, System.String, System.Type, System.Collections.Generic.List<System.String>)")]
        [PreviousInputNames("objectIdentifier", "identifier")]
        [PreviousInputNames("resultIdentifier", "whichId")]
        [PreviousInputNames("filter", "caseFilter")]
        [Description("Matches results to BHoMObjects. The output consists of a list of items corresponding to the list of BHoMObjects supplied. Each item in the output list is a list of results matching the relevant BHoMObject. The results will be matched by the ID of the object stored in the Fragments and the ObjectId of the result. If no results are found, an empty list will be provided. Note that NO compatibility check between object type and result type will be made.")]
        [Input("objects", "The objects to find results for.")]
        [Input("results", "The collection of results to search in.")]
        [Input("resultIdentifier", "Property of the IResult used to map the result to the Object. Defaults to ObjectId.")]
        [Input("objectIdentifier", "Should either be a string specifying what property on the object that should be used to map the objects to the results, or a type of IAdapterId fragment to be used to extract the object identification, i.e. which fragment type to look for to find the identifier of the object. If no identifier is provided, the object will be scanned an IAdapterId to be used.")]
        [Input("filter", "Optional filter for the results. If nothing is provided, all results will be used.")]
        [Output("results", "Results as a List of List where each inner list corresponds to one BHoMObject based on the input order.")]
        public static List<List<TResult>> MapResultsToObjects<TResult, TObject>(this IEnumerable<TObject> objects, IEnumerable<TResult> results, string resultIdentifier = nameof(IObjectIdResult.ObjectId), object objectIdentifier = null, ResultFilter filter = null)
            where TResult : IResult
            where TObject : IBHoMObject
        {
            if (objects == null || objects.Count() < 1)
            {
                Engine.Base.Compute.RecordError("No objects found. Make sure that your objects are input correctly.");
                return new List<List<TResult>>();
            }
            if (results == null || results.Count() < 1)
            {
                Engine.Base.Compute.RecordError("No results found. Make sure that your results are input correctly.");
                return new List<List<TResult>>();
            }

            Func<IBHoMObject, string> objectIdFunction = ObjectIdentifier(objects.First(), objectIdentifier as string);

            if (objectIdFunction == null)
                return new List<List<TResult>>();

            //Filter the results based on provided filter
            IEnumerable<TResult> filteredRes = results.FilterResults(filter);

            //Turn to Lookup based on the result ID function
            //Add null check for when the property of the name in whichId does not exist?
            ILookup<string, TResult> resGroups = filteredRes.ResultLookup(resultIdentifier);

            if (resGroups == null)
                return new List<List<TResult>>();

            List<List<TResult>> result = new List<List<TResult>>();

            //Run through and put results in List corresponding to objects
            foreach (TObject o in objects)
            {
                result.Add(resGroups[objectIdFunction(o)].ToList());
            }

            return result;
        }

        /***************************************************/ 
    }
}

