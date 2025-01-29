using BH.oM.Verification;
using System.Collections.Generic;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        public static bool IsTextComparisonType(this ValueComparisonType comparisonType)
        {
            return m_TextComparisonTypes.Contains(comparisonType);
        }

        private static readonly HashSet<ValueComparisonType> m_TextComparisonTypes = new HashSet<ValueComparisonType>
        {
            ValueComparisonType.EqualTo,
            ValueComparisonType.NotEqualTo,
            ValueComparisonType.Contains,
            ValueComparisonType.StartsWith,
            ValueComparisonType.EndsWith
        };
    }
}
