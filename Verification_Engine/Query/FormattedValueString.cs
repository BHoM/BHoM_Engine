using BH.Engine.Base;
using BH.oM.Verification.Reporting;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        public static string IFormattedValueString(this object value, IValueConditionReportingConfig config)
        {
            if (config == null)
                return value?.ToString() ?? "null";

            object valueString;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(value, nameof(FormattedValueString), new object[] { config }, out valueString))
            {
                BH.Engine.Base.Compute.RecordError($"Formatting failed because reporting config of type {config.GetType().Name} is currently not supported.");
                return null;
            }

            return (string)valueString;
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        public static string FormattedValueString(this object value, NumberConditionReportingConfig config)
        {
            if (!(value.GetType().IsNumeric()) || config == null)
                return value?.ToString() ?? "null";

            double number = (double)value;
            if (double.IsNaN(number))
                return "NaN";

            if (!double.IsNaN(config.ValueMultiplier))
                number /= config.ValueMultiplier;

            if (!double.IsNaN(config.RoundingAccuracy))
                number = number.Round(config.RoundingAccuracy);

            string result = number.ToString();
            if (!string.IsNullOrWhiteSpace(config.UnitLabel))
                result += $" {config.UnitLabel}";

            return result;
        }

        /***************************************************/

        public static string FormattedValueString(this object value, ValueConditionReportingConfig config)
        {
            return value?.ToString() ?? "null";
        }

        /***************************************************/
    }
}
