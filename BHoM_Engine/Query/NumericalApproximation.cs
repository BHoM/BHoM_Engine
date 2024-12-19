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

using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using BH.oM.Base.Attributes;
using System.ComponentModel;
using BH.Engine.Base;
using System.Collections;
using System.Data;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Compute the approximation of a floating-point number for its comparison with other numbers, given specific ComparisonConfig settings.")]
        [Input("number", "Number to approximate.")]
        [Input("fullName", "Name of the number or of the property that holds this number. This name will be used to seek any matching custom tolerance/significant figure to apply for this approximation in the `comparisonConfig` input.")]
        [Input("comparisonConfig", "Object that stores the settings that will used for the approximation.")]
        public static double NumericalApproximation(this double number, string fullName = null, BaseComparisonConfig comparisonConfig = null)
        {
            comparisonConfig = comparisonConfig ?? new ComparisonConfig();

            return NumericalApproximation(number, fullName, comparisonConfig.PropertyNumericTolerances, comparisonConfig.NumericTolerance, comparisonConfig.PropertySignificantFigures, comparisonConfig.SignificantFigures);
        }

        /***************************************************/

        [Description("Compute the approximation of an integer number for its comparison with other numbers, given specific ComparisonConfig settings.")]
        [Input("number", "Number to approximate.")]
        [Input("fullName", "Name of the number or of the property that holds this number. This name will be used to seek any matching custom tolerance/significant figure to apply for this approximation in the `comparisonConfig` input.")]
        [Input("comparisonConfig", "Object that stores the settings that will used for the approximation.")]
        public static double NumericalApproximation(this int number, string fullName = null, BaseComparisonConfig comparisonConfig = null)
        {
            comparisonConfig = comparisonConfig ?? new ComparisonConfig();

            return NumericalApproximation(number, fullName, comparisonConfig.PropertySignificantFigures, comparisonConfig.SignificantFigures);
        }

        /***************************************************/

        [Description("Compute the approximation of a floating-point number given specific settings.")]
        [Input("number", "Floating-point number to approximate.")]
        [Input("fullName", "Name of the number or of the property that holds this number. This name will be used to seek any matching custom tolerance/significant figure to apply for this approximation.")]
        [Input("customNumericTolerances", "Named custom numeric tolerances to be used for this approximation. If the `fullName` input matches any of these, it will be used. Wildcards are supported.")]
        [Input("globalNumericTolerance", "Fallback numeric tolerance to be used if no matching customNumericTolerance is found.")]
        [Input("customSignificantFigures", "Named custom significant figures to be used for this approximation. If the `fullName` input matches any of these, it will be used. Wildcards are supported.")]
        [Input("globalSignificantFigures", "Fallback significant figures to be used if no matching customNumericTolerance is found.")]
        public static double NumericalApproximation(this double number, string fullName = null,
            HashSet<NamedNumericTolerance> customNumericTolerances = null, double globalNumericTolerance = double.MinValue,
            HashSet<NamedSignificantFigures> customSignificantFigures = null, int globalSignificantFigures = int.MaxValue)
        {
            // If we didn't specify any custom tolerance, just return the input.
            if (globalNumericTolerance == double.MinValue && globalSignificantFigures == int.MaxValue
                && (!customNumericTolerances?.Any() ?? true) && (!customSignificantFigures?.Any() ?? true))
                return number;

            // Check if any 1) custom tolerance or 2) significant figures were specified.
            // We carry over the rounded number from the two steps for SignificantFigures/NumericTolerance approximation
            // so that, if multiple matches are found for this property, the most approximate (least precise) number is taken.
            // We arbitrarily apply the rounding from 1) and then 2) - generally rounding is more "coarse" with significant figures, so better to do it as 2nd step.

            // 1) Check NumericTolerances.
            if (globalNumericTolerance != double.MinValue || (customNumericTolerances?.Any() ?? false))
            {
                double tolerance = NumericTolerance(customNumericTolerances, globalNumericTolerance, fullName, false);
                if (tolerance != double.MaxValue)
                    number = Query.Round(number, tolerance);
            }

            // 2) Check significantFigures.
            if (globalSignificantFigures != int.MaxValue || (customSignificantFigures?.Any() ?? false))
            {
                // Find the SignificantFigures that should be applied for this property.
                int significantFigures = SignificantFigures(customSignificantFigures, globalSignificantFigures, fullName);
                if (significantFigures != int.MaxValue)
                    number = Query.RoundToSignificantFigures(number, significantFigures);
            }

            return number;
        }

        /***************************************************/

        [Description("Compute the approximation of an integer number given specific settings.")]
        [Input("number", "Integer number to approximate.")]
        [Input("fullName", "Name of the number or of the property that holds this number. This name will be used to seek any matching custom tolerance/significant figure to apply for this approximation.")]
        [Input("customSignificantFigures", "Named custom significant figures to be used for this approximation. If the `fullName` input matches any of these, it will be used. Wildcards are supported.")]
        [Input("globalSignificantFigures", "Fallback significant figures to be used if no matching customNumericTolerance is found.")]
        public static int NumericalApproximation(this int number, string fullName = null,
            HashSet<NamedSignificantFigures> customSignificantFigures = null, int globalSignificantFigures = int.MaxValue)
        {
            // If we didn't specify any custom tolerance, just return the input.
            if (globalSignificantFigures == int.MaxValue && (!customSignificantFigures?.Any() ?? true))
                return number;

            // Check if any significant figures were specified.
            if (globalSignificantFigures != int.MaxValue || (customSignificantFigures?.Any() ?? false))
            {
                // Find the SignificantFigures that should be applied for this property.
                int significantFigures = SignificantFigures(customSignificantFigures, globalSignificantFigures, fullName);
                if (significantFigures != int.MaxValue)
                    number = Query.RoundToSignificantFigures(number, significantFigures);
            }

            return number;
        }

        /***************************************************/
    }
}





