using BH.oM.Verification.Conditions;
using System.Collections.Generic;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        public static IEnumerable<ICondition> INestedConditions(this ICondition condition)
        {
            return NestedConditions(condition as dynamic);
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

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

        /***************************************************/

        public static IEnumerable<ICondition> NestedConditions(this ISingleLogicalCondition condition)
        {
            yield return condition.Condition;
        }


        /***************************************************/
        /****              Private Methods              ****/
        /***************************************************/

        private static IEnumerable<ICondition> NestedConditions(this ICondition condition)
        {
            yield return condition;
        }

        /***************************************************/
    }
}
