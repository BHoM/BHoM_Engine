/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Reflection
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<object> PropertyObjects(this object obj, bool goDeep = false)
        {
            if(obj == null)
            {
                Base.Compute.RecordWarning("Cannot query the property objects of a null object. An empty list of objects will be returned.");
                return new List<object>();
            }

            List<object> properties = new List<object>();
            foreach (var prop in obj.GetType().GetProperties())
            {
                if (!prop.CanRead || prop.GetMethod.GetParameters().Count() > 0) continue;
                var value = prop.GetValue(obj, null);
                if (value != null && !(value is ValueType))
                {
                    properties.Add(value);
                    if (goDeep)
                    {
                        if (value is IEnumerable)
                        {
                            foreach (object child in (IEnumerable)value)
                                properties.AddRange(PropertyObjects(child, true));
                        }
                        else
                            properties.AddRange(PropertyObjects(value, true));
                    }
                }
            }
            return properties;
        }

        /***************************************************/

        public static Dictionary<Type, List<object>> PropertyObjects(this IEnumerable<object> objects, Type type)
        {
            if(objects == null)
            {
                Base.Compute.RecordWarning("Cannot query the property objects of a null collection of objects. An empty dictionary will be returned.");
                return new Dictionary<Type, List<object>>();
            }

            if(type == null)
            {
                Base.Compute.RecordWarning("Cannot query the property objects of a collection of objects if the type is null. An empty dictionary will be returned.");
                return new Dictionary<Type, List<object>>();
            }

            Dictionary<Type, List<object>> propByType = new Dictionary<Type, List<object>>();
            foreach (var prop in type.GetProperties())
            {
                if (!prop.CanRead || prop.PropertyType.IsValueType || prop.GetMethod.GetParameters().Count() > 0) continue;
                List<object> properties = new List<object>();
                foreach (object obj in objects)
                {
                    var value = prop.GetValue(obj, null);
                    if (value != null)
                        properties.Add(value);
                }
                if (properties.Count > 0)
                    propByType.Add(prop.PropertyType, properties);
            }
            return propByType;
        }

        /***************************************************/
    }
}




