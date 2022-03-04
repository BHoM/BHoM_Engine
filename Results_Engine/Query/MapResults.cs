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

        [PreviousInputNames("objectIdentifier", "identifier")]
        [PreviousInputNames("resultIdentifier", "whichId")]
        [Description("Matches results to BHoMObjects. The output consists of a list of items corresponding to the list of BHoMObjects supplied. Each item in the output list is a list of results matching the relevant BHoMObject. The results will be matched by the ID of the object stored in the Fragments and the ObjectId of the result. If no results are found, an empty list will be provided. Note that NO compatibility check between object type and result type will be made.")]
        [Input("objects", "The objects to find results for.")]
        [Input("results", "The collection of results to search in.")]
        [Input("resultIdentifier", "Property of the IResult used to map the result to the Object. Defaults to ObjectId.")]
        [Input("objectIdentifier", "Should either be a string specifying what property on the object that should be used to map the objects to the results, or a type of IAdapterId fragment to be used to extract the object identification, i.e. which fragment type to look for to find the identifier of the object. If no identifier is provided, the object will be scanned an IAdapterId to be used.")]
        [Input("caseFilter", "Optional filter for the case. If nothing is provided, all cases will be used.")]
        [Output("results", "Results as a List of List where each inner list corresponds to one BHoMObject based on the input order.")]
        public static List<List<TResult>> MapResults<TResult, TObject>(this IEnumerable<TObject> objects, IEnumerable<TResult> results, string resultIdentifier = nameof(IObjectIdResult.ObjectId), object objectIdentifier = null, List<string> caseFilter = null)
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

            Func<IBHoMObject, string> objectIdFunction = GetObjectIdentifier(objects.First(), objectIdentifier as string);

            if (objectIdFunction == null)
                return new List<List<TResult>>();

            //Filter the results based on case
            IEnumerable<TResult> filteredRes;
            if (caseFilter != null && caseFilter.Count > 0)
            {
                HashSet<string> caseHash = new HashSet<string>(caseFilter); //Turn to hashset for performance boost
                filteredRes = results.OfType<ICasedResult>().Where(x => caseHash.Contains(x.ResultCase.ToString())).OfType<TResult>();
            }
            else
                filteredRes = results;


            Func<TResult, string> resultIdFunction = GetResultIdentifier(results.First(), resultIdentifier);
            //Turn to Lookup based on the result ID function
            //Add null check for when the property of the name in whichId does not exist?
            var resGroups = filteredRes.ToLookup(resultIdFunction);

            List<List<TResult>> result = new List<List<TResult>>();

            //Run through and put results in List corresponding to objects
            foreach (TObject o in objects)
            {
                result.Add(resGroups[objectIdFunction(o)].ToList());
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

        private static Func<IBHoMObject, string> GetObjectIdentifier(this IBHoMObject obj, object identifier)
        {
            //Check if no identifier has been provided. 
            if (identifier == null)
            {
                //If this is the case, identifiers are searched for on the objects
                identifier = obj.FindIdentifier();
                if (identifier != null)
                {
                    //If an adapterId identifier is found, use it
                    Type typeId = identifier as Type;
                    return x => IdMatch(x, typeId);
                }
                else
                {
                    //If not, rely on BHoM_Guid
                    Engine.Base.Compute.RecordNote("No identifier found or specified. BHoM_Guid will be used to identify the Object.");
                    return x => x.BHoM_Guid.ToString();
                }
            }
            if(identifier.GetType() == typeof(Type))
            {
                //If identifier type provided is Type, check that the type is valid
                identifier = obj.FindIdentifier(identifier as Type);
                if (identifier != null)
                {
                    Type typeId = identifier as Type;
                    return x => IdMatch(x, typeId);
                }
                else
                    return null;
            }
            else if (identifier is string)
            {
                //If string
                string idString = (identifier as string).ToLower();

                //Check if name or Guid. If so return property extractor as optimisation
                if (idString == "name")
                    return x => x.Name;

                if (idString == "bhom_guid" || idString == "guid")
                    return x => x.BHoM_Guid.ToString();

                //If not, rely on the slower running but more generic PropertyValue
                return x => Base.Query.PropertyValue(x, identifier as string).ToString();
            }


            Engine.Base.Compute.RecordError("Identifier should either be a IAdapterId type or a string corresponding to the property of the object used to be used to identify the object.");
            return null;
        }

        /***************************************************/

        private static Func<T, string> GetResultIdentifier<T>(this T obj, string identifier) where T : IResult
        {
            //If result type and identifier match for ObjectIdResult or MeshResult, create custom lamdas to speed things up
            string idLower = identifier.ToLower();
            if (idLower == nameof(IObjectIdResult.ObjectId).ToLower() && obj is IObjectIdResult)
                return x => ((IObjectIdResult)x).ObjectId.ToString();

            if (idLower == nameof(IMeshElementResult.NodeId).ToLower() && obj is IMeshElementResult)
                return x => ((IMeshElementResult)x).NodeId.ToString();

            if (idLower == nameof(IMeshElementResult.MeshFaceId).ToLower() && obj is IMeshElementResult)
                return x => ((IMeshElementResult)x).MeshFaceId.ToString();

            //Fall back on PropertyValue
            return x => Base.Query.PropertyValue(x, identifier).ToString();
        }

        /***************************************************/

    }
}

