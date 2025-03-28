/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Diffing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using kellerman = KellermanSoftware.CompareNetObjects;

namespace BH.Engine.Diffing
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Compare two versions of the same object that sustained some modification over time, and returns differences.")]
        [Input("pastObject", "Past version of the object (created before `followingObject`).")]
        [Input("followingObject", "Following version of the object (modified or created after `pastObject`).")]
        [Input("comparisonConfig", "Additional configurations to be used for the comparison.")]
        [Output("Returns an `ObjectDifferences` object storing all the found differences between `previousObject` and `followingObject`." +
            "\nIf no difference was found, returns null.")]
        public static ObjectDifferences ObjectDifferences(this object pastObject, object followingObject, BaseComparisonConfig comparisonConfig = null)
        {
            ObjectDifferences result = new ObjectDifferences() { PastObject = pastObject, FollowingObject = followingObject };
            
            // Null check. At least one of the objects must be not null.
            if (pastObject == null && followingObject == null)
                return result;

            result.Differences.AddRange(pastObject.Differences(followingObject, comparisonConfig));
            return result;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Finds the actual differences between the objects.")]
        private static IEnumerable<PropertyDifference> Differences(this object pastObject, object followingObject, BaseComparisonConfig comparisonConfig = null)
        {
            // Result object.
            List<PropertyDifference> returned = new List<PropertyDifference>();
            PropertyDifference toReturn;

            // Set ComparisonConfig if null. Clone it for immutability in the UI.
            BaseComparisonConfig cc = comparisonConfig == null ? new ComparisonConfig() : comparisonConfig.DeepClone();

            // Make sure that the propertiesToConsider and propertyExceptions are specified with their FullName form.
            //cc.PropertyNamesToFullNames(pastObject.GetType());
            //cc.PropertyNamesToFullNames(followingObject.GetType());

            // Make sure that `BHoM_Guid` will NOT be considered amongst the property differences.
            if (!cc.PropertyExceptions?.Contains("BHoM_Guid") ?? true)
                cc.PropertyExceptions.Add("BHoM_Guid");

            // General Kellerman configurations.
            kellerman.CompareLogic kellermanComparer = new kellerman.CompareLogic();
            kellermanComparer.Config.MaxDifferences = cc.MaxPropertyDifferences;
            kellermanComparer.Config.TypesToIgnore.Add(typeof(HashFragment)); // Never include the changes in HashFragment.
            kellermanComparer.Config.TypesToIgnore.Add(typeof(RevisionFragment)); // Never include the changes in RevisionFragment.
            kellermanComparer.Config.TypesToIgnore.AddRange(cc.TypeExceptions);
            kellermanComparer.Config.MembersToIgnore = cc.PropertyExceptions;
            kellermanComparer.Config.CompareStaticFields = false;
            kellermanComparer.Config.CompareStaticProperties = false;

            // Kellerman configuration for tolerance.
            // Setting Custom Tolerance for specific properties is complex with Kellerman. 
            // So instead, we set the Kellerman precision to capture all variations, that is by using the value 0.
            kellermanComparer.Config.DoublePrecision = 0;
            kellermanComparer.Config.DecimalPrecision = 0;

            //Include local copies of the DataTable comparers, as they are ruled out by netstandard builds in the NugetPackage.
            //See: https://github.com/BHoM/BHoM_Engine/issues/3455
            kellermanComparer.Config.CustomComparers.Add(new DataTableComparer(kellerman.RootComparerFactory.GetRootComparer()));
            kellermanComparer.Config.CustomComparers.Add(new DataRowComparer(kellerman.RootComparerFactory.GetRootComparer()));
            kellermanComparer.Config.CustomComparers.Add(new DataColumnComparer(kellerman.RootComparerFactory.GetRootComparer()));

            // Perform the comparison using the Kellerman library.
            kellerman.ComparisonResult kellermanResult = kellermanComparer.Compare(pastObject, followingObject);

            // Alert if the maximum properties cap was hit.
            if (kellermanResult.Differences.Count == cc.MaxPropertyDifferences)
                BH.Engine.Base.Compute.RecordWarning($"Hit the limit of {nameof(cc.MaxPropertyDifferences)} specified in the {nameof(oM.Base.ComparisonConfig)}.");

            // Iterate all property differences found by Kellerman.
            string objectFullName = pastObject == null ? pastObject.GetType().FullName : followingObject.GetType().FullName;
            foreach (var kellermanPropertyDifference in kellermanResult.Differences)
            {
                string propertyName = kellermanPropertyDifference.PropertyName;
                string propertyFullName = objectFullName + "." + propertyName;
                string propertyDisplayName = propertyName;

                // Check Max Nesting level if specified.
                if (cc.MaxNesting != int.MaxValue && cc.MaxNesting >= 0)
                {
                    int nestingLevel = propertyName.Count(c => c == '.');
                    if (nestingLevel >= cc.MaxNesting)
                        continue;
                }

                // Check namespace exceptions if specified.
                if ((cc.NamespaceExceptions?.Any() ?? false) && cc.NamespaceExceptions.Any(ne => propertyFullName.StartsWith(ne)))
                    continue;

                // Get the property path without indexes in square brackets.
                // This is useful to check if it matches with any propertyExceptions/propertiesToConsider.
                // E.g. Bar.Fragments[1].Parameters[5].Name becomes Bar.Fragments.Parameters.Name, so we can check that against exceptions like `Parameters.Name`.
                string propertyFullName_noIndexes = propertyFullName.RemoveSquareIndexing();

                // Check if there is a `PropertyComparisonInclusion()` extension method available for this property difference.
                object comparisonInclusionFromExtensionMethod = null;
                object[] parameters = new object[] { kellermanPropertyDifference.ParentObject2, propertyFullName, comparisonConfig };
                if (BH.Engine.Base.Compute.TryRunExtensionMethod(kellermanPropertyDifference.ParentObject1, "ComparisonInclusion", parameters, out comparisonInclusionFromExtensionMethod))
                {
                    ComparisonInclusion comparisonInclusion = comparisonInclusionFromExtensionMethod as ComparisonInclusion;
                    if (comparisonInclusion != null && comparisonInclusion.Include)
                    {
                        // Add to the final result.
                        toReturn = PropertyDifference(pastObject, followingObject, comparisonInclusion.DisplayName, kellermanPropertyDifference.Object1, kellermanPropertyDifference.Object2, propertyFullName);
                        returned.Add(toReturn);
                        yield return toReturn;
                    }

                    // Because a `ComparisonInclusion()` extension method was found, we've already determined if this difference was to be considered or not. Continue.
                    continue;
                }

                // Check if this difference is a difference in terms of CustomData. It may also be a CustomObject.
                if (propertyFullName.Contains("CustomData") && propertyFullName.Contains("Value"))
                {
                    // Get the custom data Key, so we can check if it belongs to the exceptions.
                    int keyStart = propertyFullName.IndexOf('[') + 1;
                    int keyEnd = propertyFullName.IndexOf(']');
                    string customDataKey = propertyFullName.Substring(keyStart, keyEnd - keyStart);

                    // For aesthetic reasons, remove the first dot in the name between CustomData.[keyname].etc
                    propertyFullName = propertyFullName.Remove(keyStart - 2, 1);
                    propertyFullName = propertyFullName.Replace(".Value", "");

                    // Workaround for Kellerman duplicating the CustomData differences.
                    if (returned.Any(d => d.FullName == propertyFullName))
                        continue;

                    // Check if we are talking about CustomObjects.
                    if (pastObject is CustomObject || followingObject is CustomObject)
                    {
                        // Replace the property name as if this CustomData difference was actually a Property Difference.
                        propertyName = customDataKey;
                        propertyFullName_noIndexes = propertyName;
                        propertyDisplayName = propertyName;
                    }
                    else
                    {
                        propertyDisplayName = $"{customDataKey} (CustomData)";

                        // Skip this custom data if the key belongs to the exceptions.
                        if (cc.CustomdataKeysExceptions?.Any(cdKeyExcept => cdKeyExcept == customDataKey || customDataKey.WildcardMatch(cdKeyExcept)) ?? false)
                            continue;

                        // If there are CustomdataKeysToConsider specified and this customDataKey is not among them, skip it.
                        if ((cc.CustomdataKeysToConsider?.Any() ?? false) && !cc.CustomdataKeysToConsider.Any(cdkeyToInc => cdkeyToInc == customDataKey || customDataKey.WildcardMatch(cdkeyToInc)))
                            continue;
                    }
                }

                // Skip if the property is among the PropertyExceptions.
                if ((cc.PropertyExceptions?.Any(pe => propertyFullName_noIndexes.EndsWith(pe) || propertyFullName_noIndexes.PropertyNameWildcardMatch(pe)) ?? false))
                    continue;

                // If the PropertiesToConsider contains at least a value, ensure that this property is among them.
                if ((cc.PropertiesToConsider?.Any() ?? false) &&
                    !cc.PropertiesToConsider.Any(ptc =>
                        propertyFullName_noIndexes.EndsWith(ptc) || propertyFullName_noIndexes.PropertyNameWildcardMatch(ptc)
                        || propertyName.StartsWith($"{ptc}.") // make sure we include sub-properties, for those cases where we did not collect all actual propertiesFullNames from the CC because of interface properties.
                        )
                    )
                    continue;

                // Check if this difference is numerical, and if so whether it should be included or not given the input tolerance/significant figures.
                if (!NumericalDifferenceInclusion(kellermanPropertyDifference.Object1, kellermanPropertyDifference.Object2, propertyFullName_noIndexes, cc))
                    continue;

                // Add to the final result.
                toReturn = PropertyDifference(pastObject, followingObject, propertyDisplayName, kellermanPropertyDifference.Object1, kellermanPropertyDifference.Object2, propertyFullName);
                returned.Add(toReturn);
                yield return toReturn;
            }
        }

        /***************************************************/

        [Description("Removes square bracket indexing from property paths, e.g. `Bar.Fragments[0].Something` is returned as `Bar.Fragments.Something`.")]
        private static PropertyDifference PropertyDifference(this object pastObject, object followingObject, string propertyDiffDisplayName, object pastValue, object follValue, string fullName, string description = null)
        {
            description = string.IsNullOrWhiteSpace(description) ?
                PropertyDifferenceDescription(pastObject, followingObject, propertyDiffDisplayName, pastValue, follValue)
                : description;

            return new PropertyDifference()
            {
                Name = propertyDiffDisplayName,
                Description = description,
                PastValue = pastValue,
                FollowingValue = follValue,
                FullName = fullName
            };
        }

        /***************************************************/

        private static string PropertyDifferenceDescription(this object pastObject, object followingObject, string propertyDiffDisplayName, object pastValue, object follValue, bool includeObjName = true, bool includeObjType = false, bool includeObjGuid = false)
        {
            if (pastValue is IEnumerable && follValue is IEnumerable)
                return $"The collection stored in the property `{propertyDiffDisplayName}` of the `{pastObject.GetType().FullName}` was modified.";

            Type t = pastValue?.GetType() ?? follValue?.GetType() ?? typeof(object);

            // If required, start the description with the Name of the modified object, if present, and if it was not modified.
            string objectDescription = "";
            if (includeObjName || includeObjType)
            {
                bool addedObjectDescription = false;

                objectDescription = pastObject is CustomObject ? "CustomObject " : "Object ";

                if (includeObjName)
                {
                    string pastObjName = (pastObject as IBHoMObject)?.Name;
                    string follObjName = (followingObject as IBHoMObject)?.Name;

                    if (!string.IsNullOrWhiteSpace(pastObjName) && pastObjName == follObjName)
                    {
                        objectDescription += $"with {nameof(IBHoMObject.Name)} `{pastObjName}` ";
                        addedObjectDescription = true;
                    }
                }

                if (includeObjType && !(pastObject is CustomObject))
                {
                    objectDescription += $"of type `{pastObject.GetType().FullName}` ";
                    addedObjectDescription = true;
                }

                if (includeObjGuid)
                {
                    Guid? pastObjGuid = (pastObject as IBHoMObject)?.BHoM_Guid;
                    Guid? follObjGuid = (followingObject as IBHoMObject)?.BHoM_Guid;

                    if (pastObjGuid != null && pastObjGuid == follObjGuid)
                    {
                        objectDescription += $"with {nameof(IBHoMObject.BHoM_Guid)} `{pastObjGuid}` ";
                        addedObjectDescription = true;
                    }
                }

                objectDescription += "was modified. ";

                if (!addedObjectDescription)
                    objectDescription = "";
            }

            if (t.IsPrimitive || (t == typeof(string)) || t.IsValueType)
            {
                if (pastValue != null && follValue == null)
                    return $"{objectDescription}The value assigned to the property `{propertyDiffDisplayName}` was removed (made null); it previously contained `{pastValue}`.";

                if (pastValue == null && follValue != null)
                    return $"{objectDescription}Some value was assigned to the property `{propertyDiffDisplayName}` that was previously not populated (null). The property now contains: {follValue}.";

                return $"{objectDescription}The value assigned to the property `{propertyDiffDisplayName}` was modified from `{pastValue}` to `{follValue}`.";
            }

            return $"{objectDescription}The value assigned to the property `{propertyDiffDisplayName}` was modified.";
        }

        /***************************************************/

        [Description("Removes square bracket indexing from property paths, e.g. `Bar.Fragments[0].Something` is returned as `Bar.Fragments.Something`.")]
        private static string RemoveSquareIndexing(this string propertyPath)
        {
            return System.Text.RegularExpressions.Regex.Replace(propertyPath, @"\[(.*?)\]", string.Empty);
        }

        /***************************************************/

        [Description("If the wildcardPattern is not a fullname, prepends a wildcard to it, then checks if it matches a given propertyName.")]
        private static bool PropertyNameWildcardMatch(this string propertyName, string wildcardPattern)
        {
            if (string.IsNullOrWhiteSpace(propertyName) || string.IsNullOrWhiteSpace(wildcardPattern))
                return false;

            if (!wildcardPattern.StartsWith("BH."))
                wildcardPattern = "*" + wildcardPattern;

            return propertyName.WildcardMatch(wildcardPattern);
        }

        /***************************************************/
    }
}

