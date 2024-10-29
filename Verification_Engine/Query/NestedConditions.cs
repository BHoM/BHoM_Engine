using BH.oM.Verification.Conditions;
using BH.oM.Verification.Results;
using BH.oM.Verification.Specifications;
using System;
using System.Collections.Generic;
using System.Text;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        public static IEnumerable<ICondition> INestedConditions(this ICondition condition)
        {
            return NestedConditions(condition as dynamic);
        }

        public static IEnumerable<ICondition> NestedConditions(this ILogicalCollectionCondition condition)
        {
            foreach (ICondition c in condition.Conditions)
            {
                foreach (ICondition nested in c.INestedConditions())
                {
                    yield return nested;
                }
            }
        }

        public static IEnumerable<ICondition> NestedConditions(this ISingleLogicalCondition condition)
        {
            yield return condition.Condition;
        }

        private static IEnumerable<ICondition> NestedConditions(this ICondition condition)
        {
            yield return condition;
        }
    }
}
