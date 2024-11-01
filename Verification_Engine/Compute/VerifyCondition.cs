using BH.Engine.Base.Objects;
using BH.oM.Base;
using BH.oM.Verification.Conditions;
using BH.oM.Verification.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Verification
{
    public static partial class Compute
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        public static IConditionResult IVerifyCondition(this object obj, ICondition condition)
        {
            if (condition is IsNotNull notNullCondition)
                return VerifyCondition(obj, notNullCondition);

            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition because it was null.");
                return null;
            }

            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(obj, nameof(VerifyCondition), new object[] { condition }, out result))
            {
                BH.Engine.Base.Compute.RecordError($"Verification failed because condition of type {condition.GetType().Name} is currently not supported.");
                return null;
            }

            return result as IConditionResult;
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        public static SingleLogicalConditionResult VerifyCondition(this object obj, LogicalNotCondition condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition because it was null.");
                return null;
            }

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
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition because it was null.");
                return null;
            }

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
            {
                BH.Engine.Base.Compute.RecordError($"Verification failed because condition of type {condition.GetType().Name} is currently not supported.");
                return null;
            }

            return new LogicalCollectionConditionResult(pass, results);
        }

        /***************************************************/

        public static ValueConditionResult VerifyCondition(this object obj, IsInDomain condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition because it was null.");
                return null;
            }

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
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition because it was null.");
                return null;
            }

            object id = ((obj as IBHoMObject)?.Fragments.FirstOrDefault(x => x is IAdapterId) as IAdapterId)?.Id;
            if (id == null)
                id = ((obj as IBHoMObject)?.Fragments.FirstOrDefault(x => x is IPersistentAdapterId) as IPersistentAdapterId)?.PersistentId;

            bool? pass = id != null && condition.Ids.Contains(id);
            return new ValueConditionResult(pass, id);
        }

        /***************************************************/

        public static ValueConditionResult VerifyCondition(this object obj, IsInSet condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition because it was null.");
                return null;
            }

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
            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition because it was null.");
                return null;
            }

            bool? pass = obj.Passes(condition);
            return new IsNotNullResult(pass);
        }

        /***************************************************/

        public static IsOfTypeResult VerifyCondition(this object obj, IsOfType condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition because it was null.");
                return null;
            }

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
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition because it was null.");
                return null;
            }

            object value = obj.ValueFromSource(condition);
            bool? pass = value.CompareValues(condition.ReferenceValue, condition.ComparisonType, condition.Tolerance);
            return new ValueConditionResult(pass, value);
        }


        /***************************************************/
        /****              Private Methods              ****/
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
