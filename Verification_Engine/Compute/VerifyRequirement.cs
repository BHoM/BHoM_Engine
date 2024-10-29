using BH.oM.Base;
using BH.oM.Verification.Requirements;
using BH.oM.Verification.Results;
using System.Linq;

namespace BH.Engine.Verification
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static RequirementResult IVerifyRequirement(object obj, IRequirement requirement)
        {
            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(obj, nameof(VerifyRequirement), new object[] { requirement }, out result))
            {
                //TODO: error
                return null;
            }

            return (RequirementResult)result;
        }

        /***************************************************/

        public static RequirementResult VerifyRequirement(IBHoMObject obj, Requirement requirement)
        {
            if (requirement == null || requirement.Condition.INestedConditions().Any(x => x == null))
            {
                BH.Engine.Base.Compute.RecordError($"Requirement {requirement.Name} is null or its condition constains nulls.");
                return null;
            }

            IConditionResult conditionResult = obj.IVerifyCondition(requirement.Condition);
            return new RequirementResult(requirement.BHoM_Guid, obj.IIdentifier(), conditionResult);
        }

        /***************************************************/
    }
}
