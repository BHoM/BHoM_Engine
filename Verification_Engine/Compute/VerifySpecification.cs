using BH.oM.Base;
using BH.oM.Verification.Results;
using BH.oM.Verification.Specifications;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Verification
{
    public static partial class Compute
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        public static SpecificationResult IVerifySpecification(this IEnumerable<object> objects, ISpecification specification)
        {
            if (objects == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify specification against null objects.");
                return null;
            }

            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(objects, nameof(VerifySpecification), new object[] { specification }, out result))
            {
                BH.Engine.Base.Compute.RecordError($"Verification failed because specification of type {result.GetType().Name} is currently not supported.");
                return null;
            }

            return (SpecificationResult)result;
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        public static SpecificationResult VerifySpecification(this IEnumerable<object> objects, Specification specification)
        {
            // Extract the objects to verify
            List<object> extracted = objects.IExtract(specification.Extraction);

            // Then apply the check to the extracted objects
            List<RequirementResult> requirementResults = extracted.SelectMany(x => specification.Requirements.Select(y => IVerifyRequirement(x, y))).ToList();

            // Finally return the result
            return new SpecificationResult(specification, requirementResults);
        }

        /***************************************************/
    }
}
