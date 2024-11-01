using BH.Engine.Base.Objects;
using BH.oM.Base;
using BH.oM.Verification;
using BH.oM.Verification.Conditions;
using System;

namespace BH.Engine.Verification
{
    public static partial class Compute
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

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
