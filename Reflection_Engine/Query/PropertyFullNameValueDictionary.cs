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

        [Description("Gathers all the properties of the input object, and puts them in a Dictionary with their FullNames and Value.")]
        [Input("obj", "Object you want to get all properties for.")]
        [Input("typeFilter", "(Optional, defaults to null) Filter only properties of this type.")]
        [Input("declaringTypeFilter", "(Optional, defaults to null) Only filter properties that belong to this type.")]
        [Input("maxNesting", "(Optional, defaults to -1) If -1, get all properties at any nesting level. Otherwise, limit the gathering to the specified level.")]
        [Output("dict", "Dictionary whose key is the FullName of the property, and whose Value is the Value of the property.")]
        public static Dictionary<string, object> PropertyFullNameValueDictionary(this object obj, Type typeFilter = null, Type declaringTypeFilter = null, int maxNesting = -1)
        {
            if (obj == null)
                return new Dictionary<string, object>();

            if (typeFilter != null)
            {
                var currentMethod = MethodBase.GetCurrentMethod();

                var result = currentMethod.DeclaringType.GetMethods()
                    .First(mi => mi.Name == currentMethod.Name && mi.GetGenericArguments().Length == 1)
                    .MakeGenericMethod(typeFilter).Invoke(null, new object[] { obj, declaringTypeFilter, maxNesting });

                return result as Dictionary<string, object>;
            }

            return PropertyFullNameValueDictionary<object>(obj, declaringTypeFilter, maxNesting);
        }

        /***************************************************/

        [Description("Gathers all the properties of the input object, and puts them in a Dictionary with their FullNames and Value.")]
        [Input("obj", "Object you want to get all properties for.")]
        [Input("declaringTypeFilter", "(Optional, defaults to null) Only filter properties that belong to this type.")]
        [Input("maxNesting", "(Optional, defaults to -1) If -1, get all properties at any nesting level. Otherwise, limit the gathering to the specified level.")]
        [Output("dict", "Dictionary whose key is the FullName of the property, and whose Value is the Value of the property.")]
        public static Dictionary<string, T> PropertyFullNameValueDictionary<T>(this object obj, Type declaringTypeFilter = null, int maxNesting = -1)
        {
            if (obj == null)
                return new Dictionary<string, T>();

            var propertyFullNameValueDictionary = PropertyFullNameValueDictionary<T>(obj, declaringTypeFilter, maxNesting, false);

            return propertyFullNameValueDictionary as Dictionary<string, T>;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static IDictionary PropertyFullNameValueDictionary<T>(this object obj, Type declaringTypeFilter = null, int maxNesting = -1, bool groupPerNestedLevel = false)
        {
            var result = new Dictionary<string, Dictionary<string, T>>();

            PropertyFullNameValueDictionary<T>(obj.GetType().FullName, 0, obj, declaringTypeFilter, maxNesting, result);

            if (!groupPerNestedLevel)
                return result.Values.ToDictionary(v => v.Keys, v => v.Values);

            return result;
        }

        /***************************************************/

        private static void PropertyFullNameValueDictionary<T>(string fullPath, int nesting, object obj, Type declaringTypeFilter, int maxNesting, Dictionary<string, Dictionary<string, T>> result)
        {
            if (obj == null)
                return;

            IList iList = obj as IList;
            Type objType = obj.GetType();

            if (objType.IsPrimitive())
                return;

            if (iList == null)
            {
                var props = objType.GetProperties();

                foreach (var prop in props)
                {
                    string subPropFullName = $"{fullPath}.{prop.Name}";

                    if (typeof(T).IsAssignableFrom(prop.PropertyType) && (declaringTypeFilter == null || declaringTypeFilter.IsAssignableFrom(prop.DeclaringType)))
                        result.AddToResult<T>((T)prop.GetValue(obj), fullPath, subPropFullName);

                    nesting++;

                    if (prop.PropertyType.IsPrimitive())
                        continue;

                    if (maxNesting < 0 || maxNesting < nesting)
                        if (declaringTypeFilter == null || declaringTypeFilter.IsAssignableFrom(prop.DeclaringType))
                        {
                            object propValue = null;
                            try
                            {
                                propValue = prop.GetValue(obj);
                            }
                            catch { }

                            PropertyFullNameValueDictionary<T>(subPropFullName, nesting, propValue, declaringTypeFilter, maxNesting, result);
                        }
                }
            }
            else
            {
                for (int i = 0; i < iList.Count; i++)
                {
                    var listItem = iList[i];

                    string subPropFullName = $"{fullPath}[{i}]";

                    if (typeof(T).IsAssignableFrom(listItem.GetType()))
                        result.AddToResult<T>((T)listItem, fullPath, subPropFullName);

                    PropertyFullNameValueDictionary<T>(subPropFullName, nesting, listItem, declaringTypeFilter, maxNesting, result);
                }
            }
        }

        /***************************************************/

        private static void AddToResult<T>(this Dictionary<string, Dictionary<string, T>> result, T value, string fullPath, string subPropFullName)
        {
            if (!result.ContainsKey(fullPath))
                result[fullPath] = new Dictionary<string, T>();

            result[fullPath][subPropFullName] = value;
        }
    }
}



