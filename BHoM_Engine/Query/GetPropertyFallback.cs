/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using BH.oM.Base;
using BH.oM.Base.Attributes;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Fallback nethod to get the value of a property with a given name from a BHoM object. This will try to get the property either from a fragment or from CustomData.")]
        [Input("obj", "BHoM object to get the value from")]
        [Input("propertyName", "name of the property to get the value from")]
        [Input("isSilent", "If true, no warning will be recorded if the property is not found or if multiple matches are found.")]
        [Output("value", "value of the property")]
        public static object GetPropertyFallback(this IBHoMObject obj, string propertyName, bool isSilent = false)
        {
            if (obj.CustomData.ContainsKey(propertyName))
            {
                return obj.CustomData[propertyName];
            }
            else
            {
                IFragment fragment = null;
                Type fragmentType = Create.Type(propertyName, true);
                if (fragmentType != null)
                {
                    List<IFragment> matches = obj.Fragments.Where(fr => fragmentType.IsAssignableFrom(fr.GetType())).ToList();
                    if (!isSilent && matches.Count > 1)
                        Compute.RecordWarning($"The object contains more than one fragment of type {fragmentType.IToText()}. The first one will be returned.");
                    fragment = matches.FirstOrDefault();
                }
                if (!isSilent && fragment == null)
                    Compute.RecordWarning($"The object does not contain a property named {propertyName}, or CustomData[{propertyName}], or a fragment of type {propertyName}.");

                return fragment;
            }
        }

        /***************************************************/
    }
}






