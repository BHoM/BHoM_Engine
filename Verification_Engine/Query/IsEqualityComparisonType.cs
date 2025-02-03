using BH.oM.Verification;
using System.Collections.Generic;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        public static bool IsEqualityComparisonType(this ValueComparisonType comparisonType)
        {
            return m_EqualityComparisonTypes.Contains(comparisonType);
        }

        private static readonly HashSet<ValueComparisonType> m_EqualityComparisonTypes = new HashSet<ValueComparisonType>
        {
            ValueComparisonType.EqualTo,
            ValueComparisonType.NotEqualTo
        };
    }
}
