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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using BH.Engine.Serialiser;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.Engine.Base;
using System.Collections;
using System.Data;
using System.Management.Automation;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Computes a Hash code for the iObject. The hash uniquely represents an object's state based on its combined properties and their values.")]
        [Input("iObj", "iObject the hash code should be calculated for.")]
        public static string Hash(this IObject iObj)
        {
            return Hash(iObj, null);
        }

        /***************************************************/

        [Description("Computes a Hash code for the iObject with exceptions and custom options.\n"
            + "The hash uniquely represents an object's state based on the included properties and their values.")]
        [Input("iObj", "iObject the hash code should be calculated for.")]
        [Input("propertyNameExceptions", "(Optional) e.g. `Fragments`. If you want to exclude a property that many objects have.")]
        [Input("propertyFullNameExceptions", "(Optional) e.g. `BH.oM.Structure.Elements.Bar.Fragments`. If you want to exclude a specific property of an object.")]
        [Input("namespaceExceptions", "(Optional) e.g. `BH.oM.Structure`. Any corresponding namespace is ignored.")]
        [Input("typeExceptions", "(Optional) e.g. `typeof(Guid)`. Any corresponding type is ignored.")]
        [Input("maxNesting", "(Optional) e.g. `100`. If any property is nested into the object over that level, it is ignored.")]
        [Input("fractionalDigits", "(Optional) Defaults to 12. Number of digits retained after the comma; applies rounding.")]
        [Input("fractionalDigitsPerProperty", "(Optional) e.g. `{ { StartNode.Point.X, 2 } }`. If a property name matches this, applies a rounding to the corresponding number of digits specified.\nSupports * wildcard.")]
        public static string Hash(
            this IObject iObj,
            List<string> propertyNameExceptions = null, //e.g. `BHoM_Guid`
            List<string> propertyFullNameExceptions = null, //e.g. `BH.oM.Structure.Elements.Bar.Fragments` – can use * wildcard here.
            List<string> namespaceExceptions = null, //e.g. `BH.oM.Structure`
            List<Type> typeExceptions = null, //e.g. `typeof(Guid)`
            int maxNesting = 100,
            int fractionalDigits = 12,
            Dictionary<string, int> fractionalDigitsPerProperty = null // e.g. { { StartNode.Point.X, 2 } } – can use * wildcard here.
            )
        {
            // Make sure that "BHoM_Guid" is added to the propertyNameExceptions.
            propertyNameExceptions = propertyNameExceptions ?? new List<string>();
            if (!propertyNameExceptions.Contains(nameof(BHoMObject.BHoM_Guid)))
                propertyNameExceptions.Add(nameof(BHoMObject.BHoM_Guid));

            string hashString = DefiningString(iObj, 0, maxNesting, propertyNameExceptions, propertyFullNameExceptions, namespaceExceptions, typeExceptions, fractionalDigits, fractionalDigitsPerProperty);

            return SHA256Hash(hashString);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        // Computes a SHA 256 hash from the given string.
        private static string SHA256Hash(string str)
        {
            byte[] strBytes = ASCIIEncoding.Default.GetBytes(str);

            HashAlgorithm SHA256Algorithm = SHA256.Create();
            byte[] byteHash = SHA256Algorithm.ComputeHash(strBytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteHash)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        /***************************************************/

        [Description("Generates a string representing the whole structure of the object with its assigned values.")]
        [Input("obj", "Objects the string should be calculated for.")]
        [Input("nestingLevel", "Nesting level of the property.")]
        [Input("maxNesting", "(Optional) e.g. `100`. If any property is nested into the object over that level, it is ignored.")]
        [Input("propertyNameExceptions", "(Optional) e.g. `Fragments`. If you want to exclude a property that many objects have.")]
        [Input("propertyFullNameExceptions", "(Optional) e.g. `BH.oM.Structure.Elements.Bar.Fragments`. If you want to exclude a specific property of an object.")]
        [Input("namespaceExceptions", "(Optional) e.g. `BH.oM.Structure`. Any corresponding namespace is ignored.")]
        [Input("typeExceptions", "(Optional) e.g. `typeof(Guid)`. Any corresponding type is ignored.")]
        [Input("fractionalDigits", "(Optional) Defaults to 12. Number of digits retained after the comma; applies rounding.")]
        [Input("fractionalDigitsPerProperty", "(Optional) e.g. `{ { StartNode.Point.X, 2 } }`. If a property name matches this, applies a rounding to the corresponding number of digits specified.")]
        [Input("propertyPath", "(Optional) Indicates the 'property path' of the current object, e.g. `BH.oM.Structure.Elements.Bar.StartNode.Point.X`")]
        private static string DefiningString(
            object obj,
            int nestingLevel,
            int maxNesting = 100,
            List<string> propertyNameExceptions = null, //e.g. "BHoM_Guid"
            List<string> propertyFullNameExceptions = null, //e.g. "BH.oM.Structure.Elements.Bar.Fragments"
            List<string> namespaceExceptions = null, //e.g. "BH.oM.Structure"
            List<Type> typeExceptions = null, //e.g. typeof(Guid)
            int fractionalDigits = 12,
            Dictionary<string, int> fractionalDigitsPerProperty = null, // e.g. { { StartNode.Point.X, 2 } } – can use * wildcard here.
            string propertyPath = null) // Indicates the "property path" of the current object, e.g. BH.oM.Structure.Elements.Bar.StartNode.Point.X
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
            else if (type.IsPrimitive || type == typeof(decimal) || type == typeof(String))
            {
                if (type == typeof(double) || type == typeof(decimal) || type == typeof(float))
                    obj = Math.Round(obj as dynamic, fractionalDigits);

                composedString += $"\n{tabs}" + obj?.ToString() ?? "";
            }
            else if (type.IsArray)
            {
                foreach (var element in (obj as dynamic))
                    composedString += $"\n{tabs}" + DefiningString(element, nestingLevel + 1, maxNesting, propertyNameExceptions, propertyFullNameExceptions, namespaceExceptions, typeExceptions, fractionalDigits, fractionalDigitsPerProperty, propertyPath);
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                IDictionary dic = obj as IDictionary;
                foreach (DictionaryEntry entry in dic)
                {
                    composedString += $"\n{tabs}" + $"[{entry.Key.GetType().FullName}]\n{tabs}{entry.Key}:\n { DefiningString(entry.Value, nestingLevel + 1, maxNesting, propertyNameExceptions, propertyFullNameExceptions, namespaceExceptions, typeExceptions, fractionalDigits, fractionalDigitsPerProperty, propertyPath)}";
                }
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type) || typeof(IList).IsAssignableFrom(type) || typeof(ICollection).IsAssignableFrom(type))
            {
                foreach (var element in (obj as dynamic))
                    composedString += $"\n{tabs}" + DefiningString(element, nestingLevel + 1, maxNesting, propertyNameExceptions, propertyFullNameExceptions, namespaceExceptions, typeExceptions, fractionalDigits, fractionalDigitsPerProperty, propertyPath);
            }
            else if (type.FullName.Contains("System.Collections.Generic.ObjectEqualityComparer`1"))
            {
                composedString = "";
            }
            else if (type == typeof(System.Data.DataTable))
            {
                DataTable dt = obj as DataTable;
                return composedString += $"{type.FullName} {string.Join(", ", dt.Columns.OfType<DataColumn>().Select(c => c.ColumnName))}\n{tabs}" + DefiningString(dt.AsEnumerable(), nestingLevel + 1, maxNesting, propertyNameExceptions, propertyFullNameExceptions, namespaceExceptions, typeExceptions, fractionalDigits, fractionalDigitsPerProperty, propertyPath);
            }
            else if (typeof(IObject).IsAssignableFrom(type))
            {
                PropertyInfo[] properties = type.GetProperties();

                foreach (PropertyInfo prop in properties)
                {
                    bool isInPropertyNameExceptions = propertyNameExceptions != null && propertyNameExceptions.Where(ex => prop.Name.Contains(ex)).Any();
                    bool isInPropertyFullNameExceptions = propertyFullNameExceptions != null && propertyFullNameExceptions.Where(ex => new WildcardPattern(ex).IsMatch(prop.Name + "." + prop.DeclaringType.FullName)).Any();

                    if (isInPropertyNameExceptions || isInPropertyFullNameExceptions)
                        continue;

                    object propValue = prop.GetValue(obj);
                    if (propValue != null)
                    {
                        if (string.IsNullOrWhiteSpace(propertyPath))
                            propertyPath = type.FullName + "." + prop.Name;
                        else if (prop.PropertyType.IsClass)
                            propertyPath += "." + prop.Name;

                        string outString = "";

                        if (fractionalDigitsPerProperty != null &&
                            prop.PropertyType == typeof(double) || prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(float))
                        {
                            Dictionary<string, int> matches = new Dictionary<string, int>();

                            string path = propertyPath + "." + prop.Name;

                            foreach (var kv in fractionalDigitsPerProperty)
                            {
                                if (path.Contains(kv.Key) ||
                                new WildcardPattern(kv.Key).IsMatch(path))
                                    matches.Add(kv.Key, kv.Value);
                            }

                            if (matches.Count() > 1)
                                throw new ArgumentException($"Too many matching results obtained with specified {nameof(fractionalDigitsPerProperty)}.");

                            int fracDigits = matches.Count() == 1 ? matches.FirstOrDefault().Value : fractionalDigits;

                            outString = DefiningString(propValue, nestingLevel + 1, maxNesting, propertyNameExceptions, propertyFullNameExceptions, namespaceExceptions, typeExceptions, fracDigits, fractionalDigitsPerProperty, path) ?? "";
                        }
                        else
                            outString = DefiningString(propValue, nestingLevel + 1, maxNesting, propertyNameExceptions, propertyFullNameExceptions, namespaceExceptions, typeExceptions, fractionalDigits, fractionalDigitsPerProperty, propertyPath) ?? "";

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

        /***************************************************/
    }
}
