using BH.oM.Verification.Conditions;
using BH.oM.Verification.Reporting;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        public static string IValueSourceLabel(this IValueSource valueSource, IValueConditionReportingConfig reportingConfig = null)
        {
            if (valueSource == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not get the value source label for a null value source.");
                return null;
            }

            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(valueSource, nameof(ValueSourceLabel), new object[] { reportingConfig }, out result))
            {
                BH.Engine.Base.Compute.RecordError($"Extraction of value source label failed because value source of type {valueSource.GetType().Name} is currently not supported.");
                return null;
            }

            return result as string;
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        public static string ValueSourceLabel(this IValueCondition condition, IValueConditionReportingConfig reportingConfig = null)
        {
            if (condition?.ValueSource == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not get the value source label for a null value source.");
                return null;
            }

            return condition.ValueSource.IValueSourceLabel(reportingConfig);
        }

        /***************************************************/

        public static string ValueSourceLabel(this PropertyValueSource valueSource, IValueConditionReportingConfig reportingConfig = null)
        {
            if (valueSource == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not get the value source label for a null value source.");
                return null;
            }

            if (!string.IsNullOrWhiteSpace(reportingConfig?.ValueSourceLabelOverride))
                return reportingConfig.ValueSourceLabelOverride;
            else
                return $"Property {valueSource.PropertyName}";
        }

        /***************************************************/
    }
}
