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

using BH.Engine.Base.Objects;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace BH.Engine.Base
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Generates a string representing the whole structure of the object with its assigned values.")]
        [Input("obj", "Objects the string should be calculated for.")]
        [Input("propertyNameExceptions", "(Optional) e.g. `Fragments`. If you want to exclude a property that many objects have.")]
        [Input("propertyFullNameExceptions", "(Optional) e.g. `BH.oM.Structure.Elements.Bar.Fragments`. If you want to exclude a specific property of an object.")]
        [Input("namespaceExceptions", "(Optional) e.g. `BH.oM.Structure`. Any corresponding namespace is ignored.")]
        [Input("typeExceptions", "(Optional) e.g. `typeof(Guid)`. Any corresponding type is ignored.")]
        [Input("maxNesting", "(Optional) e.g. `100`. If any property is nested into the object over that level, it is ignored.")]
        public static string GetHashString(
            this object obj,
            int nestingLevel, //e.g. "Fragments"
            int maxNesting = 100, //e.g. "BH.oM.Structure.Elements.Bar.Fragments"
            List<string> propertyNameExceptions = null, //e.g. "BH.oM.Structure"
            List<string> propertyFullNameExceptions = null, //e.g. typeof(Guid)
            List<string> namespaceExceptions = null,
            List<Type> typeExceptions = null)
        {
            string composedString = "";
            string tabs = new String('\t', nestingLevel);

            Type type = obj?.GetType();

            if (type == null
                || (typeExceptions != null && typeExceptions.Contains(type))
                || (namespaceExceptions != null && namespaceExceptions.Where(ex => type.Namespace.Contains(ex)).Any())
                || nestingLevel >= maxNesting)
            {
                return composedString;
            }
            else if (type.IsPrimitive || type == typeof(Decimal) || type == typeof(String))
            {
                composedString += $"\n{tabs}" + obj?.ToString() ?? "";
            }
            else if (type.IsArray)
            {
                foreach (var element in (obj as dynamic))
                    composedString += $"\n{tabs}" + GetHashString(element, nestingLevel + 1, maxNesting, propertyNameExceptions, propertyFullNameExceptions, namespaceExceptions, typeExceptions);
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                IDictionary dic = obj as IDictionary;
                foreach (DictionaryEntry entry in dic)
                {
                    composedString += $"\n{tabs}" + $"[{entry.Key.GetType().FullName}]\n{tabs}{entry.Key}:\n { GetHashString(entry.Value, nestingLevel + 1, maxNesting, propertyNameExceptions, propertyFullNameExceptions, namespaceExceptions, typeExceptions)}";
                }
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type) || typeof(IList).IsAssignableFrom(type) || typeof(ICollection).IsAssignableFrom(type))
            {
                foreach (var element in (obj as dynamic))
                    composedString += $"\n{tabs}" + GetHashString(element, nestingLevel + 1, maxNesting, propertyNameExceptions, propertyFullNameExceptions, namespaceExceptions, typeExceptions);
            }
            else if (type.FullName.Contains("System.Collections.Generic.ObjectEqualityComparer`1"))
            {
                composedString = "";
            }
            else if (type == typeof(System.Data.DataTable))
            {
                DataTable dt = obj as DataTable;
                return composedString += $"{type.FullName} {string.Join(", ", dt.Columns.OfType<DataColumn>().Select(c => c.ColumnName))}\n{tabs}" + GetHashString(dt.AsEnumerable(), nestingLevel + 1, maxNesting, propertyNameExceptions, propertyFullNameExceptions, namespaceExceptions, typeExceptions);
            }
            else if (typeof(object).IsAssignableFrom(type))
            {
                var allProperties = type
                .GetProperties();

                var properties = allProperties
                    .Where(p => !p.GetIndexParameters().Any()); // skip indexers to avoid error on GetValue() for FragmentSet and the like.

                // Iterate "normal" properties
                foreach (PropertyInfo prop in properties)
                {
                    if (
                       (propertyNameExceptions != null && propertyNameExceptions.Where(ex => prop.Name.Contains(ex)).Any())
                    || (propertyFullNameExceptions != null && propertyFullNameExceptions.Where(ex => $"{type.FullName}.{prop.Name}".Contains(ex)).Any()))
                    {
                        continue;
                    }

                    object propValue = prop.GetValue(obj);
                    if (propValue != null)
                    {
                        string outString = GetHashString(propValue, nestingLevel + 1, maxNesting, propertyNameExceptions, propertyFullNameExceptions, namespaceExceptions, typeExceptions) ?? "";
                        if (!string.IsNullOrWhiteSpace(outString))
                            composedString += $"\n{tabs}" + $"{type.FullName}.{prop.Name}:\n{tabs}{outString} ";
                    }
                }
            }
            else
            {
                composedString = $"\n{tabs}" + obj?.ToString() ?? "";
            }

            if (!string.IsNullOrWhiteSpace(composedString))
                composedString = (nestingLevel > 0 ? "\t" : "") + $"[{type.FullName}]" + composedString;

            return composedString;
        }


    }
}

