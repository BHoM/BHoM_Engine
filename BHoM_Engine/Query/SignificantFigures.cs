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

        [Description("Extract the significant figures from the ComparisonConfig that matches the given propertyFullName." +
            "If a CustomTolerance match is found for this property Full Name, then return it. " +
            "If multiple matches are found, return the most sensistive among the matches. " +
            "If no match is found, return `ComparisonConfig.SignificantFigures`.")]
        [Input("comparisonConfig", "Comparison Config from where tolerance information should be extracted.")]
        [Input("propertyFullName", "Full name (path) of the property for which we want to extract the numerical Tolerance.")]
        public static int SignificantFigures(this BaseComparisonConfig comparisonConfig, string propertyFullName)
        {
            comparisonConfig = comparisonConfig ?? new ComparisonConfig();

            return SignificantFigures(comparisonConfig.PropertySignificantFigures, comparisonConfig.SignificantFigures, propertyFullName);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Extract the significant figures from the ComparisonConfig that matches the given propertyFullName." +
            "If a CustomTolerance match is found for this property Full Name, then return it. " +
            "If multiple matches are found, return the most sensistive among the matches. " +
            "If no match is found, return `ComparisonConfig.SignificantFigures`.")]
        [Input("namedSignificantFigures", "Custom significant figures associated with a certain name, to be matched with the `fullName` input.")]
        [Input("globalSignificantFigures", "Fallback significant figures to be used when no named match is found.")]
        [Input("fullName", "Full name of the property or object for which we want to extract the significant figures. This name will be seeked in the `propertySignificantFigures` input.")]
        public static int SignificantFigures(this HashSet<NamedSignificantFigures> namedSignificantFigures, int globalSignificantFigures, string fullName = null)
        {
            // Initially set the result to an arbitrarily large number to perform the search for a smaller number, see loop.
            int significantFigures = int.MaxValue;

            // Take care of PropertyNumericTolerances.
            if (!string.IsNullOrWhiteSpace(fullName) && (namedSignificantFigures?.Any() ?? false))
            {
                // Iterate the specified PropertyNumericTolerances. If more than one match with the current property is found, take the safest (smallest) value.
                List<string> matchingCustomTolerancesNames = new List<string>();
                foreach (var pnt in namedSignificantFigures)
                {
                    if (fullName == pnt.Name || fullName.EndsWith($".{pnt.Name}") || fullName.WildcardMatch(pnt.Name))
                    {
                        matchingCustomTolerancesNames.Add(pnt.Name);

                        if (pnt.SignificantFigures < significantFigures)
                            significantFigures = pnt.SignificantFigures;
                    }
                }

                if (matchingCustomTolerancesNames.Count > 1)
                    BH.Engine.Base.Compute.RecordWarning($"The property `{fullName}` matched with {matchingCustomTolerancesNames.Count} {nameof(namedSignificantFigures)}: `{string.Join("`, ", matchingCustomTolerancesNames)}`." +
                    $"\nThe most sensitive tolerance was picked (smallest value): {significantFigures}.");
            }

            // If no matching CustomTolerance was found, return the globalSignificantFigures.
            if (significantFigures == int.MaxValue)
                significantFigures = globalSignificantFigures;

            return significantFigures;
        }
    }
}


