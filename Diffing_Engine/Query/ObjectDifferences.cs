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
using kellerman = KellermanSoftware.CompareNetObjects;
using System.Reflection;
using BH.Engine.Base;

namespace BH.Engine.Diffing
{
    public static partial class Query
    {
        [Description("Compare two versions of the same object that sustained some modification over time, and returns differences.")]
        [Input("pastObject", "Past version of the object (created before `followingObject`).")]
        [Input("followingObject", "Following version of the object (modified or created after `pastObject`).")]
        [Input("comparisonConfig", "Additional configurations to be used for the comparison.")]
        [Output("Returns an `ObjectDifferences` object storing all the found differences between `previousObject` and `followingObject`." +
            "\nIf no difference was found, returns null.")]
        [PreviousVersion("5.0", "BH.Engine.Diffing.DifferentProperties(System.Object, System.Object, BH.oM.Base.ComparisonConfig)")]
        public static ObjectDifferences ObjectDifferences(this object pastObject, object followingObject, ComparisonConfig comparisonConfig = null)
        {
            // Set ComparisonConfig if null. Clone it for immutability in the UI.
            ComparisonConfig cc = comparisonConfig == null ? new BaseComparisonConfig() : comparisonConfig.DeepClone();

            ObjectDifferences result = new ObjectDifferences() { PastObject = pastObject, FollowingObject = followingObject };

            // General Kellerman configurations.
            kellerman.CompareLogic kellermanComparer = new kellerman.CompareLogic();
            kellermanComparer.Config.MaxDifferences = cc.MaxPropertyDifferences;
            kellermanComparer.Config.DoublePrecision = cc.NumericTolerance;
            kellermanComparer.Config.TypesToIgnore.Add(typeof(HashFragment)); // Never include the changes in HashFragment.
            kellermanComparer.Config.TypesToIgnore.Add(typeof(RevisionFragment)); // Never include the changes in RevisionFragment.
            kellermanComparer.Config.TypesToIgnore.AddRange(cc.TypeExceptions);

            // Make sure that `BHoM_Guid` will NOT be considered amongst the property differences.
            if (!cc.PropertyExceptions?.Contains("BHoM_Guid") ?? true)
                cc.PropertyExceptions.Add("BHoM_Guid");

            // Set the properties to be ignored.
            kellermanComparer.Config.MembersToIgnore = cc.PropertyExceptions;

            // Perform the comparison using the Kellerman library.
            kellerman.ComparisonResult kellermanResult = kellermanComparer.Compare(pastObject, followingObject);

            // Alert if the maximum properties cap was hit.
            if (kellermanResult.Differences.Count == cc.MaxPropertyDifferences)
                BH.Engine.Reflection.Compute.RecordWarning($"Hit the limit of {nameof(cc.MaxPropertyDifferences)} specified in the {nameof(oM.Base.ComparisonConfig)}.");

            // Check if the objects to be compared are of the same type. Useful when recording information about the difference.
            bool sameType = true;
            Type commonType = pastObject?.GetType();
            if (commonType != followingObject?.GetType())
                sameType = false;

            // Iterate all property differences found by Kellerman.
            foreach (var kellermanPropertyDifference in kellermanResult.Differences)
            {
                string propertyName = kellermanPropertyDifference.PropertyName;
                string propertyFullName = sameType ? commonType.FullName + "." + propertyName : propertyName; // if the objects are of the same type, add the object Type fullname to the propertyName.
                string propertyDisplayName = propertyFullName;

                // Check if there is a `PropertyComparisonInclusion()` extension method available for this property difference.
                object propCompIncl = null;
                object[] parameters = new object[] { propertyFullName, cc };
                if (BH.Engine.Reflection.Compute.TryRunExtensionMethod(kellermanPropertyDifference.ParentObject2, "PropertyComparisonInclusion", parameters, out propCompIncl))
                {
                    PropertyComparisonInclusion propertyComparisonInclusion = propCompIncl as PropertyComparisonInclusion;

                    propertyDisplayName = propertyComparisonInclusion.PropertyDisplayName;

                    if (propertyComparisonInclusion.IncludeProperty)
                    {
                        // Add to the final result.
                        result.Differences.Add(new PropertyDifference()
                        {
                            DisplayName = propertyDisplayName,
                            PastValue = kellermanPropertyDifference.Object1,
                            FollowingValue = kellermanPropertyDifference.Object2,
                            FullName = propertyFullName
                        });
                    }

                    // Because a `PropertyComparisonInclusion()` extension method was found, we've already determined if this property difference was to be added or not. Continue.
                    continue;
                }

                // Get the property path without indexes in square brackets.
                // This is useful to check if it matches with any property exception.
                // E.g. Bar.Fragments[1].Parameters[5].Name becomes Bar.Fragments.Parameters.Name, so we can check that against exceptions like `Parameters.Name`.
                string propertyFullName_noIndexes = propertyFullName.RemoveSquareIndexing();

                // If there is a PropertyFullNameModifier, invoke it to modify the property full name.
                if (cc.ComparisonFunctions?.PropertyFullNameModifier != null)
                {
                    propertyFullName = cc.ComparisonFunctions.PropertyFullNameModifier.Invoke(propertyFullName, kellermanPropertyDifference.ParentObject2);
                    propertyFullName_noIndexes = propertyFullName;
                    propertyDisplayName = propertyFullName;
                }

                // If there is a PropertyFullNameFilter, invoke it to see if we should discard this property difference.
                if (cc.ComparisonFunctions?.PropertyFullNameFilter != null)
                    if (cc.ComparisonFunctions.PropertyFullNameFilter.Invoke(propertyFullName_noIndexes, kellermanPropertyDifference.ParentObject2))
                        continue;

                // Skip if the property is among the PropertyExceptions.
                if (cc.PropertyExceptions?.Any() ?? false && cc.PropertyExceptions.Any(pe => propertyFullName_noIndexes.EndsWith(pe)))
                    continue;

                // Check if this difference is a difference in terms of CustomData.
                if (propertyFullName.Contains("CustomData") && propertyFullName.Contains("Value"))
                {
                    // Get the custom data Key, so we can check if it belongs to the exceptions.
                    int keyStart = propertyFullName.IndexOf('[') + 1;
                    int keyEnd = propertyFullName.IndexOf(']');
                    string customDataKey = propertyFullName.Substring(keyStart, keyEnd - keyStart);

                    // If there is a CustomDataKeyFilter, invoke it to see if we should discard this CustomData difference.
                    if (cc.ComparisonFunctions?.CustomDataKeyFilter != null)
                        if (cc.ComparisonFunctions.CustomDataKeyFilter.Invoke(customDataKey, kellermanPropertyDifference.ParentObject2))
                            continue;

                    // If there are CustomdataKeysToInclude specified and this customDataKey is not among them, skip it.
                    if ((cc.CustomdataKeysToInclude?.Any() ?? false) && !cc.CustomdataKeysToInclude.Contains(customDataKey))
                        continue;

                    // Skip this custom data if the key belongs to the exceptions.
                    if (cc.CustomdataKeysExceptions?.Contains(customDataKey) ?? false)
                        continue;

                    // Just for aesthetic reasons, remove the first dot in the name between CustomData.[keyname].etc
                    propertyDisplayName = propertyFullName.Remove(propertyFullName.IndexOf('.'), 1);
                }

                // Check if the property Full name matches any of the specified PropertiesToConsider.
                if (cc.PropertiesToConsider?.Any() ?? false)
                    if (!cc.PropertiesToConsider.Any(ptc => propertyFullName == ptc || propertyFullName.EndsWith($".{ptc}")))
                        continue; // no match found, skip this property.

                // If there is a PropertyDisplayNameModifier, invoke it to modify the property full name.
                if (cc.ComparisonFunctions?.PropertyDisplayNameModifier != null)
                    propertyDisplayName = cc.ComparisonFunctions.PropertyDisplayNameModifier.Invoke(propertyFullName, kellermanPropertyDifference.ParentObject2);

                // Add to the final result.
                result.Differences.Add(new PropertyDifference()
                {
                    DisplayName = propertyDisplayName,
                    PastValue = kellermanPropertyDifference.Object1,
                    FollowingValue = kellermanPropertyDifference.Object2,
                    FullName = propertyFullName
                });
            }

            if (result.Differences.Count == 0)
                return null;

            return result;
        }

        [Description("Removes square bracket indexing from property paths, e.g. `Bar.Fragments[0].Something` is returned as `Bar.Fragments.Something`.")]
        private static string RemoveSquareIndexing(this string propertyPath)
        {
            return System.Text.RegularExpressions.Regex.Replace(propertyPath, @"\[(.*?)\]", string.Empty);
        }
    }
}


