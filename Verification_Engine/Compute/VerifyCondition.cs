using BH.Engine.Base;
using BH.Engine.Base.Objects;
using BH.oM.Base;
using BH.oM.Verification;
using BH.oM.Verification.Conditions;
using BH.oM.Verification.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace BH.Engine.Verification
{
    public static partial class Compute
    {
        public static IConditionResult IVerifyCondition(this object obj, ICondition condition)
        {
            if (condition is IsNotNull notNullCondition)
                return VerifyCondition(obj, notNullCondition);

            //TODO: null check

            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(obj, nameof(VerifyCondition), new object[] { condition }, out result))
            {
                //TODO: error
                return null;
            }

            return VerifyCondition(obj, condition as dynamic);
        }

        /***************************************************/

        public static SingleLogicalConditionResult VerifyCondition(this object obj, LogicalNotCondition condition)
        {
            IConditionResult result = obj.IVerifyCondition(condition.Condition);
            bool? inverted;
            if (result.Passed == null)
                inverted = null;
            else
                inverted = !(result.Passed.Value);

            return new SingleLogicalConditionResult(inverted, result);
        }

        /***************************************************/

        public static LogicalCollectionConditionResult VerifyCondition(this object obj, ILogicalCollectionCondition condition)
        {
            if (condition.Conditions.Count == 0)
                return new LogicalCollectionConditionResult(null, new List<IConditionResult>());

            List<IConditionResult> results = new List<IConditionResult>();
            foreach (ICondition f in condition.Conditions)
            {
                if (f == null)
                    continue;

                var r = obj.IVerifyCondition(f);
                results.Add(r);
            }

            bool? pass;
            if (condition is LogicalAndCondition)
            {
                if (results.Any(x => x == null))
                    pass = null;
                else
                    pass = results.All(x => x.Passed == true);
            }
            else if (condition is LogicalOrCondition)
                pass = results.Any(x => x.Passed == true);
            else
                throw new NotImplementedException();

            return new LogicalCollectionConditionResult(pass, results);
        }

        /***************************************************/

        public static ValueConditionResult VerifyCondition(this object obj, IsInDomain condition)
        {
            bool pass = false;

            object value = obj.ValueFromSource(condition);
            double numericalValue;
            double tolerance;
            double.TryParse(condition.Tolerance.ToString(), out tolerance);

            if (double.TryParse(value?.ToString(), out numericalValue))
                pass = Query.IsInDomain(numericalValue, condition.Domain, tolerance);
            else if (obj is DateTime)
            {
                DateTime? dt = obj as DateTime?;
                pass = Query.IsInDomain(dt.Value.Ticks, condition.Domain, tolerance);
            }

            return new ValueConditionResult(pass, value);
        }

        /***************************************************/

        public static ValueConditionResult VerifyCondition(this object obj, HasId condition)
        {
            object id = ((obj as IBHoMObject)?.Fragments.FirstOrDefault(x => x is IAdapterId) as IAdapterId)?.Id;
            if (id == null)
                id = ((obj as IBHoMObject)?.Fragments.FirstOrDefault(x => x is IPersistentAdapterId) as IPersistentAdapterId)?.PersistentId;

            bool? pass = id != null && condition.Ids.Contains(id);
            return new ValueConditionResult(pass, id);
        }

        /***************************************************/

        public static ValueConditionResult VerifyCondition(this object obj, IsInSet condition)
        {
            object value = obj.ValueFromSource(condition);
            bool? pass = false;
            if (value.IsInSet(condition.Set, condition.ComparisonConfig))
                pass = true;

            if (value is IEnumerable<object> ienumerable)
                pass = ienumerable.All(x => x.IsInSet(condition.Set, condition.ComparisonConfig));

            return new ValueConditionResult(pass, value);
        }

        /***************************************************/

        public static IsNotNullResult VerifyCondition(this object obj, IsNotNull condition)
        {
            bool? pass = obj.Passes(condition);
            return new IsNotNullResult(pass);
        }

        /***************************************************/

        public static IsOfTypeResult VerifyCondition(this object obj, IsOfType condition)
        {
            Type type = condition.Type is string ? BH.Engine.Base.Create.Type(condition.Type.ToString()) : condition.Type as Type;
            if (type == null)
                return new IsOfTypeResult(null, null);

            Type extractedType = obj.GetType();
            bool? pass = extractedType == type;
            return new IsOfTypeResult(pass, extractedType);
        }

        /***************************************************/

        public static ValueConditionResult VerifyCondition(this object obj, ValueCondition condition)
        {
            //TODO: null check

            object value = obj.ValueFromSource(condition);
            bool? pass = value.Passes(condition.ReferenceValue, condition.ComparisonType, condition.Tolerance);
            return new ValueConditionResult(pass, value);
        }

        /***************************************************/

        public static bool? Passes(this object value, object referenceValue, ValueComparisonType comparisonType, object tolerance)
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

            //TODO: meaningful error or handle more cases
            string error = "";
            BH.Engine.Base.Compute.RecordError(error);
            return null;
        }

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

        private static bool IsInSet(this object value, List<object> set, ComparisonConfig comparisonConfig)
        {
            if (comparisonConfig != null)
                return set.Contains(value, new HashComparer<object>(comparisonConfig));
            else
                return set.Contains(value);
        }

        /***************************************************/
    }
}
