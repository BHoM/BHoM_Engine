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

using BH.Engine.Base.Objects;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Verification;
using BH.oM.Verification.Conditions;
using System;
using System.ComponentModel;

namespace BH.Engine.Verification
{
    public static partial class Compute
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Compares two values using the provided comparison requirement and tolerance.")]
        [Input("value", "Value to compare against the reference value.")]
        [Input("referenceValue", "Reference value to compare the value against.")]
        [Input("comparisonType", "Comparison requirement, i.e. whether the value should be equal, greater, less than reference value etc.")]
        [Input("tolerance", "Tolerance to apply in the comparison.")]
        [Output("result", "True if comparison of the input values meets the comparison requirement, otherwise false. Null in case of inconclusive comparison.")]
        public static bool? CompareValues(this object value, object referenceValue, ValueComparisonType comparisonType, object tolerance)
        {
            // Basic cases (check for nullity)
            if (referenceValue == null && value == null)
                return true;
            else if (referenceValue == null || value == null)
                return false;

            if (value is Type && referenceValue is Type)
                return value == referenceValue;

            // Try enum comparison
            if (value is Enum || referenceValue is Enum)
                return value.GetType() == referenceValue.GetType() && (int)value == (int)referenceValue;

            // Try a numerical comparison
            double numericalValue;
            if (double.TryParse(value?.ToString(), out numericalValue))
            {
                double referenceNumValue;
                double.TryParse(referenceValue?.ToString(), out referenceNumValue);

                double numTolerance;
                if (!double.TryParse(tolerance?.ToString(), out numTolerance))
                    numTolerance = 1e-03;

                return NumericalComparison(numericalValue, referenceNumValue, numTolerance, comparisonType);
            }

            // Try string comparison
            if (value is string && referenceValue is string)
                return StringComparison((string)value, (string)referenceValue, comparisonType);

            // Consider some other way to compare objects.
            if (comparisonType == ValueComparisonType.EqualTo || comparisonType == ValueComparisonType.NotEqualTo)
            {
                bool? passed;

                // If the referenceValue is a Type, convert this ValueCondition to a IsOfType condition.
                if (referenceValue is Type)
                {
                    IsOfType typeCondition = new IsOfType() { Type = referenceValue as Type };
                    passed = value.IPasses(typeCondition);
                }
                else
                    passed = CompareObjectEquality(value, referenceValue, tolerance);

                if (passed != null && comparisonType == ValueComparisonType.NotEqualTo)
                    passed = !passed;

                return passed;
            }

            BH.Engine.Base.Compute.RecordError("Objects could not be compared because no meaningful comparison method has been found.");
            return null;
        }


        /***************************************************/
        /****              Private Methods              ****/
        /***************************************************/

        private static bool CompareObjectEquality(object value, object refValue, object tolerance)
        {
            if (value == null || refValue == null)
                return value == refValue;

            if (value.GetType() != refValue.GetType())
                return false;

            var cc = tolerance as ComparisonConfig;
            if (cc != null)
            {
                HashComparer<object> hc = new HashComparer<object>(cc);
                return hc.Equals(value, refValue);
            }

            return value.Equals(refValue);
        }

        /***************************************************/

        private static bool NumericalComparison(double value, double referenceValue, double tolerance, ValueComparisonType condition)
        {
            switch (condition)
            {
                case ValueComparisonType.EqualTo:
                    return (Math.Abs(value - referenceValue) <= tolerance);
                case ValueComparisonType.NotEqualTo:
                    return (Math.Abs(value - referenceValue) > tolerance);
                case ValueComparisonType.GreaterThan:
                    return (value - referenceValue > tolerance);
                case ValueComparisonType.GreaterThanOrEqualTo:
                    return (value - referenceValue >= -tolerance);
                case ValueComparisonType.LessThan:
                    return (value - referenceValue < -tolerance);
                case ValueComparisonType.LessThanOrEqualTo:
                    return (value - referenceValue <= tolerance);
                default:
                    return false;
            }
        }

        /***************************************************/

        private static bool StringComparison(string value, string referenceValue, ValueComparisonType condition)
        {
            switch (condition)
            {
                case ValueComparisonType.EqualTo:
                    return value == referenceValue;
                case ValueComparisonType.NotEqualTo:
                    return value != referenceValue;
                case ValueComparisonType.Contains:
                    return value.Contains(referenceValue);
                case ValueComparisonType.StartsWith:
                    return value.StartsWith(referenceValue);
                case ValueComparisonType.EndsWith:
                    return value.EndsWith(referenceValue);
                default:
                    {
                        return false;
                    }
            }
        }

        /***************************************************/
    }
}
