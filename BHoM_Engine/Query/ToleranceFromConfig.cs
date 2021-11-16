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
using BH.oM.Reflection.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Extract the numeric tolerance from the ComparisonConfig." +
            "If a CustomTolerance match is found for this property Full Name, then return it. " +
            "If multiple matches are found, return the most sensistive tolerance. " +
            "If no match is found, return the generic numeric tolerance.")]
        [Input("comparisonConfig", "Comparison Config from where tolerance information should be extracted.")]
        [Input("propertyFullName", "Full name (path) of the property for which we want to extract the numerical Tolerance.")]
        [Input("getGlobalSmallest", "(Optional, defaults to false) If true, extract the smallest (most precise) tolerance amongst all CustomTolerances for all properties and the global numeric tolerance.")]
        public static double ToleranceFromConfig(this BaseComparisonConfig comparisonConfig, string propertyFullName)
        {
            return ToleranceFromConfig(comparisonConfig, propertyFullName, false);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Extract the numeric tolerance from the ComparisonConfig." +
            "If a CustomTolerance match is found for this property Full Name, then return it. " +
            "If multiple matches are found, return the most sensistive tolerance. " +
            "If no match is found, return the generic numeric tolerance.")]
        [Input("comparisonConfig", "Comparison Config from where tolerance information should be extracted.")]
        [Input("propertyFullName", "Full name (path) of the property for which we want to extract the numerical Tolerance.")]
        [Input("getGlobalSmallest", "(Optional, defaults to false) If true, extract the smallest (most precise) tolerance amongst all CustomTolerances for all properties and the global numeric tolerance. Setting this to true makes `propertyFullName` irrelevant.")]
        private static double ToleranceFromConfig(this BaseComparisonConfig comparisonConfig, string propertyFullName, bool getGlobalSmallest = false)
        {
            double tolerance = double.MaxValue;

            // Take care of CustomTolerances.
            if (comparisonConfig.CustomTolerances?.Any() ?? false)
            {
                // Iterate the specified CustomTolerances. If more than one match with the current property is found, take the safest (smallest) value.
                List<string> matchingCustomTolerancesNames = new List<string>();
                foreach (var ct in comparisonConfig.CustomTolerances)
                {
                    if (getGlobalSmallest || propertyFullName.EndsWith($".{ct.Name}") || new WildcardPattern(ct.Name).IsMatch(propertyFullName))
                    {
                        if (!getGlobalSmallest)
                            matchingCustomTolerancesNames.Add(ct.Name);

                        if (ct.Tolerance < tolerance)
                            tolerance = ct.Tolerance;
                    }
                }

                if (matchingCustomTolerancesNames.Count > 1)
                    BH.Engine.Reflection.Compute.RecordWarning($"The property `{propertyFullName}` matched with {matchingCustomTolerancesNames.Count} CustomTolerances: `{string.Join("`, ", matchingCustomTolerancesNames)}`." +
                    $"\nThe most sensitive tolerance was picked (smallest value): {tolerance}.");
            }

            // If no matching CustomTolerance was found, set the tolerance to be the general tolerance.
            if (tolerance == double.MaxValue)
                tolerance = comparisonConfig.NumericTolerance;

            if (getGlobalSmallest && tolerance > comparisonConfig.NumericTolerance)
                tolerance = comparisonConfig.NumericTolerance;

            return tolerance;
        }
    }
}