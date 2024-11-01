using BH.oM.Verification.Extraction;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Verification
{
    public static partial class Compute
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        public static List<object> IExtract(this IEnumerable<object> objects, IExtraction extraction)
        {
            if (objects == null)
            {
                BH.Engine.Base.Compute.RecordError($"Extraction failed because the provided objects to extract from are null.");
                return null;
            }

            if (extraction == null)
            {
                BH.Engine.Base.Compute.RecordNote("No filter provided, all input objects have been verified against the requirements.");
                return objects.ToList();
            }

            object filtered;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(objects, nameof(Extract), new object[] { extraction }, out filtered))
            {
                BH.Engine.Base.Compute.RecordError($"Extraction failed because extraction type {extraction.GetType().Name} is currently not supported.");
                return null;
            }

            return filtered as List<object>;
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        public static List<object> Extract(this IEnumerable<object> objects, ConditionBasedFilter extraction)
        {
            return objects.Where(x => x.IPasses(extraction.Condition) == true).ToList();
        }

        /***************************************************/
    }
}
