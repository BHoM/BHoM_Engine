using BH.oM.Verification.Conditions;
using BH.oM.Verification.Reporting;
using System;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        public static string ValueSourceLabel(this IValueCondition condition, IValueConditionReportingConfig reportingConfig = null)
        {
            return condition.ValueSource.IValueSourceLabel(reportingConfig);
        }

        public static string IValueSourceLabel(this IValueSource valueSource, IValueConditionReportingConfig reportingConfig = null)
        {
            return (string)BH.Engine.Base.Compute.RunExtensionMethod(valueSource, nameof(ValueSourceLabel), new object[] { reportingConfig });
        }

        public static string ValueSourceLabel(this PropertyValueSource valueSource, IValueConditionReportingConfig reportingConfig = null)
        {
            if (!string.IsNullOrWhiteSpace(reportingConfig?.ValueSourceLabelOverride))
                return reportingConfig.ValueSourceLabelOverride;
            else
                return $"Property {valueSource.PropertyName}";
        }
    }
}
