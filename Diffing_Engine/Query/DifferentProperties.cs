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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;
using BH.oM.Diffing;
using BH.oM.Base;
using KellermanSoftware.CompareNetObjects;
using System.Reflection;
using BH.Engine.Base;

namespace BH.Engine.Diffing
{
    public static partial class Query
    {
        [Description("Checks two BHoMObjects property by property and returns the differences")]
        [Input("DiffingConfig", "Config to be used for the comparison. Can set numeric tolerance, wheter to check the guid, if custom data should be ignored and if any additional properties should be ignored")]
        [Output("Dictionary whose key is the name of the property, and value is a tuple with the different values found in obj1 and obj2.\n" +
            "This dictionary can be 'exploded' in the UI by using `ListDifferentProperties` method.")]
        public static Dictionary<string, Tuple<object, object>> DifferentProperties(this object obj1, object obj2, DiffingConfig diffingConfig = null)
        {
            bool differentTypes = false;
            Type commonType = obj1.GetType();
            if (commonType != obj2.GetType())
            {
                differentTypes = true;
                commonType = BH.Engine.Reflection.Query.CommonBaseType(new List<Type>() { obj1.GetType(), obj2.GetType() });
            }

            // Set configurations if DiffingConfig is null. Clone it for immutability in the UI.
            DiffingConfig dc = diffingConfig == null ? new DiffingConfig() : diffingConfig.DeepClone() as DiffingConfig;

            object obj1Copy = obj1.DeepClone();
            object obj2Copy = obj2.DeepClone();

            var dict = new Dictionary<string, Tuple<object, object>>();

            CompareLogic comparer = new CompareLogic();

            // General configurations.
            comparer.Config.MaxDifferences = dc.MaxPropertyDifferences;
            comparer.Config.DoublePrecision = dc.ComparisonConfig.NumericTolerance;

            // Set the properties to be ignored.
            if (!dc.ComparisonConfig.PropertyExceptions?.Contains("BHoM_Guid") ?? true)
                dc.ComparisonConfig.PropertyExceptions.Add("BHoM_Guid");
            // the above should be replaced by BH.Engine.Reflection.Compute.RecordWarning($"`BHoM_Guid` should generally be ignored when computing the diffing. Consider adding it to the {nameof(DiffingConfig.PropertiesToIgnore)}.");
            // when the bug in the auto Create() method ("auto-property initialisers for ByRef values like lists do not populate default values") is resolved.

            comparer.Config.MembersToIgnore = dc.ComparisonConfig.PropertyExceptions;

            // Never include the changes in HashFragment.
            comparer.Config.TypesToIgnore.Add(typeof(HashFragment));
            comparer.Config.TypesToIgnore.Add(typeof(RevisionFragment));
            comparer.Config.TypesToIgnore.AddRange(dc.ComparisonConfig.TypeExceptions);

            // Perform the comparison.
            ComparisonResult result = comparer.Compare(obj1Copy, obj2Copy);

            if (result.Differences.Count == dc.MaxPropertyDifferences)
                BH.Engine.Reflection.Compute.RecordWarning($"Hit the limit of {nameof(DiffingConfig.MaxPropertyDifferences)} specified in the {nameof(DiffingConfig)}.");

            // Parse and store the differnces as appropriate.
            foreach (var difference in result.Differences)
            {
                string propertyName = difference.PropertyName;
                string propertyFullName = differentTypes ? commonType + "." + propertyName : propertyName; // propertyPath does not need to record the base common type if the types are equal.

                // The property path without indexes in square brackets is useful to check if it matches with any property exception.
                // E.g. Bar.Fragments[1].Parameters[5].Name becomes Bar.Fragments.Parameters.Name, so we can check that against exceptions like `Parameters.Name`.
                string propertyFullName_noIndexes = propertyFullName.RemoveSquareIndexing();

                // If there is a PropertyFullNameModifier, invoke it to modify the property full name.
                if (dc.ComparisonConfig.ComparisonFunctions?.PropertyFullNameModifier != null)
                {
                    propertyFullName = dc.ComparisonConfig.ComparisonFunctions.PropertyFullNameModifier.Invoke(propertyFullName, obj2);
                    propertyFullName_noIndexes = propertyFullName;
                }

                // If there is a PropertyFullNameFilter, invoke it to see if we should discard this property difference.
                if (dc.ComparisonConfig.ComparisonFunctions.PropertyFullNameFilter != null)
                    if (dc.ComparisonConfig.ComparisonFunctions.PropertyFullNameFilter.Invoke(propertyFullName_noIndexes, obj2))
                        continue;

                // Skip if the property is among the PropertyExceptions.
                if (dc.ComparisonConfig.PropertyExceptions.Any() && dc.ComparisonConfig.PropertyExceptions.Any(pe => propertyFullName_noIndexes.EndsWith(pe)))
                    continue;

                // Check if this difference is a difference in terms of CustomData.
                if (propertyFullName.Contains("CustomData") && propertyFullName.Contains("Value"))
                {
                    // Get the custom data Key, so we can check if it belongs to the exceptions.
                    int keyStart = propertyFullName.IndexOf('[') + 1;
                    int keyEnd = propertyFullName.IndexOf(']');
                    string customDataKey = propertyFullName.Substring(keyStart, keyEnd - keyStart);

                    // If there is a CustomDataKeyFilter, invoke it to see if we should discard this CustomData difference.
                    if (dc.ComparisonConfig.ComparisonFunctions.CustomDataKeyFilter != null)
                        if (dc.ComparisonConfig.ComparisonFunctions.CustomDataKeyFilter.Invoke(customDataKey, obj2))
                            continue;

                    // If the customDataKey is not in the CustomdataKeysToInclude, skip this CustomData difference.
                    if (!dc.ComparisonConfig.CustomdataKeysToInclude.Contains(customDataKey))
                        continue;

                    // Skip this custom data if the key belongs to the exceptions.
                    if (dc.ComparisonConfig.CustomdataKeysExceptions.Contains(customDataKey))
                        continue;

                    // Just for aesthetic reasons, remove the first dot in the name between CustomData.[keyname].etc
                    propertyFullName = propertyFullName.Remove(propertyFullName.IndexOf('.'), 1);
                }

                // Check if the property Full name matches any of the specified PropertiesToConsider.
                if (dc.ComparisonConfig.PropertiesToConsider.Any())
                {
                    if (!dc.ComparisonConfig.PropertiesToConsider.Any(ptc => propertyFullName.EndsWith(ptc)))
                        continue; // no match found, skip this property.
                }

                // Add to the final result.
                dict[propertyFullName] = new Tuple<object, object>(difference.Object1, difference.Object2);
            }

            if (dict.Count == 0)
                return null;

            return dict; // this Dictionary may be exploded in the UI by using the method "ListDifferentProperties".
        }

        [Description("Removes square bracket indexing from property paths, e.g. `Bar.Fragments[0].Something` is returned as `Bar.Fragments.Something`.")]
        private static string RemoveSquareIndexing(this string propertyPath)
        {
            return System.Text.RegularExpressions.Regex.Replace(propertyPath, @"\[(.*?)\]", string.Empty);
        }
    }
}


