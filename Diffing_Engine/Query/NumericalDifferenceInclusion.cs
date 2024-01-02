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
using System.ComponentModel;
using BH.oM.Base.Attributes;
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
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Given two numbers, checks if their difference is to be considered as relevant under the input ComparisonConfig settings.")]
        [Input("number1", "First number to compare.")]
        [Input("number2", "Second number to compare.")]
        [Input("propertyFullName", "If the numbers are part of an object, full name of the property that owns them. This name will be used to seek matches in the ComparisoConfig named numeric tolerance/significant figures.")]
        [Input("comparisonConfig", "Object containing the settings for this numerical comparison.")]
        [Output("seenAsDifferent", "Whether the input numbers are seen as different, given the numerical approximations specified in the comparisonConfig.")]
        public static bool NumericalDifferenceInclusion(this object number1, object number2, string propertyFullName = null, BaseComparisonConfig comparisonConfig = null)
        {
            comparisonConfig = comparisonConfig ?? new ComparisonConfig();

            return NumericalDifferenceInclusion(number1, number2, propertyFullName, comparisonConfig.PropertyNumericTolerances, comparisonConfig.NumericTolerance, comparisonConfig.PropertySignificantFigures, comparisonConfig.SignificantFigures);
        }

        /***************************************************/

        [Description("Given two numbers, checks if their difference is to be considered as relevant under the input ComparisonConfig settings.")]
        [Input("number1", "First number to compare.")]
        [Input("number2", "Second number to compare.")]
        [Input("fullName", "If the numbers are part of an object, full name of the property that owns them. This name will be used to seek matches in the `customNumericTolerances`/`customSignificantFigures` sets.")]
        [Input("namedNumericTolerances", "Custom numeric tolerances associated with a certain name, to be matched with the `fullName` input.")]
        [Input("globalNumericTolerance", "Fallback numeric tolerance to be used when no named match is found.")]
        [Input("namedSignificantFigures", "Custom significant figures associated with a certain name, to be matched with the `fullName` input.")]
        [Input("globalSignificantFigures", "Fallback significant figures to be used when no named match is found.")]
        [Output("seenAsDifferent", "Whether the input numbers are seen as different, given the numerical approximations specified.")]
        public static bool NumericalDifferenceInclusion(this object number1, object number2, string fullName,
            HashSet<NamedNumericTolerance> namedNumericTolerances = null,
            double globalNumericTolerance = double.MinValue,
            HashSet<NamedSignificantFigures> namedSignificantFigures = null,
            int globalSignificantFigures = int.MaxValue)
        {
            // Check if we specified CustomTolerances and if this difference is a number difference.
            if ((globalNumericTolerance != double.MinValue || globalSignificantFigures != int.MaxValue
                || (namedNumericTolerances?.Any() ?? false) || (namedSignificantFigures?.Any() ?? false))
                && (number1?.GetType().IsNumeric(false) ?? false) && (number2?.GetType().IsNumeric(false) ?? false)) // GetType() is slow; call only after checking that any custom tolerance is present.
            {
                // We have specified some custom tolerance in the ComparisonConfig AND this property difference is numeric.
                // Because we have set Kellerman to retrieve any possible numerical variation,
                // we now want to "filter out" number variations following our BHoM ComparisonConfig settings.
                if (globalNumericTolerance!= double.MinValue || (namedNumericTolerances?.Any() ?? false))
                {
                    double toleranceToUse = BH.Engine.Base.Query.NumericTolerance(namedNumericTolerances, globalNumericTolerance, fullName);
                    if (toleranceToUse != double.MinValue)
                    {
                        double value1 = double.Parse(number1.ToString());
                        double value2 = double.Parse(number2.ToString());

                        double difference = Math.Abs(value1 - value2);

                        // If the difference is less than the Tolerance, it means that we do not want to consider this Difference. Skip.
                        return difference > toleranceToUse;
                    }
                }

                if (globalSignificantFigures != int.MaxValue || (namedSignificantFigures?.Any() ?? false))
                {
                    int significantFiguresToUse = BH.Engine.Base.Query.SignificantFigures(namedSignificantFigures, globalSignificantFigures, fullName);
                    if (significantFiguresToUse != int.MaxValue)
                    {
                        double value1 = double.Parse(number1.ToString()).RoundToSignificantFigures(significantFiguresToUse);
                        double value2 = double.Parse(number2.ToString()).RoundToSignificantFigures(significantFiguresToUse);

                        // If, once rounded, the numbers are the same, it means that we do not want to consider this Difference. Skip.
                        return value1 != value2;
                    }
                }
            }

            return true;
        }
    }
}
