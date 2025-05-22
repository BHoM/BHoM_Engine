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

using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Base.Reflection;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<string> PropertyNames(this object obj)
        {
            if(obj == null)
            {
                Base.Compute.RecordWarning("Cannot query the property names of a null object. An empty list of names will be returned.");
                return new List<string>();
            }

            if (obj is CustomObject)
                return PropertyNames(obj as CustomObject);
            else if (obj is IDynamicObject)
                return PropertyNames(obj as IDynamicObject);
            else
                return obj.GetType().PropertyNames();
        }

        /***************************************************/

        public static List<string> PropertyNames(this Type type)
        {
            if(type == null)
            {
                Base.Compute.RecordWarning("Cannot query the property names of a null type. An empty list of names will be returned.");
                return new List<string>();
            }

            List<string> names = new List<string>();
            foreach (var prop in type.GetProperties())
            {
                if (!prop.CanRead) continue;
                names.Add(prop.Name);
            }
            return names;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<string> PropertyNames(this CustomObject obj)
        {
            return obj.GetType().PropertyNames().Where(x => x != "CustomData").Concat(obj.CustomData.Keys.ToList()).ToList();
        }

        /***************************************************/

        private static List<string> PropertyNames(this IDynamicObject obj)
        {
            if (obj is IDynamicPropertyProvider)
            {
                object result;
                bool success = BH.Engine.Base.Compute.TryRunExtensionMethod(obj, "GetProperties", new object[] { }, out result);
                List<Property> properties = result as List<Property>;

                if (success && properties != null)
                    return properties.Select(x => x.Name).ToList();
                else
                    return new List<string>();
            }
            else
            {
                List<string> properties = new List<string>();

                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (prop.GetCustomAttribute<DynamicPropertyAttribute>() != null
                        && typeof(IDictionary).IsAssignableFrom(prop.PropertyType)
                        && prop.PropertyType.GenericTypeArguments.First().IsEnum)
                    {
                        IDictionary dic = prop.GetValue(obj) as IDictionary;
                        properties.AddRange(dic.Keys.OfType<Enum>().OrderBy(x => x).Select(x => x.ToString()).Where(x => !string.IsNullOrEmpty(x)));

                    }
                    else
                    {
                        properties.Add(prop.Name);
                    }
                }

                return properties;
            }
                
        }

        /***************************************************/
    }
}






