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

using BH.oM.Base;
using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Base
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Parse the ComparisonConfig's property inclusions (the `PropertiesToConsider` and `PropertyExceptions`), and get them all as Full Names." +
            "This allows to make sure we collect all relevant properties, even if we are given partial names or wildcards (e.g. `StartNode.*.X`).")]
        [Input("comparisonConfig", "ComparisonConfig object whose `PropertiesToConsider` and `PropertyExceptions` will be parsed." +
            "If they contain partial names or wildcards, they will be expanded with all the matching property FullNames found for the specified Type.")]
        [Input("type", "Object type whose properties will be collected and matched against the comparisonConfig's `PropertiesToConsider` and `PropertyExceptions`.")]
        [Input("cache", "If set to true and the object already contains a fragment of the type being added, the fragment will be replaced by this instance.")]
        public static void PropertyInclusionsToFullNames(this BaseComparisonConfig comparisonConfig, Type type, bool cache = true)
        {
            // Null checks.
            if (comparisonConfig == null || !typeof(IObject).IsAssignableFrom(type))
                return;

            // Result variables.
            List<string> propertiesToConsider_fullNames = null;
            List<string> propertyExceptions_fullNames = null;

            // If all the propertiesToConsider/propertyExceptions are already in FullName form and without wildcards, do nothing.
            List<string> propertiesToConsiderToParse = comparisonConfig.PropertiesToConsider.Where(p => !p.StartsWith("BH.") || p.Contains("*")).ToList();
            List<string> propertyExceptionsToParse = comparisonConfig.PropertyExceptions.Where(p => !p.StartsWith("BH.") || p.Contains("*")).ToList();

            if (!propertiesToConsiderToParse.Any() && !propertyExceptionsToParse.Any())
                return;

            // Collect all property FullNames for this Type of object.
            HashSet<string> allPropertiesFullNames = BH.Engine.Reflection.Query.GetAllPropertyFullNames(type, comparisonConfig.MaxNesting, true);

            // Check if we already have encountered and cached this same object Type and PropertiesToConsider.
            string propertiesToConsiderCacheKey = $"{type.FullName}:{string.Join(",", comparisonConfig.PropertiesToConsider)}";
            if (cache && !m_cachedPropertiesToConsider.TryGetValue(propertiesToConsiderCacheKey, out propertiesToConsider_fullNames))
            {
                propertiesToConsider_fullNames = new List<string>();

                // If not, parse the PropertiesToConsider and all the Properties of the object,
                // so we convert partial property names/wildcards to their matching Full Name form.
                foreach (var propToConsider in propertiesToConsiderToParse)
                    foreach (var propertyFullName in allPropertiesFullNames)
                        if (IsMatchingInclusion(propertyFullName, propToConsider))
                            propertiesToConsider_fullNames.Add(propertyFullName);

                m_cachedPropertiesToConsider[propertiesToConsiderCacheKey] = propertiesToConsider_fullNames;
            }

            // Check if we already have encountered and cached this same object Type and PropertyExceptions.
            string propertyExceptionsCacheKey = $"{type.FullName}:{string.Join(",", comparisonConfig.PropertyExceptions)}";
            if (cache && !m_cachedPropertiesToConsider.TryGetValue(propertyExceptionsCacheKey, out propertyExceptions_fullNames))
            {
                propertyExceptions_fullNames = new List<string>();

                // If not, parse the PropertyExceptions and all the Properties of the object,
                // so we convert partial property names/wildcards to their matching Full Name form.
                foreach (var propException in propertyExceptionsToParse)
                    foreach (var propertyFullName in allPropertiesFullNames)
                        if (IsMatchingInclusion(propertyFullName, propException))
                            comparisonConfig.PropertyExceptions.Add(propertyFullName);

                m_cachedPropertyExceptions[propertyExceptionsCacheKey] = propertyExceptions_fullNames;
            }

            // Add the results to the ComparisonConfig.
            comparisonConfig.PropertiesToConsider.AddRange(propertiesToConsider_fullNames);
            comparisonConfig.PropertyExceptions.AddRange(propertyExceptions_fullNames);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        // Check if the given propertyFullName matches with the input inclusionOrExclusion.
        private static bool IsMatchingInclusion(string propertyFullName, string inclusionOrExclusion)
        {
            // If the propertyFullName ends with the inclusion or exclusion, it is a match.
            // E.g. propertyFullName = "BH.oM.Engine.Structure.Elements.Bar.Name"; inclusionOrExclusion = "Name".
            if (propertyFullName.EndsWith($".{inclusionOrExclusion}"))
                return true;

            // If the inclusionOrExclusion is not a FullName (i.e. it does not start with "BH.")
            // and it also does NOT have a starting wildcard (i.e. "StartNode.*.X" and not "*.StartNode.*.X"),
            // a wildcard must be prepended to make sure we match all possible properties.
            if (!inclusionOrExclusion.StartsWith("BH") && !inclusionOrExclusion.StartsWith("*"))
                inclusionOrExclusion = $"*{inclusionOrExclusion}";

            // Finds if there is a match.
            return Query.WildcardMatch(propertyFullName, inclusionOrExclusion);
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<string, List<string>> m_cachedPropertiesToConsider = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> m_cachedPropertyExceptions = new Dictionary<string, List<string>>();
    }
}
