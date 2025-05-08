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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Quantities;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Get the value of a property with a given name from an object")]
        [Input("obj", "object to get the value from")]
        [Input("propName", "name of the property to get the value from")]
        [Output("value", "value of the property")]
        public static object PropertyValue(this object obj, string propName)
        {
            if (obj == null || propName == null)
                return null;

            if (propName.Contains("."))
            {
                string[] props = propName.Split('.');
                foreach (string innerProp in props)
                {
                    obj = obj.PropertyValue(innerProp);
                    if (obj == null)
                        break;
                }
                return obj;
            }

            System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(propName);

            object result = null;
            if (prop != null)
                result = prop.GetValue(obj);
            else
                result = GetValue(obj as dynamic, propName);

            if (result is IQuantity)
                result = ((IQuantity)result).Value;

            return result;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static object GetValue(this IObject obj, string propName)
        {
			if (obj == null || propName == null)
				return null;

			object result = null;

            // Handle Dynamic property providers
			if (obj is IDynamicPropertyProvider)
            {
				if (Compute.TryRunExtensionMethod(obj, "GetProperty", new object[] { propName }, out result))
                    return result;
			}

            // Handle dynamic objects
            if (obj is IDynamicObject)
            {
				List<PropertyInfo> dynamicProperties = obj.GetType().GetProperties()
					.Where(x => x.GetCustomAttribute<DynamicPropertyAttribute>() != null && typeof(IDictionary).IsAssignableFrom(x.PropertyType) && x.PropertyType.GenericTypeArguments.First().IsEnum)
					.ToList();

				// Try to extract from a dynamic property
				foreach (PropertyInfo prop in dynamicProperties)
				{
					if (TryGetDynamicValue(prop.GetValue(obj) as dynamic, propName, out result))
                        return result;
				}
			}

            // Handle BHoM objects
            if (obj is IBHoMObject)
                result = GetPropertyFallback(obj as IBHoMObject, propName);

            return result;
        }

        /***************************************************/

        private static object GetValue<T>(this Dictionary<string, T> dic, string propName)
        {
            if (dic.ContainsKey(propName))
            {
                return dic[propName];
            }
            else
            {
                Compute.RecordWarning($"{dic} does not contain the key: {propName}");
                return null;
            }
        }

        /***************************************************/

        private static object GetValue<K, T>(this Dictionary<K, T> dic, string propName) where K : struct, Enum
        {
            K key;
            if (!Enum.TryParse(propName, out key))
            {
                Compute.RecordError($"cannot convert {propName} into an enum of type {typeof(K)}");
                return null;
            }
            else if (dic.ContainsKey(key))
            {
                return dic[key];
            }
            else
            {
                Compute.RecordWarning($"{dic} does not contain the key: {propName}");
                return null;
            }
        }

        /***************************************************/

        private static bool TryGetDynamicValue<K, T>(this Dictionary<K, T> dic, string propName, out object result) where K : struct, Enum
        {
            result = null;
            bool isCorrectContainer = Enum.TryParse(propName, out K key);

            if (isCorrectContainer)
            {
                if (dic.ContainsKey(key))
                    result = dic[key];
                else
                    Compute.RecordWarning($"The object does not contain a dynamic property named {propName}");
            } 
                
            return isCorrectContainer;
        }

        /***************************************************/

        private static object GetValue<T>(this IEnumerable<T> obj, string propName)
        {
            return obj.Select(x => x.PropertyValue(propName)).ToList();
        }


        /***************************************************/
        /**** Fallback Methods                           ****/
        /***************************************************/

        private static object GetValue(this object obj, string propName)
        {
            Compute.RecordWarning($"This instance of {obj.GetType()} does not contain the property: {propName}");
            return null;
        }

        /***************************************************/
    }
}






