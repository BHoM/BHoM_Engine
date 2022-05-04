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
using BH.Engine.Base;

namespace BH.Engine.Reflection
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns a deepclone of the object, whose IEnumerable properties are replaced with new IEnumerables of the correct type if they are null." +
            "\nThe method uses the default parameterless constructor of the property type's IEnumerable," +
            "so if a property's IEnumerable type requires any input parameter, this method will not set that property.")]
        [Input("obj", "Object whose properties that are null and whose type is a subtype of IEnumerable will be replaced with a new empty IEnumerable of the correct type.")]
        [Input("warningForUnset", "(Optional, defaults to true) If false, the method does not return a warning when a certain property could not be set.")]
        [Output("obj", "Object with the null IEnumerable properties replaced with empty IEnumerables of the correct type.")]
        public static T SetNewEmptyIEnumPropsIfNull<T>(this T obj, bool warningForUnset = true) where T : class
        {
            if (obj == null)
                return null;

            T deepClone = obj.DeepClone();

            var props = deepClone.GetType().GetProperties();
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

                        prop.SetValue(deepClone, newEmptyIEnumerable);
                    }
                    catch
                    {
                        if (warningForUnset)
                            BH.Engine.Base.Compute.RecordWarning($"Null property `{prop.Name}` of type `{prop.DeclaringType.FullName}` could not be set to a new empty `{prop.PropertyType.FullName}`.");
                    }
                }
            }

            return deepClone;
        }

        /***************************************************/

        [Description("Returns a deepclone of the object, whose IEnumerable properties are replaced with new IEnumerables of the correct type if they are null." +
            "\nThe method uses the default parameterless constructor of the property type's IEnumerable," +
            "so if a property's IEnumerable type requires any input parameter, this method will not set that property.")]
        [Input("obj", "Object whose properties that are null and whose type is a subtype of IEnumerable will be replaced with a new empty IEnumerable of the correct type.")]
        [Input("warningForUnset", "(Optional, defaults to true) If false, the method does not return a warning when a certain property could not be set.")]
        [Output("obj", "Object with the null IEnumerable properties replaced with empty IEnumerables of the correct type.")]
        public static object SetNewEmptyIEnumPropsIfNull(this object obj, bool warningForUnset = true)
        {
            return SetNewEmptyIEnumPropsIfNull<object>(obj, warningForUnset);
        }
    }
}
