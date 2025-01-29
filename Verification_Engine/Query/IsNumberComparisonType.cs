using BH.oM.Verification;
using System.Collections.Generic;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        public static bool IsNumberComparisonType(this ValueComparisonType comparisonType)
        {
            return m_NumberComparisonTypes.Contains(comparisonType);
        }

        private static readonly HashSet<ValueComparisonType> m_NumberComparisonTypes = new HashSet<ValueComparisonType>
        {
            ValueComparisonType.EqualTo,
            ValueComparisonType.NotEqualTo,
            ValueComparisonType.LessThan,
            ValueComparisonType.LessThanOrEqualTo,
            ValueComparisonType.GreaterThan,
            ValueComparisonType.GreaterThanOrEqualTo
        };
    }
}
