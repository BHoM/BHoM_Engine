/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Structure.Results;
using BH.Engine.Base;

namespace BH.Engine.Results
{
    public static partial class Query
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/

        [Description("Matches results to BHoMObjects. The output consists of a list of items corresponding to the list of BHoMObjects supplied. Each item in the output list is a list of results matching the relevant BHoMObject. The results will be matched by the ID of the object stored in the Fragments and the ObjectId of the result. If no results are found, an empty list will be provided. Note that NO compatibility check between objecttype and result type will be made.")]
        [Input("objects", "The objects to find results for.")]
        [Input("results", "The collection of results to search in.")]
        [Input("whichId", "The name of the object identifier to group results by. Defaults to ObjectId.")]
        [Input("identifier", "The type of IAdapterId fragment to be used to extract the object identification, i.e. which fragment type to look for to find the identifier of the object. If no identifier is provided, the object will be scanned an IAdapterId to be used.")]
        [Input("caseFilter", "Optional filter for the case. If nothing is provided, all cases will be used.")]
        [Output("results", "Results as a List of List where each inner list corresponds to one BHoMObject based on the input order.")]
        public static List<List<T>> MapResults<T>(this IEnumerable<IBHoMObject> objects, IEnumerable<T> results, string whichId = "ObjectId", Type identifier = null, List<string> caseFilter = null) where T : IResult
        {
            //Check if no identifier has been provided. If this is the case, identifiers are searched for on the obejcts
            identifier = objects.ElementAt(0).FindIdentifier(identifier);

            //Filter the results based on case
            IEnumerable<T> filteredRes;
            if (caseFilter != null && caseFilter.Count > 0)
            {
                HashSet<string> caseHash = new HashSet<string>(caseFilter); //Turn to hashset for performance boost
                filteredRes = results.Where(x => caseHash.Contains(x.ResultCase.ToString()));
            }
            else
                filteredRes = results;

            Dictionary<string, IGrouping<string, T>> resGroups;

            //Group results by Id and turn to dictionary
            resGroups = filteredRes.GroupBy(x => Reflection.Query.PropertyValue(x, whichId).ToString()).ToDictionary(x => x.Key);

            // Add null check for when the property of the name in whichId does not exist?

            List<List<T>> result = new List<List<T>>();

            //Run through and put results in List corresponding to objects
            foreach (IBHoMObject o in objects)
            {
                IGrouping<string, T> outVal;
                if (resGroups.TryGetValue(o.IdMatch(identifier), out outVal))
                    result.Add(outVal.ToList());
                else
                    result.Add(new List<T>());
            }

            return result;
        }


        /***************************************************/
        /****           Private Methods                 ****/
        /***************************************************/

        [Description("Gets the AdapterIdName from an object.")]
        [Output("adapterIdName ", "The AdapterIdName of the specified identifier.")]
        private static string IdMatch(this IBHoMObject o, Type identifier)
        {
            IFragment id;
            if (!o.Fragments.TryGetValue(identifier, out id))
                return "";

            return ((IAdapterId)id).Id.ToString();
        }

        /***************************************************/

        [Description("Tries to find a IAdapterId on the object.")]
        [Output("identifier", "First plausible identifier present on the object.")]
        private static Type FindIdentifier(this IBHoMObject o)
        {
            return o.Fragments.FirstOrDefault(fr => fr is IAdapterId)?.GetType();
        }

        /***************************************************/

        [Description("Tries to find a AdapterIdName on the object if no input is provided.")]
        [Output("adapterIdName", "First plausible identifier present on the object or provided.")]
        private static Type FindIdentifier(this IBHoMObject o, Type adapterIdType)
        {
            if (adapterIdType == null)
            {
                adapterIdType = o.FindIdentifier();
                if (adapterIdType == null)
                {
                    //Engine.Reflection.Compute.RecordError("No Identifier found");
                    throw new System.Exception("No Identifier found");
                }
                else
                    Engine.Reflection.Compute.RecordNote("Auto-generated Identifier as " + adapterIdType.Name);
            }
            return adapterIdType;
        }

        /***************************************************/

    }
}
