/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
using BH.Engine.Reflection;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Computes a Hash code for the iObject. The hash uniquely represents an object's state, based on its properties and their values. It can be used for comparisons." +
            "\nYou can change how the hash is computed by changing the settings in the ComparisonConfig.")]
        [Input("iObj", "iObject the hash code should be calculated for.")]
        [Input("comparisonConfig", "Configure how the hash is computed.")]
        [Input("hashFromFragment", "If true, if the object is a BHoMObject that owns a HashFragment, retrieve the hash from it instead of computing the hash.")]
        public static string Hash(this IObject iObj, BaseComparisonConfig comparisonConfig = null, bool hashFromFragment = false)
        {
            if (iObj == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the hash of a null object.");
                return "";
            }

            if (hashFromFragment && iObj is IBHoMObject)
            {
                // Instead of computing the Hash, first tryGet the hash in HashFragment
                string fragmentHash = (iObj as IBHoMObject).FindFragment<HashFragment>()?.Hash;

                if (!string.IsNullOrWhiteSpace(fragmentHash))
                    return fragmentHash;
            }

            // ------ SET UP OF CONFIGURATION ------

            // Make sure we always have a config object. Clone for immutability.
            BaseComparisonConfig cc = comparisonConfig == null ? new ComparisonConfig() : comparisonConfig.DeepClone();
            cc.TypeExceptions.Add(typeof(HashFragment));

            // Parse the ComparisonConfig's `PropertiesToConsider` and `PropertyExceptions` and get them all as Full Names.
            Modify.PropertyNamesToFullNames(cc, iObj.GetType());

            // ----- HASH -----

            // Compute the defining string.
            string hashString = HashString(iObj, cc, 0);

            if (string.IsNullOrWhiteSpace(hashString))
            {
                // This means that:
                // - all properties of the input object were disregarded due to the settings specified in the ComparisonConfig, or
                // - all properties of the input object that were not disregarded were null or empty,
                // Since a hash has to be always returned, for this scenario we are forced to build a defining string out of the type full name.
                hashString = iObj.GetType().FullName;
            }

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
        [Input("cc", "HashConfig, options for the hash calculation.")]
        [Input("nestingLevel", "Nesting level of the property.")]
        [Input("currentPropertyFullName", "(Optional) Indicates the 'property path' of the current object, e.g. `BH.oM.Structure.Elements.Bar.StartNode.Point.X`")]
        private static string HashString(object obj, BaseComparisonConfig cc, int nestingLevel, string currentPropertyFullName = null)
        {
            string definingString = "";

            // Get the considered object type.
            Type type = obj?.GetType();

            // If the currentPropertyFullName is empty, it means we are at the top level of the object. We can consider the object type name as the currentPropertyFullName.
            currentPropertyFullName = string.IsNullOrWhiteSpace(currentPropertyFullName) ? type.FullName : currentPropertyFullName;

            // Determine the number of tabs that should precede the current property's definingString. Useful for visualizing the definingString e.g. in Notepad.
            string tabs = new String('\t', nestingLevel);

            // -------------------------------------------------------- // 
            // Compute the definingString depending on the object type. //
            // -------------------------------------------------------- // 


            if (type == null
                || (cc.TypeExceptions != null && cc.TypeExceptions.Any(te => te.IsAssignableFrom(type)))
                || (cc.NamespaceExceptions != null && cc.NamespaceExceptions.Where(ex => type.Namespace.Contains(ex)).Any())
                || nestingLevel >= cc.MaxNesting)
            {
                return "";
            }
            else if (type.IsNumeric())
            {
                // If we didn't specify any custom tolerance/significant figures, just return the input.
                if (cc.NumericTolerance == double.MinValue && cc.SignificantFigures == int.MaxValue
                    && (!cc.PropertyNumericTolerances?.Any() ?? true) && (!cc.PropertySignificantFigures?.Any() ?? true))
                    return $"\n{tabs}" + obj.ToString();

                if (type == typeof(double))
                    return $"\n{tabs}" + NumericalApproximation((double)obj, currentPropertyFullName, cc).ToString();

                if (type == typeof(int))
                    return $"\n{tabs}" + NumericalApproximation((int)obj, currentPropertyFullName, cc).ToString();

                // Fallback for any other floating-point numeric type.
                if (type.IsFloatingPointNumericType())
                    return $"\n{tabs}" + NumericalApproximation(double.Parse(obj.ToString()), currentPropertyFullName, cc).ToString();

                // Fallback for any other integral numeric type.
                if (type.IsIntegralNumericType())
                    return $"\n{tabs}" + NumericalApproximation(double.Parse(obj.ToString()), currentPropertyFullName, cc).ToString();

            }
            else if (type.IsPrimitive || type == typeof(String))
            {
                definingString += $"\n{tabs}" + obj?.ToString() ?? "";
            }
            else if (type.IsArray)
            {
                foreach (var element in (obj as dynamic))
                    definingString += $"\n{tabs}" + HashString(element, cc, nestingLevel + 1, currentPropertyFullName);
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                IDictionary dic = obj as IDictionary;

                bool isCustomDataDic = currentPropertyFullName.EndsWith("CustomData") && obj is Dictionary<string, object>;

                foreach (DictionaryEntry entry in dic)
                {
                    if (isCustomDataDic)
                    {
                        // If the CustomDataKey is among the exceptions, skip it.
                        if (cc.CustomdataKeysExceptions?.Contains(entry.Key) ?? false)
                            continue;

                        // If there are CustomdataKeysToInclude specified and CustomDataKey is not among them, skip it.
                        if ((cc.CustomdataKeysToInclude?.Any() ?? false) && !cc.CustomdataKeysToInclude.Contains(entry.Key))
                            continue;
                    }

                    definingString += $"\n{tabs}" + $"[{entry.Key.GetType().FullName}]\n{tabs}{entry.Key}:\n { HashString(entry.Value, cc, nestingLevel + 1, currentPropertyFullName)}";
                }
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                foreach (var element in (obj as dynamic))
                    definingString += $"\n{tabs}" + HashString(element, cc, nestingLevel + 1, currentPropertyFullName);
            }
            else if (type.FullName.Contains("System.Collections.Generic.ObjectEqualityComparer`1"))
            {
                definingString = "";
            }
            else if (type == typeof(System.Data.DataTable))
            {
                DataTable dt = obj as DataTable;
                return definingString += $"{type.FullName} {string.Join(", ", dt.Columns.OfType<DataColumn>().Select(c => c.ColumnName))}\n{tabs}" + HashString(dt.AsEnumerable(), cc, nestingLevel + 1, currentPropertyFullName);
            }
            else if (typeof(IObject).IsAssignableFrom(type))
            {
                // If the object is an IObject (= a BHoM class), first check if there is a `HashString()` extension method available for this IObject.
                object hashStringFromExtensionMethod = null;
                object[] parameters = new object[] { currentPropertyFullName, cc };
                if (BH.Engine.Reflection.Compute.TryRunExtensionMethod(obj, "HashString", parameters, out hashStringFromExtensionMethod))
                    return (string)hashStringFromExtensionMethod;

                // If the object is an IObject (= a BHoM class), let's look at its properties. 
                // We only do this for IObjects (BHoM types) since we cannot guarantee full compatibility of the following procedure with any possible (non-BHoM) type.
                PropertyInfo[] properties = type.GetProperties();

                // Iterate all properties
                foreach (PropertyInfo prop in properties)
                {
                    string propHashString = "";
                    string propName = prop.Name;
                    string propFullName = $"{currentPropertyFullName}.{propName}";

                    // Check the propertyExceptions/propertiesToConsider in the ComparisonConfig..

                    // Skip if the property is among the PropertyExceptions, or if it's a BHoM_Guid.
                    if ((cc.PropertyExceptions?.Contains(propFullName) ?? false) || propName == "BHoM_Guid")
                        continue;

                    // If the PropertiesToConsider contains at least a value, ensure that this property is "compatible" with at least one of them.
                    // Compatible means to check not only that the current propFullName is among the propertiesToInclude;
                    // we need to consider this propFullName ALSO IF there is at least one PropertyToInclude that specifies a property that is a child of the current propFullName.
                    if ((cc.PropertiesToConsider?.Any() ?? false) &&
                        !cc.PropertiesToConsider.Any(ptc => ptc.StartsWith(propFullName) || propFullName.StartsWith(ptc))) // we want to make sure that we do not exclude sub-properties to include, hence the OR condition.
                        continue;

                    object propertyValue = prop.GetValue(obj);
                    if (propertyValue != null)
                    {
                        // Recurse for this property.
                        propHashString = HashString(propertyValue, cc, nestingLevel + 1, propFullName) ?? "";
                        if (!string.IsNullOrWhiteSpace(propHashString))
                            definingString += $"\n{tabs}" + $"{type.FullName}.{propName}:\n{tabs}{propHashString} ";
                    }
                }
            }
            else
            {
                definingString = $"\n{tabs}" + obj?.ToString() ?? "";
            }

            if (!string.IsNullOrWhiteSpace(definingString))
                definingString = (nestingLevel > 0 ? "\t" : "") + $"[{type.FullName}]" + definingString;

            return definingString;
        }

        /***************************************************/
    }
}

