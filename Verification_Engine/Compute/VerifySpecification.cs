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
        /**** Public Methods                            ****/
        /***************************************************/

        // Engineer to Order workflow. Verify against specs.
        public static SpecificationResult IVerifySpecification(IEnumerable<object> objects, ISpecification specification)
        {
            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(objects, nameof(VerifySpecification), new object[] { specification }, out result))
            {
                //TODO: error
                return null;
            }

            return (SpecificationResult)result;
        }

        /***************************************************/

        public static SpecificationResult VerifySpecification(IEnumerable<object> objects, Specification specification)
        {
            // Extract the objects to verify
            List<object> extracted = objects.IExtract(specification.Extraction);

            // Then apply the check to the extracted objects
            List<RequirementResult> requirementResults = extracted.SelectMany(x => specification.Requirements.Select(y => IVerifyRequirement(x, y))).ToList();

            // Finally return the result
            return new SpecificationResult(specification, requirementResults);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        // Fallback
        private static SpecificationResult VerifySpecification(List<object> objects, ISpecification specification)
        {
            BH.Engine.Base.Compute.RecordError($"Specification of type {specification.GetType()} is currently not supported.");
            return null;
        }

        /***************************************************/
    }
}
