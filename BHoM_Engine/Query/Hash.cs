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
        public static string Hash(this IObject iObj, DistinctConfig distinctConfig = null)
        {
            // Make sure to clone for immutability, and always have a HashConfig.
            DistinctConfig hc = distinctConfig == null ? new DistinctConfig() : distinctConfig.DeepClone();

            // Make sure that "BHoM_Guid" is added to the propertyNameExceptions of the HashConfig.
            hc.PropertyNameExceptions = hc.PropertyNameExceptions ?? new List<string>();
            if (!hc.PropertyNameExceptions.Contains(nameof(BHoMObject.BHoM_Guid)))
                hc.PropertyNameExceptions.Add(nameof(BHoMObject.BHoM_Guid));

            // Convert from the Numeric Tolerance to fractionalDigits (required for the hash).
            int fractionalDigits = Math.Abs(Convert.ToInt32(Math.Log10(hc.NumericTolerance)));

            // Process the "PropertiesToInclude" property.
            if (hc.PropertiesToConsider?.Any() ?? false)
            {
                // The hash computation can only consider "exceptions".
                // We need to retrieve all the object properties, intersect them with PropertiesToInclude, and treat all those remaining as "exceptions".
                IEnumerable<string> exceptions = BH.Engine.Reflection.Query.PropertyNames(iObj).Except(hc.PropertiesToConsider);
                hc.PropertyNameExceptions.AddRange(exceptions);
            }

            // Any HashFragment present on the object must not be considered when computing the Hash. Remove if present.
            IBHoMObject bhomobj = iObj as IBHoMObject;
            if (bhomobj != null)
            {
                bhomobj = BH.Engine.Base.Query.DeepClone(iObj) as IBHoMObject;
                List<IHashFragment> hashFragments = bhomobj.GetAllFragments(typeof(IHashFragment)).OfType<IHashFragment>().ToList();
                hashFragments.ForEach(f => bhomobj.Fragments.Remove(f.GetType()));
            }

            // Compute the defining string.
            string hashString = DefiningString(iObj, hc, fractionalDigits, 0);

            if (string.IsNullOrWhiteSpace(hashString))
                throw new Exception("Error computing the defining string of the object.");

            // Return the SHA256 hash of the defining string.
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
        [Input("hc", "HashConfig, options for the hash calculation.")]
        [Input("nestingLevel", "Nesting level of the property.")]
        [Input("propertyPath", "(Optional) Indicates the 'property path' of the current object, e.g. `BH.oM.Structure.Elements.Bar.StartNode.Point.X`")]
        private static string DefiningString(object obj, DistinctConfig dc, int fractionalDigits, int nestingLevel, string propertyPath = null)
        {
            string composedString = "";
            string tabs = new String('\t', nestingLevel);

            Type type = obj?.GetType();

            if (type == null
                || (dc.TypeExceptions != null && dc.TypeExceptions.Contains(type))
                || (dc.NamespaceExceptions != null && dc.NamespaceExceptions.Where(ex => type.Namespace.Contains(ex)).Any())
                || nestingLevel >= dc.MaxNesting)
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
                    composedString += $"\n{tabs}" + DefiningString(element, dc, fractionalDigits, nestingLevel + 1, propertyPath);
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                IDictionary dic = obj as IDictionary;

                bool isCustomDataDic = propertyPath.EndsWith("CustomData");

                foreach (DictionaryEntry entry in dic)
                {
                    if (isCustomDataDic && dc.CustomdataKeysExceptions.Contains(entry.Key))
                        continue;

                    composedString += $"\n{tabs}" + $"[{entry.Key.GetType().FullName}]\n{tabs}{entry.Key}:\n { DefiningString(entry.Value, dc, fractionalDigits, nestingLevel + 1, propertyPath)}";
                }
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type) || typeof(IList).IsAssignableFrom(type) || typeof(ICollection).IsAssignableFrom(type))
            {
                foreach (var element in (obj as dynamic))
                    composedString += $"\n{tabs}" + DefiningString(element, dc, fractionalDigits, nestingLevel + 1, propertyPath);
            }
            else if (type.FullName.Contains("System.Collections.Generic.ObjectEqualityComparer`1"))
            {
                composedString = "";
            }
            else if (type == typeof(System.Data.DataTable))
            {
                DataTable dt = obj as DataTable;
                return composedString += $"{type.FullName} {string.Join(", ", dt.Columns.OfType<DataColumn>().Select(c => c.ColumnName))}\n{tabs}" + DefiningString(dt.AsEnumerable(), dc, fractionalDigits, nestingLevel + 1, propertyPath);
            }
            else if (typeof(IObject).IsAssignableFrom(type))
            {
                PropertyInfo[] properties = type.GetProperties();

                foreach (PropertyInfo prop in properties)
                {
                    bool isInPropertyNameExceptions = dc.PropertyNameExceptions?.Count > 0 && dc.PropertyNameExceptions.Where(ex => prop.Name.Contains(ex)).Any();
                    bool isInPropertyFullNameExceptions = dc.PropertyFullNameExceptions?.Count > 0 && dc.PropertyFullNameExceptions.Where(ex => new WildcardPattern(ex).IsMatch(prop.Name + "." + prop.DeclaringType.FullName)).Any();

                    if (isInPropertyNameExceptions || isInPropertyFullNameExceptions)
                        continue;

                    if (dc.PropertiesToConsider?.Count() > 0 && !dc.PropertiesToConsider.Contains(prop.Name))
                        continue;

                    object propValue = prop.GetValue(obj);
                    if (propValue != null)
                    {
                        if (string.IsNullOrWhiteSpace(propertyPath))
                            propertyPath = type.FullName + "." + prop.Name;
                        else if (prop.PropertyType.IsClass)
                            propertyPath += "." + prop.Name;

                        string outString = "";

                        if (dc.FractionalDigitsPerProperty != null &&
                            prop.PropertyType == typeof(double) || prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(float))
                        {
                            Dictionary<string, int> matches = new Dictionary<string, int>();

                            string path = propertyPath + "." + prop.Name;

                            foreach (var kv in dc.FractionalDigitsPerProperty)
                            {
                                if (path.Contains(kv.Key) ||
                                new WildcardPattern(kv.Key).IsMatch(path))
                                    matches.Add(kv.Key, kv.Value);
                            }

                            if (matches.Count() > 1)
                                throw new ArgumentException($"Too many matching results obtained with specified {nameof(dc.FractionalDigitsPerProperty)}.");

                            int fracDigits = matches.Count() == 1 ? matches.FirstOrDefault().Value : fractionalDigits;

                            outString = DefiningString(propValue, dc, fracDigits, nestingLevel + 1, path) ?? "";
                        }
                        else
                            outString = DefiningString(propValue, dc, fractionalDigits, nestingLevel + 1, propertyPath) ?? "";

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
