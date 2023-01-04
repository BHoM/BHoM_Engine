/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Extract the numeric tolerance from the ComparisonConfig that matches the given propertyFullName." +
            "If a CustomTolerance match is found for this property Full Name, then return it. " +
            "If multiple matches are found, return the most sensistive tolerance among the matches. " +
            "If no match is found, return the most sensitive tolerance (double.MinValue).")]
        [Input("comparisonConfig", "Comparison Config from where tolerance information should be extracted.")]
        [Input("propertyFullName", "Full name (path) of the property for which we want to extract the numerical Tolerance.")]
        public static double NumericTolerance(this BaseComparisonConfig comparisonConfig, string propertyFullName)
        {
            comparisonConfig = comparisonConfig ?? new ComparisonConfig();

            return NumericTolerance(comparisonConfig.PropertyNumericTolerances, comparisonConfig.NumericTolerance, propertyFullName, false);
        }

        /***************************************************/

        [Description("Extract the numeric tolerance from the ComparisonConfig that matches the given propertyFullName." +
            "If a CustomTolerance match is found for this property Full Name, then return it. " +
            "If multiple matches are found, return the most sensistive tolerance among the matches. " +
            "If no match is found, return the most sensitive tolerance (double.MinValue).")]
        [Input("namedNumericTolerances", "Custom numeric tolerances associated with a certain name, to be matched with the `fullName` input.")]
        [Input("globalNumericTolerance", "Fallback numeric tolerance to be used when no named match is found.")]
        [Input("fullName", "Full name of the property or object for which we want to extract the numerical Tolerance. This name will be seeked in the `namedNumericTolerances` input.")]
        [Input("getGlobalSmallest", "(Optional, defaults to false) If true, extract the smallest (most precise) tolerance amongst all PropertyNumericTolerances for all properties. Setting this to true makes `propertyFullName` irrelevant.")]
        public static double NumericTolerance(this HashSet<NamedNumericTolerance> namedNumericTolerances, double globalNumericTolerance, string fullName = null, bool getGlobalSmallest = false)
        {
            // Initially set the result to an arbitrarily large number to perform the search for a smaller number, see loop.
            double tolerance = double.MaxValue;

            // Take care of custom NumericTolerances.
            if (!string.IsNullOrWhiteSpace(fullName) && (namedNumericTolerances?.Any() ?? false))
            {
                // Iterate the specified custom NumericTolerances. If more than one match with the current property is found, take the safest (smallest) value.
                List<string> matchingCustomTolerancesNames = new List<string>();
                foreach (var pnt in namedNumericTolerances)
                {
                    if (getGlobalSmallest || fullName == pnt.Name || fullName.EndsWith($".{pnt.Name}") || fullName.WildcardMatch(pnt.Name))
                    {
                        if (!getGlobalSmallest)
                            matchingCustomTolerancesNames.Add(pnt.Name);

                        if (pnt.Tolerance < tolerance)
                            tolerance = pnt.Tolerance;
                    }
                }

                if (matchingCustomTolerancesNames.Count > 1)
                    BH.Engine.Base.Compute.RecordWarning($"The property `{fullName}` matched with {matchingCustomTolerancesNames.Count} {nameof(namedNumericTolerances)}: `{string.Join("`, ", matchingCustomTolerancesNames)}`." +
                    $"\nThe most sensitive tolerance was picked (smallest value): {tolerance}.");
            }

            // If no matching CustomTolerance was found, return the globalNumericTolerance.
            if (tolerance == double.MaxValue)
                tolerance = globalNumericTolerance;

            return tolerance;
        }
    }
}

