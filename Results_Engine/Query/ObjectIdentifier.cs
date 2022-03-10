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

        [Description("Gets a function taking an IBHoMObejct and returning a string used to identify the object in relation to Results. Method used for maping results to obejcts.")]
        [Input("obj", "The object to get an identifier from.")]
        [Input("identifier", "Should either be a string specifying what property on the object that should be used to map the objects to the results, or a type of IAdapterId fragment to be used to extract the object identification, i.e. which fragment type to look for to find the identifier of the object. If no identifier is provided, the object will be scanned an IAdapterId to be used.")]
        [Output("func", "The function used to identify the object.")]
        public static Func<IBHoMObject, string> ObjectIdentifier(this IBHoMObject obj, object identifier)
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
            else if (identifier is Type)
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

                //Check if string is a type string in a BHoM namespace
                if ((identifier as string).StartsWith("BH."))
                {
                    Type type = Create.Type(identifier as string, true);
                    if (type != null)
                        return ObjectIdentifier(obj, type);
                }

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
    }
}

