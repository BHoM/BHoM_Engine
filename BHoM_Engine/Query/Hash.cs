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
using System.Management.Automation;

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
        [Input("hashFromFragment", "If true, if the object is a BHoMObject storing a HashFragment, retrieve the hash from it instead of computing the hash.")]
        public static string Hash(this IObject iObj, ComparisonConfig comparisonConfig = null, bool hashFromFragment = false)
        {
            if (iObj == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the hash of a null object.");
                return "";
            }

            if (hashFromFragment && iObj is IBHoMObject)
            {
                // Instead of computing the Hash, first tryGet the hash in HashFragment
                string hash = (iObj as IBHoMObject).FindFragment<HashFragment>()?.Hash;

                if (!string.IsNullOrWhiteSpace(hash))
                    return hash;
            }

            // ------ SET UP OF CONFIGURATION ------

            // Make sure we always have a config object. Clone for immutability.
            ComparisonConfig cc = comparisonConfig == null ? new ComparisonConfig() : comparisonConfig.DeepClone();

            // Make sure that "BHoM_Guid" is added to the PropertyExceptions of the config.
            cc.PropertyExceptions = cc.PropertyExceptions ?? new List<string>();
            if (!cc.PropertyExceptions.Contains(nameof(BHoMObject.BHoM_Guid)))
                cc.PropertyExceptions.Add(nameof(BHoMObject.BHoM_Guid));

            // Make sure that the single Property exceptions are either:
            // - explicitly referring to a property in its "property path": e.g. Bar.StartNode.Point.X
            // - OR if it's only a property name e.g. BHoM_Guid make sure that we prepend the wildcard so we can match the single property inside any property path: e.g. *BHoM_Guid
            //cc.PropertyExceptions = cc.PropertyExceptions.Select(pe => pe = pe.Contains('.') ? pe : "*" + pe).ToList();

            // Convert from the Numeric Tolerance to fractionalDigits (required for the hash).
            int fractionalDigits = Math.Abs(Convert.ToInt32(Math.Log10(cc.NumericTolerance)));

            // ----- SET UP OF INPUT OBJECT -----

            // Copy the object for immutability
            IObject iObj_copy = iObj.ShallowClone();

            // Any HashFragment present on the object must not be considered when computing the Hash. Remove if present.
            IBHoMObject bhomobj = iObj_copy as IBHoMObject;
            if (bhomobj != null)
            {
                List<IHashFragment> hashFragments = bhomobj.GetAllFragments(typeof(IHashFragment)).OfType<IHashFragment>().ToList();
                hashFragments.ForEach(f => bhomobj.Fragments.Remove(f.GetType()));
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
        private static string DefiningString(object obj, ComparisonConfig cc, int fractionalDigits, int nestingLevel, string currentPropertyPath = null)
        {
            string composedString = "";
            string tabs = new String('\t', nestingLevel);

            Type type = obj?.GetType();

            currentPropertyPath = currentPropertyPath.IsNullOrEmpty() ? type.FullName : currentPropertyPath;
            List<string> currentPropertyPathComponents = currentPropertyPath?.Split('.').ToList();
            string currentParentTypePath = "";
            string currentPropertyName = "";
            if (currentPropertyPathComponents != null)
            {
                currentParentTypePath = string.Join(".", currentPropertyPathComponents.Take(currentPropertyPathComponents.Count - 1));
                currentPropertyName = currentPropertyPathComponents?.Last();
            }


            if (type == null
                || (cc.TypeExceptions != null && cc.TypeExceptions.Contains(type))
                || (cc.NamespaceExceptions != null && cc.NamespaceExceptions.Where(ex => type.Namespace.Contains(ex)).Any())
                || nestingLevel >= cc.MaxNesting)
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
                    composedString += $"\n{tabs}" + DefiningString(element, cc, fractionalDigits, nestingLevel + 1, currentPropertyPath);
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                IDictionary dic = obj as IDictionary;

                bool isCustomDataDic = currentPropertyPath.EndsWith("CustomData") && obj is Dictionary<string,object>;

                List<string> customDataKeysToExcludeOnThisObject = new List<string>();

                if (isCustomDataDic && cc.CustomdataKeysToInclude.Any())
                {
                    Dictionary<string, object> customDataDict = obj as Dictionary<string, object>;
                    List<string> allCustomDataKeys = customDataDict.Keys.ToList();

                    var allCustomDataExceptToInclude = allCustomDataKeys.Except(cc.CustomdataKeysToInclude).ToList();

                    bool customDataToIncludeFound = allCustomDataExceptToInclude.Count() < allCustomDataKeys.Count();

                    if (customDataToIncludeFound)
                        customDataKeysToExcludeOnThisObject.AddRange(allCustomDataExceptToInclude);
                }

                foreach (DictionaryEntry entry in dic)
                {
                    if (isCustomDataDic && ((cc.CustomdataKeysExceptions.Contains(entry.Key)) || customDataKeysToExcludeOnThisObject.Contains(entry.Key)))
                        continue;

                    composedString += $"\n{tabs}" + $"[{entry.Key.GetType().FullName}]\n{tabs}{entry.Key}:\n { DefiningString(entry.Value, cc, fractionalDigits, nestingLevel + 1, currentPropertyPath)}";
                }
            }
            else if (typeof(IEnumerable).IsAssignableFrom(type) || typeof(IList).IsAssignableFrom(type) || typeof(ICollection).IsAssignableFrom(type))
            {
                foreach (var element in (obj as dynamic))
                    composedString += $"\n{tabs}" + DefiningString(element, cc, fractionalDigits, nestingLevel + 1, currentPropertyPath);
            }
            else if (type.FullName.Contains("System.Collections.Generic.ObjectEqualityComparer`1"))
            {
                composedString = "";
            }
            else if (type == typeof(System.Data.DataTable))
            {
                DataTable dt = obj as DataTable;
                return composedString += $"{type.FullName} {string.Join(", ", dt.Columns.OfType<DataColumn>().Select(c => c.ColumnName))}\n{tabs}" + DefiningString(dt.AsEnumerable(), cc, fractionalDigits, nestingLevel + 1, currentPropertyPath);
            }
            else if (typeof(IObject).IsAssignableFrom(type))
            {
                // If it's an IObject (= a BHoM class), let's look at its properties. 
                // We only do this for IObjects (BHoM types) since we cannot guarantee full compatibility of the following procedure with any possible type.

                PropertyInfo[] properties = type.GetProperties();
                List<string> allDeclaredPropertyNames = BH.Engine.Reflection.Query.PropertyNames(type);
                List<string> allDeclaredPropertyPaths = allDeclaredPropertyNames.Select(pn => $"{currentPropertyPath}.{pn}").ToList();

                if (cc.PropertiesToConsider?.Any() ?? false)
                {
                    // The user specified some PropertiesToConsider.

                    // Get the currentLevelPropertiesToConsider, that is those propertiesToConsider that are at the current nesting level. E.g. for top-level propertiesToConsider, they should have no dot `.` in their string.
                    List<string> currentLevelPropertiesToConsider = cc.PropertiesToConsider.Where(pTc => pTc.Count(c => c == '.') == nestingLevel).ToList();

                    //List<string> currentLevelPropertiesExceptions = cc.PropertiesToConsider.Where(ptc => ptc.Contains('.')).Select(ptc => )

                    // Get the declaredPropertiesToConsider, which are the currentLevelPropertiesToConsider for which there is a match among this object's properties.
                    List<string> declaredPropertiesToConsider = allDeclaredPropertyPaths.Where(pPath => currentLevelPropertiesToConsider.Any(ptc => pPath.EndsWith(ptc))).ToList();

                    if (declaredPropertiesToConsider.Any())
                    {
                        // All the remaining declaredProperties are to be added to the Exceptions.
                        List<string> declaredPropertiesExceptions = allDeclaredPropertyPaths.Except(declaredPropertiesToConsider).ToList();
                        cc.PropertyExceptions.AddRange(declaredPropertiesExceptions);
                    }


                    //bool propertiesToConsiderFoundInCustomData = false;

                    //if (currentLevelPropertiesToConsider.Any() && !declaredPropertiesToConsider.Any())
                    //{
                        // We know that the user specified some currentLevelPropertiesToConsider, but no corresponding declaredPropertiesToConsider were found for this object.
                        // (a) Look in customdata for a key named like that.
                        //IBHoMObject bHoMObject = obj as IBHoMObject;
                        //if (bHoMObject != null)
                        //{
                        //    List<KeyValuePair<string, object>> customDataToInclude = bHoMObject.CustomData.Where(kv => declaredPropertiesToConsider.Any(dptc => dptc.EndsWith(kv.Key))).ToList();
                        //    List<KeyValuePair<string, object>> customDataToExclude = bHoMObject.CustomData.Except(customDataToInclude).ToList();

                        //    foreach (var kv in bHoMObject.CustomData)
                        //    {
                        //        string customDataKeyPath = $"{currentPropertyPath}.CustomData[{kv.Key}]";
                        //        bool isInCustomDataExceptions = cc.PropertyExceptions.Any(pe => pe.EndsWith(customDataKeyPath) || pe.EndsWith(kv.Key));
                        //        bool isCustomDataToBeConsidered = (declaredPropertiesToConsider.Any(dptc => dptc.EndsWith(kv.Key) || dptc.EndsWith(customDataKeyPath)));

                        //        if (!isInCustomDataExceptions && isCustomDataToBeConsidered)
                        //        {
                        //            propertiesToConsiderFoundInCustomData = true;

                        //            string customDataDefiningString = DefiningString(kv.Value, cc, fractionalDigits, nestingLevel + 1, customDataKeyPath);
                        //            if (!string.IsNullOrWhiteSpace(composedString))
                        //                composedString += $"\n{tabs}" + $"{type.FullName}.{customDataDefiningString}:\n{tabs}{customDataDefiningString} ";
                        //        }
                        //        else
                        //            cc.PropertyExceptions.Add(customDataKeyPath);
                        //    }
          

                        //    if (propertiesToConsiderFoundInCustomData)
                        //    {
                        //        // Add all declaredProperties to the exceptions.
                        //        List<string> declaredPropertiesExceptions = allPropertyPaths.Except(declaredPropertiesToConsider).ToList();
                        //        cc.PropertyExceptions.AddRange(declaredPropertiesExceptions);
                        //    }
                        //}

                        // (b) (not currently doable) there is an extension method named like a propertyToConsider that we can run onto the object to get a value to hash.
                        //     This is currently not doable because RunExtensionMethod sits in reflection.
                        // (c) If we formalize some other concept like a Fragment that should own KeyValue pairs, this is the place to look for it.
                    //}

                }

                // Iterate all properties
                foreach (PropertyInfo prop in properties)
                {
                    string propertyHashString = "";
                    string propertyName = prop.Name;
                    string propertyPath = $"{currentPropertyPath}.{propertyName}";

                    bool isInPropertyExceptions = cc.PropertyExceptions.Any(ex => propertyPath.EndsWith(ex)) || cc.PropertyExceptions.Any(ex => currentPropertyPath.EndsWith(ex));
                    bool isInPropertiesToConsider = cc.PropertiesToConsider.Any(pTc => propertyPath.EndsWith(pTc));

                    if (isInPropertyExceptions)
                        continue;

                    //// Process the "PropertiesToConsider" config option.
                    // if (cc.PropertiesToConsider?.Any() ?? false)
                    // {
                    //    // The hash computation can only consider "exceptions".
                    //    // We need to retrieve all the type's properties, intersect them with PropertiesToInclude, and treat all those remaining as "exceptions".
                    //    // Works only for top-level properties.



                    //    if (isInPropertiesToConsider)
                    //    {
                    //        // Some propertyToInclude matches were found for this object. 
                    //        // Add all other properties of this object to propertyExceptions (do not consider them).

                    //        // This ensures that all the exceptions are noted as "property paths" e.g. if `StartNode` was specified, this returns `BH.oM.Structure.Elements.Bar.StartNode`
                    //        List<string> exceptionsPropertyPaths = allPropertyPaths.Where(pp => propertiesToConsider.Any(pi => !pp.EndsWith(pi))).ToList();

                    //        // Add them to the exceptions.
                    //        cc.PropertyExceptions.AddRange(exceptionsPropertyPaths);
                    //    }
                    //    else
                    //    {
                    //        // The PropertiesToConsider includes only property names that were not found on this type.
                    //        // We could:
                    //        // (a) Iterate the PropertiesToConsider and look for extension method(s) called like that for this object. If so, recurse for each of them.
                    //        //     This is currently not doable because RunExtensionMethod sits in reflection.
                    //        // (b) Check if the currently iterated property (prop) is a "INamedValue" object. If so, iterate the PropertiesToConsider to check if the INamedValue.Name == propertyToConsider.
                    //        // (c) If all the above failed, assume that the PropertiesToConsider may be names of some of this object's sub-property. Do not add them to the PropertyExceptions, do nothing.
                    //    }
                    //}



                    object propertyValue = prop.GetValue(obj);
                    if (propertyValue != null)
                    {
                        // Take care of fractional digits config option.
                        int propertyFracionalDigits = fractionalDigits;
                        if (cc.FractionalDigitsPerProperty != null &&
                            prop.PropertyType == typeof(double) || prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(float))
                        {
                            Dictionary<string, int> matches = new Dictionary<string, int>();

                            foreach (var kv in cc.FractionalDigitsPerProperty)
                            {
                                if (propertyPath.Contains(kv.Key) ||
                                new WildcardPattern(kv.Key).IsMatch(propertyPath))
                                    matches.Add(kv.Key, kv.Value);
                            }

                            if (matches.Count() > 1)
                                throw new ArgumentException($"Too many matching results obtained with specified {nameof(cc.FractionalDigitsPerProperty)}.");

                            propertyFracionalDigits = matches.Count() == 1 ? matches.FirstOrDefault().Value : fractionalDigits;
                        }

                        // Recurse for this property.
                        propertyHashString = DefiningString(propertyValue, cc, propertyFracionalDigits, nestingLevel + 1, propertyPath) ?? "";
                        if (!string.IsNullOrWhiteSpace(propertyHashString))
                            composedString += $"\n{tabs}" + $"{type.FullName}.{propertyName}:\n{tabs}{propertyHashString} ";
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

