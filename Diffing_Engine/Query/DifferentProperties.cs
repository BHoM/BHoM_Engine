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
        [Input("diffConfig", "Config to be used for the comparison. Can set numeric tolerance, wheter to check the guid, if custom data should be ignored and if any additional properties should be ignored")]
        [Output("Dictionary whose key is the name of the property, and value is a tuple with its value in obj1 and obj2.")]
        public static Dictionary<string, Tuple<object, object>> DifferentProperties(this object obj1, object obj2, DiffingConfig diffConfig = null)
        {
            // Set configurations if diffConfig is null. Clone it for immutability in the UI.
            DiffingConfig dc = diffConfig == null ? new DiffingConfig() : diffConfig.DeepClone() as DiffingConfig;

            object obj1Copy = obj1.DeepClone();
            object obj2Copy = obj2.DeepClone();

            var dict = new Dictionary<string, Tuple<object, object>>();

            CompareLogic comparer = new CompareLogic();

            // General configurations.
            comparer.Config.MaxDifferences = dc.MaxPropertyDifferences;
            comparer.Config.DoublePrecision = dc.DistinctConfig.NumericTolerance;

            // Set the properties to be ignored.
            if (!dc.DistinctConfig.PropertyNameExceptions?.Contains("BHoM_Guid") ?? true)
                dc.DistinctConfig.PropertyNameExceptions.Add("BHoM_Guid");
                // the above should be replaced by BH.Engine.Reflection.Compute.RecordWarning($"`BHoM_Guid` should generally be ignored when computing the diffing. Consider adding it to the {nameof(diffConfig.PropertiesToIgnore)}.");
                // when the bug in the auto Create() method ("auto-property initialisers for ByRef values like lists do not populate default values") is resolved.

            comparer.Config.MembersToIgnore = dc.DistinctConfig.PropertyNameExceptions;

            // Removes the CustomData to be ignored.
            var bhomobj1 = (obj1Copy as IBHoMObject);
            var bhomobj2 = (obj2Copy as IBHoMObject);

            if (bhomobj1 != null)
            {
                dc.DistinctConfig.CustomdataKeysExceptions.ForEach(k => bhomobj1.CustomData.Remove(k));
                obj1Copy = bhomobj1;
            }

            if (bhomobj2 != null)
            {
                dc.DistinctConfig.CustomdataKeysExceptions.ForEach(k => bhomobj2.CustomData.Remove(k));
                obj2Copy = bhomobj2;
            }

            // Never include the changes in HashFragment.
            comparer.Config.TypesToIgnore.Add(typeof(HashFragment));
            comparer.Config.TypesToIgnore.Add(typeof(RevisionFragment));

            // Perform the comparison.
            ComparisonResult result = comparer.Compare(obj1Copy, obj2Copy);

            // Parse and store the differnces as appropriate.
            foreach (var difference in result.Differences)
            {
                string propertyName = difference.PropertyName;

                //workaround for Revit's parameters in Fragments
                if (propertyName.Contains("Fragments") && propertyName.Contains("Parameter") && propertyName.Contains("Value"))
                    propertyName = BH.Engine.Reflection.Query.PropertyValue(difference.ParentObject2, "Name").ToString();

                if (propertyName.Contains("CustomData") && propertyName.Contains("Value"))
                {
                    var splittedName = difference.PropertyName.Split('.');

                    int idx = 0;
                    Int32.TryParse(string.Join(null, System.Text.RegularExpressions.Regex.Split(splittedName.ElementAtOrDefault(1), "[^\\d]")), out idx);

                    string keyName = (obj2Copy as IBHoMObject)?.CustomData.ElementAtOrDefault(idx - 1).Key; // this seems buggy ATM.

                    propertyName = splittedName.FirstOrDefault() + $"['{keyName}']." + splittedName.Last();
                }

                if (dc.DistinctConfig.PropertyNameExceptions.Any() && !dc.DistinctConfig.PropertyNameExceptions.Contains(difference.PropertyName))
                    dict[propertyName] = new Tuple<object, object>(difference.Object1, difference.Object2);
            }

            if (dict.Count == 0)
                return null;

            return dict; // this Dictionary may be exploded in the UI by using the method "ListDifferentProperties".
        }
    }
}

