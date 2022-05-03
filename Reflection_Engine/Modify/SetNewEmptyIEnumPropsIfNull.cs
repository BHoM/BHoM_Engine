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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Base;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using System.Collections;

namespace BH.Engine.Reflection
{
    public static partial class Modify
    {
        [Description("Iterates the properties of an object; if the property type is a subtype of IEnumerable and it is null, " +
            "this method replaces the null with a new empty IEnumerable of the correct type." +
            "The method uses the default parameterless constructor of the property type's IEnumerable, so if the IEnumerable requires any input parameter, this method will return an error.")]
        [Input("obj", "Object whose properties are examined. Any property that is null and whose type is a subtype of IEnumerable will have its value replaced with a new empty IEnumerable of the correct type.")]
        [Output("obj", "Object with the null IEnumerable properties replaced with empty IEnumerables of the correct type.")]
        public static void SetNewEmptyIEnumPropsIfNull(this object obj)
        {
            var props = obj.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (!prop.CanRead || prop.GetMethod.GetParameters().Count() > 0 || !prop.CanWrite)
                    continue;

                var value = prop.GetValue(obj, null);

                if (value == null && typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
                {
                    try
                    {
                        object newEmptyIEnumerable = System.Activator.CreateInstance(prop.PropertyType);

                        prop.SetValue(obj, newEmptyIEnumerable);
                    }
                    catch
                    {
                        BH.Engine.Base.Compute.RecordWarning($"Null property `{prop.Name}` of type `{prop.DeclaringType.FullName}` could not be set to a new empty `{prop.PropertyType.FullName}`.");
                    }
                }
            }
        }
    }
}

