/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using System.ComponentModel;

namespace BH.Engine.Base
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Parse the ComparisonConfig's property-based configs (`PropertiesToConsider`, `PropertyExceptions`, etc.), and get them all as Full Names." +
            "This allows to make sure we collect all relevant properties, even if we are given partial names or wildcards (e.g. `Start.*.X`).")]
        [Input("comparisonConfig", "ComparisonConfig object whose `PropertiesToConsider` and `PropertyExceptions` will be parsed." +
            "If they contain partial names or wildcards, they will be expanded with all the matching property FullNames found for the specified Type.")]
        [Input("type", "Object type whose properties will be collected and matched against the comparisonConfig's `PropertiesToConsider`, `PropertyExceptions`, and the other property-based configs.")]
        [Input("cache", "If set to true and the object already contains a fragment of the type being added, the fragment will be replaced by this instance.")]
        public static void PropertyNamesToFullNames(this BaseComparisonConfig comparisonConfig, Type type, bool cache = true)
        {
            // Null checks.
            if (comparisonConfig == null || !typeof(IObject).IsAssignableFrom(type))
                return;

            // Check if this combination of comparisonConfig and Type was already processed.
            Tuple<Type, object> cachedResultKey = new Tuple<Type, object>(type, comparisonConfig);
            if (m_ComparisonConfig_Type_processed.Keys.Contains(cachedResultKey))
            {
                comparisonConfig = (BaseComparisonConfig)m_ComparisonConfig_Type_processed[cachedResultKey];
                return;
            }

            // Result variables.
            List<string> propertiesToConsider_fullNames = null;
            List<string> propertyExceptions_fullNames = null;
            HashSet<NamedNumericTolerance> propertyNumericTolerances_fullNames = null;

            // We need to process only those properties that have not been specified in FullName form, or that contain wildcards.
            List<string> propertiesToConsiderToParse = comparisonConfig.PropertiesToConsider?.Where(p => !p.StartsWith("BH.") || p.Contains("*")).ToList() ?? new List<string>();
            List<string> propertyExceptionsToParse = comparisonConfig.PropertyExceptions?.Where(p => !p.StartsWith("BH.") || p.Contains("*")).ToList() ?? new List<string>();
            List<string> propertyNumericTolerancesToParse = comparisonConfig.PropertyNumericTolerances?.Select(p => p.Name).Where(p => !p.StartsWith("BH.") || p.Contains("*")).ToList() ?? new List<string>();

            // If all the property names are already in FullName form and without wildcards, return.
            if (!propertiesToConsiderToParse.Any() && !propertyExceptionsToParse.Any() && !propertyNumericTolerancesToParse.Any())
                return;

            // Collect all property FullNames for this Type of object. This operation also caches results if the same Type is encountered in the same session.
            HashSet<string> allPropertiesFullNames = Query.GetAllPropertyFullNames(type, comparisonConfig.MaxNesting, true);

            // Check if we have some cached results for this comparisonConfig and type combination.
            // We cache separately the processed propertiesToConsider, propertyExceptions and propertyNumericTolerances to get optimal performance/versatility.

            // Check if we already have encountered and cached this same object Type and PropertiesToConsider.
            string propertiesToConsiderCacheKey = $"{type.FullName}:{string.Join(",", comparisonConfig.PropertiesToConsider)}";
            bool processPropertiesToConsider = propertiesToConsiderToParse.Any() && (!cache || !m_cachedPropertiesToConsider.TryGetValue(propertiesToConsiderCacheKey, out propertiesToConsider_fullNames));

            // Check if we already have encountered and cached this same object Type and PropertyExceptions.
            string propertyExceptionsCacheKey = $"{type.FullName}:{string.Join(",", comparisonConfig.PropertyExceptions)}";
            bool processPropertiesExceptions = propertyExceptionsToParse.Any() && (!cache || !m_cachedPropertyExceptions.TryGetValue(propertyExceptionsCacheKey, out propertyExceptions_fullNames));

            // Check if we already have encountered and cached this same object Type and PropertyNumericTolerances.
            string propertyNumericTolerancesCacheKey = $"{type.FullName}:{string.Join(",", comparisonConfig.PropertyNumericTolerances?.Select(ct => ct.Name + ct.Tolerance))}";
            bool processPropertyNumericTolerances = propertyNumericTolerancesToParse.Any() && (!cache || !m_cachedPropertyNumericTolerances.TryGetValue(propertyNumericTolerancesCacheKey, out propertyNumericTolerances_fullNames));

            // Safety clauses because C#'s Dictionary TryGetValue sets the out variable to `null` if it failed.
            propertiesToConsider_fullNames = propertiesToConsider_fullNames ?? new List<string>();
            propertyExceptions_fullNames = propertyExceptions_fullNames ?? new List<string>();
            propertyNumericTolerances_fullNames = propertyNumericTolerances_fullNames ?? new HashSet<NamedNumericTolerance>();

            // Iterate all of the input Type's propertyFullNames and see if they match with the propertiesToConsiderToParse, propertyExceptionsToParse and/or propertyNumericTolerancesToParse.
            foreach (var propertyFullName in allPropertiesFullNames)
            {
                if (processPropertiesToConsider)
                {
                    foreach (string propToConsider in propertiesToConsiderToParse)
                        if (IsMatchingInclusion(propertyFullName, propToConsider))
                            propertiesToConsider_fullNames.Add(propertyFullName);

                    if (cache) m_cachedPropertiesToConsider[propertiesToConsiderCacheKey] = propertiesToConsider_fullNames;
                }

                if (processPropertiesExceptions)
                {
                    foreach (var propException in propertyExceptionsToParse)
                        if (IsMatchingInclusion(propertyFullName, propException))
                            propertyExceptions_fullNames.Add(propertyFullName);

                    if (cache) m_cachedPropertyExceptions[propertyExceptionsCacheKey] = propertyExceptions_fullNames;
                }

                if (processPropertyNumericTolerances)
                {
                    foreach (var propNumericTolerance in propertyNumericTolerancesToParse)
                        if (IsMatchingInclusion(propertyFullName, propNumericTolerance))
                            propertyNumericTolerances_fullNames.Add(new NamedNumericTolerance() { Name = propertyFullName, Tolerance = comparisonConfig.PropertyNumericTolerances.Where(pnc => pnc.Name == propNumericTolerance).First().Tolerance });

                    if (cache) m_cachedPropertyNumericTolerances[propertyNumericTolerancesCacheKey] = propertyNumericTolerances_fullNames;
                }
            }

            // Add the results to the ComparisonConfig.
            comparisonConfig.PropertiesToConsider.AddRange(propertiesToConsider_fullNames);
            comparisonConfig.PropertyExceptions.AddRange(propertyExceptions_fullNames);
            comparisonConfig.PropertyNumericTolerances.UnionWith(propertyNumericTolerances_fullNames);

            m_ComparisonConfig_Type_processed[new Tuple<Type, object>(type, comparisonConfig)] = comparisonConfig;
        }

        /***************************************************/

        [Description("Parse the ComparisonConfig's property-based configs (from the `PropertiesToConsider`, `PropertyExceptions`, etc.), and get them all as Full Names." +
            "This allows to make sure we collect all relevant properties, even if we are given partial names or wildcards (e.g. `Start.*.X`).")]
        [Input("comparisonConfig", "ComparisonConfig object whose `PropertiesToConsider` and `PropertyExceptions` will be parsed." +
            "If they contain partial names or wildcards, they will be expanded with all the matching property FullNames found for the specified Type.")]
        [Input("obj", "Object whose properties will be collected and matched against the comparisonConfig's `PropertiesToConsider`, `PropertyExceptions`, and the other property-based configs.")]
        public static void PropertyNamesToFullNames(this BaseComparisonConfig comparisonConfig, object obj)
        {
            // Null checks.
            if (comparisonConfig == null || obj == null)
                return;

            // We need to execute this method only if we have properties in the ComparisonConfig that have NOT been specified in FullName form, or that contain wildcards.
            List<string> propertiesToConsiderToParse = comparisonConfig.PropertiesToConsider?.Where(p => !p.StartsWith("BH.") || p.Contains("*")).ToList() ?? new List<string>();
            List<string> propertyExceptionsToParse = comparisonConfig.PropertyExceptions?.Where(p => !p.StartsWith("BH.") || p.Contains("*")).ToList() ?? new List<string>();
            List<string> propertyNumericTolerancesToParse = comparisonConfig.PropertyNumericTolerances?.Select(p => p.Name).Where(p => !p.StartsWith("BH.") || p.Contains("*")).ToList() ?? new List<string>();

            // If all the property names in the ComparisonConfig are already in FullName form and without wildcards, return.
            if (!propertiesToConsiderToParse.Any() && !propertyExceptionsToParse.Any() && !propertyNumericTolerancesToParse.Any())
                return;

            // Result variables.
            List<string> propertiesToConsider_fullNames = new List<string>();
            List<string> propertyExceptions_fullNames = new List<string>();
            HashSet<NamedNumericTolerance> propertyNumericTolerances_fullNames = new HashSet<NamedNumericTolerance>();

            // Collect all property FullNames for this Type of object. This operation also caches results if the same Type is encountered in the same session.
            HashSet<string> allPropertiesFullNames = Query.GetAllPropertyFullNames(obj, comparisonConfig.MaxNesting);

            // Iterate all of the input Type's propertyFullNames and see if they match with the propertiesToConsiderToParse, propertyExceptionsToParse and/or propertyNumericTolerancesToParse.
            foreach (var propertyFullName in allPropertiesFullNames)
            {
                foreach (string propToConsider in propertiesToConsiderToParse)
                    if (IsMatchingInclusion(propertyFullName, propToConsider))
                        propertiesToConsider_fullNames.Add(propertyFullName);


                foreach (var propException in propertyExceptionsToParse)
                    if (IsMatchingInclusion(propertyFullName, propException))
                        propertyExceptions_fullNames.Add(propertyFullName);


                foreach (var propNumericTolerance in propertyNumericTolerancesToParse)
                    if (IsMatchingInclusion(propertyFullName, propNumericTolerance))
                        propertyNumericTolerances_fullNames.Add(new NamedNumericTolerance() { Name = propertyFullName, Tolerance = comparisonConfig.PropertyNumericTolerances.Where(pnc => pnc.Name == propNumericTolerance).First().Tolerance });
            }

            // Add the results to the ComparisonConfig.
            comparisonConfig.PropertiesToConsider = comparisonConfig.PropertiesToConsider?.Concat(propertiesToConsider_fullNames).ToList() ?? propertiesToConsider_fullNames;
            comparisonConfig.PropertyExceptions = comparisonConfig.PropertyExceptions?.Concat(propertyExceptions_fullNames).ToList() ?? propertyExceptions_fullNames;
            comparisonConfig.PropertyNumericTolerances = comparisonConfig.PropertyNumericTolerances == null ?
                new HashSet<NamedNumericTolerance>(comparisonConfig.PropertyNumericTolerances?.Union(propertyNumericTolerances_fullNames))
                : propertyNumericTolerances_fullNames;
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
            // and it also does NOT have a starting wildcard (i.e. "Start.*.X" and not "*.Start.*.X"),
            // a wildcard must be prepended to make sure we match all possible properties.
            if (!inclusionOrExclusion.StartsWith("BH") && !inclusionOrExclusion.StartsWith("*"))
                inclusionOrExclusion = $"*{inclusionOrExclusion}";

            // Finds if there is a match.
            return Query.WildcardMatch(propertyFullName, inclusionOrExclusion);
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        // We cache separately the processed propertiesToConsider, propertyExceptions and propertyNumericTolerances to get optimal performance/versatility.
        private static Dictionary<Tuple<Type, object>, object> m_ComparisonConfig_Type_processed = new Dictionary<Tuple<Type, object>, object>();
        private static Dictionary<string, List<string>> m_cachedPropertiesToConsider = new Dictionary<string, List<string>>();
        private static Dictionary<string, List<string>> m_cachedPropertyExceptions = new Dictionary<string, List<string>>();
        private static Dictionary<string, HashSet<NamedNumericTolerance>> m_cachedPropertyNumericTolerances = new Dictionary<string, HashSet<NamedNumericTolerance>>();
    }
}



