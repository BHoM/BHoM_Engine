using BH.oM.Verification.Conditions;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        public static bool? IPasses(this object obj, ICondition condition)
        {
            if (condition is IsNotNull notNullCondition)
                return Passes(obj, notNullCondition);

            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition because it was null.");
                return null;
            }

            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(obj, nameof(Passes), new object[] { condition }, out result))
            {
                BH.Engine.Base.Compute.RecordError($"Condition check failed because condition of type {condition.GetType().Name} is currently not supported.");
                return null;
            }

            return (bool?)result;
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        public static bool? Passes(this object obj, IValueCondition condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition because it was null.");
                return null;
            }

            return obj.IVerifyCondition(condition).Passed;
        }

        /***************************************************/

        public static bool? Passes(this object obj, IsOfType condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition because it was null.");
                return null;
            }

            return obj.VerifyCondition(condition).Passed;
        }

        /***************************************************/

        public static bool? Passes(this object obj, HasId condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition because it was null.");
                return null;
            }

            return obj.VerifyCondition(condition).Passed;
        }

        /***************************************************/

        public static bool? Passes(this object obj, IsNotNull condition)
        {
            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition because it was null.");
                return null;
            }

            return obj != null;
        }

        /***************************************************/

        public static bool? Passes(this object obj, LogicalNotCondition condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition because it was null.");
                return null;
            }

            return !obj.IPasses(condition.Condition);
        }

        /***************************************************/

        public static bool? Passes(this object obj, LogicalAndCondition condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition because it was null.");
                return null;
            }

            List<bool?> subResults = condition.Conditions.Select(x => obj.IPasses(x)).ToList();
            if (subResults.Any(x => x == null))
                return null;
            else
                return subResults.All(x => x == true);
        }

        /***************************************************/

        public static bool? Passes(this object obj, LogicalOrCondition condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition because it was null.");
                return null;
            }

            return condition.Conditions.Any(x => obj.IPasses(x) == true);
        }

        /***************************************************/
    }
}
