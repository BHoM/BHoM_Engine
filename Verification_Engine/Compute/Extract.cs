using BH.oM.Verification.Conditions;
using BH.oM.Verification.Extraction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Verification
{
    public static partial class Compute
    {
        public static List<object> IExtract(this IEnumerable<object> objects, IExtraction extraction)
        {
            if (objects == null)
            {
                //TODO: error
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
                //error
            }

            return filtered as List<object>;
        }

        public static List<object> Extract(this IEnumerable<object> objects, ConditionBasedFilter extraction)
        {
            return objects.Where(x => x.IPasses(extraction.Condition) == true).ToList();
        }

        private static List<object> Extract(this IEnumerable<object> objects, IExtraction extraction)
        {
            throw new NotImplementedException();
        }
    }
}
