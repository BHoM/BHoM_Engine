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
        [Input("storeInFragment", "(Optional, defaults to false) If true, if the object is a BHoMObject, store the hash on it in a HashFragment.")]
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

            // Make sure that "BHoM_Guid" is added to the PropertyExceptions of the config.
            cc.PropertyExceptions = cc.PropertyExceptions ?? new List<string>();
            if (!cc.PropertyExceptions.Contains(nameof(BHoMObject.BHoM_Guid)))
                cc.PropertyExceptions.Add(nameof(BHoMObject.BHoM_Guid));

            // Parse `PropertiesToConsider`. If problems are found, expose all the error messages to the user, then return.
            bool invalidPropertiesToConsider = false;
            for (int i = 0; i < cc.PropertiesToConsider.Count; i++)
            {
                string ptc = cc.PropertiesToConsider[i];

                if (ptc.Contains("*"))
                {
                    BH.Engine.Reflection.Compute.RecordError($"Wildcards * are not supported in `{nameof(BaseComparisonConfig.PropertiesToConsider)}` when computing the Hash of an object. The input `{ptc}` is not valid, please remove it. will not be considered.");
                    invalidPropertiesToConsider = true;
                }

                if (!ptc.StartsWith("BH"))
                {
                    BH.Engine.Reflection.Compute.RecordError($"When computing the Hash using `{nameof(BaseComparisonConfig.PropertiesToConsider)}`, the property name must be provided in its Full Name form. The input `{ptc}` does not comply with this requirement." +
                        $"\nFor example, if you are interested in considering `StartNode.Name` (the name of a Bar's StartNode), you need to specify `BH.oM.Structure.Elements.Bar.StartNode.Name`).");
                    invalidPropertiesToConsider = true;
                }
            }

            if (invalidPropertiesToConsider) return "";

            // Convert from the Numeric Tolerance to fractionalDigits (required for the hash).
            int fractionalDigits = Math.Abs(Convert.ToInt32(Math.Log10(cc.NumericTolerance)));

            // ----- SET UP OF INPUT OBJECT -----

            // Copy the object for immutability
            IObject iObj_copy = iObj.ShallowClone();

            // Any HashFragment present on the object must not be considered when computing the Hash. Remove if present.
            IBHoMObject bhomobj = iObj_copy as IBHoMObject;
            if (bhomobj != null)
            {
                IEnumerable<IHashFragment> hashFragments = bhomobj.GetAllFragments(typeof(IHashFragment)).OfType<IHashFragment>();
                if (hashFragments.Any())
                    hashFragments.ToList().ForEach(f => bhomobj.Fragments.Remove(f.GetType()));
                iObj_copy = bhomobj;
            }

            // ----- HASH -----

            // Compute the defining string.
            string hashString = DefiningString(iObj_copy, cc, fractionalDigits, 0);

            if (string.IsNullOrWhiteSpace(hashString))
            {
                // This means that:
                // - all properties of the input object were disregarded due to the settings specified in the ComparisonConfig, or
                // - all properties of the input object that were not disregarded were null or empty,
                // Since a hash has to be always returned, for this scenario we are forced to build a defining string out of the type full name.
                hashString = iObj_copy.GetType().FullName;
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
        [Input("propertyPath", "(Optional) Indicates the 'property path' of the current object, e.g. `BH.oM.Structure.Elements.Bar.StartNode.Point.X`")]
        private static string DefiningString(object obj, BaseComparisonConfig cc, int fractionalDigits, int nestingLevel, string currentPropertyFullName = null)
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
            else if (type.IsPrimitive || type == typeof(decimal) || type == typeof(String))
            {
                if (type == typeof(double) || type == typeof(decimal) || type == typeof(float))
                {
                    // Take care of fractional digits config option.
                    if (cc.FractionalDigitsPerProperty?.Any() ?? false)
                    {
                        int digitsToLimit = -1;

                        foreach (var kv in cc.FractionalDigitsPerProperty)
                        {
                            if (currentPropertyFullName.Contains(kv.Key) || currentPropertyFullName.WildcardMatch(kv.Key))
                                if (digitsToLimit == -1)
                                    digitsToLimit = kv.Value;
                                else
                                    BH.Engine.Reflection.Compute.RecordError($"Too many matching results obtained with specified {nameof(cc.FractionalDigitsPerProperty)} `{kv.Key}`.");
                        }

                        fractionalDigits = digitsToLimit != -1 ? digitsToLimit : fractionalDigits;
                    }

                    obj = Math.Round(obj as dynamic, fractionalDigits);
                }

                definingString += $"\n{tabs}" + obj?.ToString() ?? "";
            }
            else if (type.IsArray)
            {
                foreach (var element in (obj as dynamic))
                    definingString += $"\n{tabs}" + DefiningString(element, cc, fractionalDigits, nestingLevel + 1, currentPropertyFullName);
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

                    definingString += $"\n{tabs}" + $"[{entry.Key.GetType().FullName}]\n{tabs}{entry.Key}:\n { DefiningString(entry.Value, cc, fractionalDigits, nestingLevel + 1, currentPropertyFullName)}";
                }
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                foreach (var element in (obj as dynamic))
                    definingString += $"\n{tabs}" + DefiningString(element, cc, fractionalDigits, nestingLevel + 1, currentPropertyFullName);
            }
            else if (type.FullName.Contains("System.Collections.Generic.ObjectEqualityComparer`1"))
            {
                definingString = "";
            }
            else if (type == typeof(System.Data.DataTable))
            {
                DataTable dt = obj as DataTable;
                return definingString += $"{type.FullName} {string.Join(", ", dt.Columns.OfType<DataColumn>().Select(c => c.ColumnName))}\n{tabs}" + DefiningString(dt.AsEnumerable(), cc, fractionalDigits, nestingLevel + 1, currentPropertyFullName);
            }
            else if (typeof(IObject).IsAssignableFrom(type))
            {
                // If the object is an IObject (= a BHoM class), let's look at its properties. 
                // We only do this for IObjects (BHoM types) since we cannot guarantee full compatibility of the following procedure with any possible (non-BHoM) type.
                PropertyInfo[] properties = type.GetProperties();

                // Make sure we cover the PropertiesToConsider.
                cc.AddExceptionsFromPropertiesToConsider(currentPropertyFullName, type, nestingLevel);

                // Iterate all properties
                foreach (PropertyInfo prop in properties)
                {
                    string propHashString = "";
                    string propName = prop.Name;
                    string propFullName = $"{currentPropertyFullName}.{propName}";

                    // If the object is an IObject (= a BHoM class), check if there is a `ComparisonInclusion()` extension method available for this IObject.
                    object propCompIncl = null;
                    object[] parameters = new object[] { currentPropertyFullName, cc };
                    if (BH.Engine.Reflection.Compute.TryRunExtensionMethod(obj, "ComparisonInclusion", parameters, out propCompIncl))
                    {
                        // If a ComparisonInclusion() extension method was found, use its result to determine whether this property is to be included or not in the Hash.
                        ComparisonInclusion comparisonInclusion = propCompIncl as ComparisonInclusion;

                        if (!comparisonInclusion.Include)
                            return ""; // do not include this in the Hash.
                    }
                    else
                    {
                        // If no ComparisonInclusion() extension method was found, check the Exceptions to check if this property is to be skipped.
                        if (cc.IsInPropertyExceptions(propFullName))
                            continue;
                    }

                    object propertyValue = prop.GetValue(obj);
                    if (propertyValue != null)
                    {
                        // Recurse for this property.
                        propHashString = DefiningString(propertyValue, cc, fractionalDigits, nestingLevel + 1, propFullName) ?? "";
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

        // Populates the properties exceptions from the properties to consider.
        // To compute the hash, this is the simplest way of covering all possible use cases.
        private static void AddExceptionsFromPropertiesToConsider(this BaseComparisonConfig cc, string currentPropertyFullName, Type currentObjType, int nestingLevel = -1)
        {
            // Null checks
            if (cc == null) return;
            if (!cc?.PropertiesToConsider?.Where(ptc => !string.IsNullOrWhiteSpace(ptc)).Any() ?? true) return;
            if (string.IsNullOrEmpty(currentPropertyFullName)) return;

            // Set nestingLevel if missing
            if (nestingLevel == -1)
                nestingLevel = currentPropertyFullName.Replace(currentObjType.FullName, "").Count(c => c == '.');

            // Get the current object's declared properties full names.
            List<string> allDeclaredPropertyNames = BH.Engine.Reflection.Query.PropertyNames(currentObjType);
            List<string> allDeclaredPropertyFullNames = allDeclaredPropertyNames.Select(pn => $"{currentPropertyFullName}.{pn}").ToList();

            // Get the parent property's full name.
            string[] currentPropertyFullNameComponents = currentPropertyFullName?.Split('.');
            string currentParentPropertyFullName = "";
            if (currentPropertyFullNameComponents != null)
                currentParentPropertyFullName = string.Join(".", currentPropertyFullNameComponents.Take(currentPropertyFullNameComponents.Count() - 1));

            // Make sure that the propertiesToConsider are relative paths to the current property, if they were specified as Full Names.
            List<string> propertiesToConsider = GetRelativePathsForFullNames(cc.PropertiesToConsider, currentPropertyFullNameComponents, nestingLevel);

            // Get the currentLevelPropertiesToConsider, that is those propertiesToConsider that are at the current nesting level. E.g. for top-level propertiesToConsider, they should have no dot `.` in their string.
            List<string> currentLevelPropertiesToConsider = propertiesToConsider.Where(pTc => pTc.Count(c => c == '.') == nestingLevel || pTc.StartsWith("*.")).ToList();

            List<string> subPropsToConsider = propertiesToConsider
                .Where(pTc => pTc.Count(c => c == '.') > nestingLevel && !pTc.StartsWith("*."))
                .Where(ptc => currentParentPropertyFullName.EndsWith(string.Join(".", ptc.Take(nestingLevel))))
                .ToList();

            // Get the declaredPropertiesToConsider, which are the currentLevelPropertiesToConsider for which there is a match among this object's properties.
            List<string> declaredPropertiesToConsider = allDeclaredPropertyFullNames
                .Where(pPath => currentLevelPropertiesToConsider
                .Any(ptc => pPath.EndsWith(ptc) || pPath.WildcardMatch(ptc))).ToList();

            if (declaredPropertiesToConsider.Any())
            {
                // All the remaining declaredProperties are to be added to the Exceptions.
                List<string> declaredPropertiesExceptions = allDeclaredPropertyFullNames.Except(declaredPropertiesToConsider).ToList();
                cc.PropertyExceptions.AddRange(declaredPropertiesExceptions);
            }

            if (subPropsToConsider.Any())
            {
                // If we found sub-properties to consider, we need to make sure to add all declared properties of the "parent type" (= the current object) as exceptions.

                // Find the current object PropertiesToConsider from the subpropertiesToConsider, by splitting their path at dots, and taking the property name at the current nesting level.
                List<string> currentObjectPropertiesToConsiderFromSubProps = subPropsToConsider.Select(ptc => ptc.Split('.').ElementAtOrDefault(nestingLevel)).ToList();

                // If we wrote e.g. *.Name, this allows to keep recursing until we find all properties called `Name`.
                currentObjectPropertiesToConsiderFromSubProps.RemoveAll(s => s == "*" || string.IsNullOrWhiteSpace(s));

                if (currentObjectPropertiesToConsiderFromSubProps.Any())
                {
                    // If we found "parent type" (= current object) properties to consider from the SubPropertiesToConsider, add them to the exceptions.

                    // This is to ensure we record the "property path" form rather than "property name".
                    IEnumerable<string> currentObjectPropertypathsToConsiderFromSubProps = allDeclaredPropertyFullNames.Where(pp => currentObjectPropertiesToConsiderFromSubProps.Any(ptc => pp.EndsWith(ptc)));

                    // Obtain the exceptions and add them.
                    IEnumerable<string> declaredPropertiesExceptions = allDeclaredPropertyFullNames.Except(currentObjectPropertypathsToConsiderFromSubProps);
                    cc.PropertyExceptions.AddRange(declaredPropertiesExceptions);
                }
            }
        }

        private static List<string> GetRelativePathsForFullNames(List<string> propertiesToConsider, string[] currentPropertyFullNameComponents, int nestingLevel)
        {
            List<string> propertiesToConsiderPartialName = new List<string>();
            foreach (var ptc in propertiesToConsider)
            {
                if (ptc.StartsWith("BH"))
                {
                    var ptcComponents = ptc.Split('.');
                    int lastCommonIndex = -1;
                    int commonLength = Math.Min(currentPropertyFullNameComponents.Length, ptcComponents.Length);
                    for (int i = 0; i < commonLength; i++)
                    {
                        if (ptcComponents[i] == currentPropertyFullNameComponents[i])
                            lastCommonIndex = i;
                    }

                    string adjusted = string.Join(".", ptcComponents.Skip(lastCommonIndex + 1 - nestingLevel));
                    
                    propertiesToConsiderPartialName.Add(adjusted);
                }
                else
                    propertiesToConsiderPartialName.Add(ptc);
            }

            return propertiesToConsiderPartialName;
        }
    }
}

