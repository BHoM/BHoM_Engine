/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        
        [Description("Gathers all the properties of the input object, their value and their FullNames; then, groups the FullNames and Values of each property per their nesting level.")]
        [Input("obj", "Object you want to get all properties for.")]
        [Input("typeFilter", "(Optional, defaults to null) Filter only properties of this type.")]
        [Input("declaringTypeFilter", "(Optional, defaults to null) Only filter properties that belong to this type.")]
        [Input("maxNesting", "(Optional, defaults to -1) If -1, get all properties at any nesting level. Otherwise, limit the gathering to the specified level.")]
        [Output("dict", "Dictionary whose key is the property Path, and whose Value is another Dictionary, whose Key is the FullName of the property, and whose Value is the Value of the property.")]
        public static Dictionary<string, Dictionary<string, object>> PropertyFullNameValueGroups(this object obj, Type typeFilter = null, Type declaringTypeFilter = null, int maxNesting = -1)
        {
            if (obj == null)
                return new Dictionary<string, Dictionary<string, object>>();

            if (typeFilter != null)
            {
                var currentMethod = MethodBase.GetCurrentMethod();

                var result = currentMethod.DeclaringType.GetMethods()
                    .First(mi => mi.Name == currentMethod.Name && mi.GetGenericArguments().Length == 1)
                    .MakeGenericMethod(typeFilter).Invoke(null, new object[] { obj, declaringTypeFilter, maxNesting }) as IDictionary;

                Dictionary<string, Dictionary<string, object>> output = new Dictionary<string, Dictionary<string, object>>();

                return result as Dictionary<string, Dictionary<string, object>>;
            }

            var propertyFullNameValueDictionary = PropertyFullNameValueDictionary<object>(obj, declaringTypeFilter, maxNesting, true);

            return propertyFullNameValueDictionary as Dictionary<string, Dictionary<string, object>>;
        }

        /***************************************************/

        [Description("Gathers all the properties of specified type from the input object, their value and their FullNames; then, groups the FullNames and Values of each property per their nesting level.")]
        [Input("obj", "Object you want to get all properties for.")]
        [Input("declaringTypeFilter", "(Optional, defaults to null) Only filter properties that belong to this type.")]
        [Input("maxNesting", "(Optional, defaults to -1) If -1, get all properties at any nesting level. Otherwise, limit the gathering to the specified level.")]
        [Output("dict", "Dictionary whose key is the property Path, and whose Value is another Dictionary, whose Key is the FullName of the property, and whose Value is the Value of the property.")]
        public static Dictionary<string, Dictionary<string, T>> PropertyFullNameValueGroups<T>(this object obj, Type declaringTypeFilter = null, int maxNesting = -1)
        {
            if (obj == null)
                return new Dictionary<string, Dictionary<string, T>>();

            var propertyFullNameValueDictionary = PropertyFullNameValueDictionary<T>(obj, declaringTypeFilter, maxNesting, true);

            return propertyFullNameValueDictionary as Dictionary<string, Dictionary<string, T>>;
        }
    }
}





