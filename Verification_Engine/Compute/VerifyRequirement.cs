using BH.oM.Base;
using BH.oM.Verification.Requirements;
using BH.oM.Verification.Results;
using System.Linq;

namespace BH.Engine.Verification
{
    public static partial class Compute
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        public static RequirementResult IVerifyRequirement(this object obj, IRequirement requirement)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify requirement against a null object.");
                return null;
            }

            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(obj, nameof(VerifyRequirement), new object[] { requirement }, out result))
            {
                BH.Engine.Base.Compute.RecordError($"Verification failed because requirement of type {result.GetType().Name} is currently not supported.");
                return null;
            }

            return result as RequirementResult;
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        public static RequirementResult VerifyRequirement(this IBHoMObject obj, Requirement requirement)
        {
            if (requirement == null || requirement.Condition.INestedConditions().Any(x => x == null))
            {
                BH.Engine.Base.Compute.RecordError($"Requirement {requirement.Name} is null or its condition contains nulls.");
                return null;
            }

            IConditionResult conditionResult = obj.IVerifyCondition(requirement.Condition);
            return new RequirementResult(requirement.BHoM_Guid, obj.IIdentifier(), conditionResult);
        }

        /***************************************************/
    }
}
