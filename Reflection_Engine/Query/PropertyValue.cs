/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Reflection
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

            if (prop != null)
                return prop.GetValue(obj);
            else 
                return GetValue(obj as dynamic, propName);

        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static object GetValue(this IBHoMObject obj, string propName)
        {
            IBHoMObject bhom = obj as IBHoMObject;
            if (bhom.CustomData.ContainsKey(propName))
            {
                if (!(bhom is CustomObject))
                    Compute.RecordNote($"{propName} is stored in CustomData");
                return bhom.CustomData[propName];
            }
            else
            {
                Compute.RecordWarning($"{bhom} does not contain a property: {propName}, or: CustomData[{propName}]");
                return null;
            }
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

