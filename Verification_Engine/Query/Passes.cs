using BH.oM.Verification.Conditions;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        public static bool? IPasses(this object obj, ICondition condition)
        {
            if (condition is IsNotNull notNullCondition)
                return Passes(obj, notNullCondition);

            //TODO: null check

            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(obj, nameof(Passes), new object[] { condition }, out result))
            {
                //error
                return null;
            }

            return (bool)result;
        }

        public static bool? Passes(this object obj, IValueCondition condition)
        {
            return obj.IVerifyCondition(condition).Passed;
        }

        public static bool? Passes(this object obj, IsOfType condition)
        {
            return obj.VerifyCondition(condition).Passed;
        }

        public static bool? Passes(this object obj, HasId condition)
        {
            return obj.VerifyCondition(condition).Passed;
        }

        public static bool? Passes(this object obj, IsNotNull condition)
        {
            return obj != null;
        }

        public static bool? Passes(this object obj, LogicalNotCondition condition)
        {
            return !obj.IPasses(condition.Condition);
        }

        public static bool? Passes(this object obj, LogicalAndCondition condition)
        {
            List<bool?> subResults = condition.Conditions.Select(x => obj.IPasses(x)).ToList();
            if (subResults.Any(x => x == null))
                return null;
            else
                return subResults.All(x => x == true);
        }

        public static bool? Passes(this object obj, LogicalOrCondition condition)
        {
            return condition.Conditions.Any(x => obj.IPasses(x) == true);
        }
    }
}
