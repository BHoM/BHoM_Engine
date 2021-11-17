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
using BH.Engine.Reflection;

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
        public static ObjectDifferences ObjectDifferences(this object pastObject, object followingObject, BaseComparisonConfig comparisonConfig = null)
        {
            // Result object.
            ObjectDifferences result = new ObjectDifferences() { PastObject = pastObject, FollowingObject = followingObject };

            // Null check. At least one of the objects must be not null.
            if (pastObject == null && followingObject == null)
                return result;

            // Set ComparisonConfig if null. Clone it for immutability in the UI.
            BaseComparisonConfig cc = comparisonConfig == null ? new ComparisonConfig() : comparisonConfig.DeepClone();

            // Make sure that the propertiesToConsider and propertyExceptions are specified with their FullName form.
            cc.PropertyNamesToFullNames(pastObject.GetType());
            cc.PropertyNamesToFullNames(followingObject.GetType());

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

            // Kellerman configuration for tolerance.
            // Setting Custom Tolerance for specific properties is complex with Kellerman. 
            // So instead, we set the Kellerman precision to capture all variations, that is by using the value 0.
            kellermanComparer.Config.DoublePrecision = 0;
            kellermanComparer.Config.DecimalPrecision = 0;

            // Perform the comparison using the Kellerman library.
            kellerman.ComparisonResult kellermanResult = kellermanComparer.Compare(pastObject, followingObject);

            // Alert if the maximum properties cap was hit.
            if (kellermanResult.Differences.Count == cc.MaxPropertyDifferences)
                BH.Engine.Reflection.Compute.RecordWarning($"Hit the limit of {nameof(cc.MaxPropertyDifferences)} specified in the {nameof(oM.Base.ComparisonConfig)}.");

            // Iterate all property differences found by Kellerman.
            string objectFullName = pastObject == null ? pastObject.GetType().FullName : followingObject.GetType().FullName;
            foreach (var kellermanPropertyDifference in kellermanResult.Differences)
            {
                string propertyName = kellermanPropertyDifference.PropertyName;
                string propertyFullName = objectFullName + "." + propertyName;
                string propertyDisplayName = propertyName;

                // Check if there is a `PropertyComparisonInclusion()` extension method available for this property difference.
                object propCompIncl = null;
                object[] parameters = new object[] { propertyFullName, cc };
                if (BH.Engine.Reflection.Compute.TryRunExtensionMethod(kellermanPropertyDifference.ParentObject2, "ComparisonInclusion", parameters, out propCompIncl))
                {
                    ComparisonInclusion propComparisonInclusion = propCompIncl as ComparisonInclusion;

                    if (propComparisonInclusion.Include)
                    {
                        // Add to the final result.
                        result.Differences.Add(new PropertyDifference()
                        {
                            DisplayName = propComparisonInclusion.DisplayName,
                            PastValue = kellermanPropertyDifference.Object1,
                            FollowingValue = kellermanPropertyDifference.Object2,
                            FullName = propertyFullName
                        });
                    }

                    // Because a `PropertyComparisonInclusion()` extension method was found, we've already determined if this property difference was to be added or not. Continue.
                    continue;
                }

                // Get the property path without indexes in square brackets.
                // This is useful to check if it matches with any propertyExceptions/propertiesToConsider.
                // E.g. Bar.Fragments[1].Parameters[5].Name becomes Bar.Fragments.Parameters.Name, so we can check that against exceptions like `Parameters.Name`.
                string propertyFullName_noIndexes = propertyFullName.RemoveSquareIndexing();

                // Skip if the property is among the PropertyExceptions.
                if ((cc.PropertyExceptions?.Contains(propertyFullName_noIndexes) ?? false))
                    continue;

                // If the PropertiesToConsider contains at least a value, ensure that this property is among them.
                if ((cc.PropertiesToConsider?.Any() ?? false) && !cc.PropertiesToConsider.Contains(propertyFullName_noIndexes))
                    continue;

                // Check if this difference is a difference in terms of CustomData.
                if (propertyFullName.Contains("CustomData") && propertyFullName.Contains("Value"))
                {
                    // Get the custom data Key, so we can check if it belongs to the exceptions.
                    int keyStart = propertyFullName.IndexOf('[') + 1;
                    int keyEnd = propertyFullName.IndexOf(']');
                    string customDataKey = propertyFullName.Substring(keyStart, keyEnd - keyStart);

                    // If there are CustomdataKeysToInclude specified and this customDataKey is not among them, skip it.
                    if ((cc.CustomdataKeysToInclude?.Any() ?? false) && !cc.CustomdataKeysToInclude.Contains(customDataKey))
                        continue;

                    // Skip this custom data if the key belongs to the exceptions.
                    if (cc.CustomdataKeysExceptions?.Contains(customDataKey) ?? false)
                        continue;

                    // Just for aesthetic reasons, remove the first dot in the name between CustomData.[keyname].etc
                    propertyDisplayName = propertyFullName.Remove(propertyFullName.IndexOf('.'), 1);
                }

                // Check if we specified CustomTolerances and if this difference is a number difference.
                if (cc.NumericTolerance != double.MinValue || cc.SignificantFigures != int.MaxValue
                    || (cc.PropertyNumericTolerances?.Any() ?? false) || (cc.PropertySignificantFigures?.Any() ?? false)
                    && kellermanPropertyDifference.Object1.GetType().IsNumeric() && kellermanPropertyDifference.Object2.GetType().IsNumeric()) // GetType() is slow; call only after checking that any custom tolerance is present.
                {
                    // We have specified some custom tolerance in the ComparisonConfig AND this property difference is numeric.
                    // Because we have set Kellerman to retrieve any possible numerical variation,
                    // we now want to "filter out" number variations following our BHoM ComparisonConfig settings.

                    if (cc.NumericTolerance != double.MinValue || (cc.PropertyNumericTolerances?.Any() ?? false))
                    {
                        double tolerance = cc.PropertyNumericTolerance(propertyFullName_noIndexes);
                        if (tolerance != double.MinValue)
                        {
                            double value1 = double.Parse(kellermanPropertyDifference.Object1.ToString()).RoundWithTolerance(tolerance);
                            double value2 = double.Parse(kellermanPropertyDifference.Object2.ToString()).RoundWithTolerance(tolerance);

                            // If, once rounded, the numbers are the same, it means that we do not want to consider this Difference. Skip.
                            if (value1 == value2)
                                continue;
                        }
                    }

                    if (cc.SignificantFigures != int.MaxValue || (cc.PropertySignificantFigures?.Any() ?? false))
                    {
                        int significantFigures = cc.PropertySignificantFigures(propertyFullName_noIndexes);
                        if (significantFigures != int.MaxValue)
                        {
                            double value1 = double.Parse(kellermanPropertyDifference.Object1.ToString()).RoundToSignificantFigures(significantFigures);
                            double value2 = double.Parse(kellermanPropertyDifference.Object2.ToString()).RoundToSignificantFigures(significantFigures);

                            // If, once rounded, the numbers are the same, it means that we do not want to consider this Difference. Skip.
                            if (value1 == value2)
                                continue;
                        }
                    }
                }

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

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Removes square bracket indexing from property paths, e.g. `Bar.Fragments[0].Something` is returned as `Bar.Fragments.Something`.")]
        private static string RemoveSquareIndexing(this string propertyPath)
        {
            return System.Text.RegularExpressions.Regex.Replace(propertyPath, @"\[(.*?)\]", string.Empty);
        }

        /***************************************************/

        private static bool IsFloatingPointNumber(this object obj)
        {
            return obj is double || obj is float || obj is decimal;
        }
    }
}


