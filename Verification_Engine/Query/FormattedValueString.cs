using BH.Engine.Base;
using BH.oM.Verification.Reporting;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        public static string IFormattedValueString(this object value, IValueConditionReportingConfig config)
        {
            if (config == null)
                return value?.ToString() ?? "null";

            object valueString;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(value, nameof(FormattedValueString), new object[] { config }, out valueString))
            {
                //TODO: error
                return null;
            }

            return (string)valueString;
        }

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
                number = number.RoundNumericValue(config.RoundingAccuracy);

            string result = number.ToString();
            if (!string.IsNullOrWhiteSpace(config.UnitLabel))
                result += $" {config.UnitLabel}";

            return result;
        }

        public static string FormattedValueString(this object value, ValueConditionReportingConfig config)
        {
            return value?.ToString() ?? "null";
        }

        //TODO: merge into Alessio's stuff?
        private static double RoundNumericValue(this double value, double accuracy)
        {
            if (double.IsNaN(value) || double.IsNaN(accuracy))
            {
                return value;
            }

            decimal num = (decimal)accuracy;
            if (num == 0m)
            {
                return value;
            }

            decimal num2 = (decimal)value;
            decimal num3 = num2 % num;
            if (num3 >= num / 2m)
            {
                num3 = -(num - num3);
            }

            decimal num4 = num2 - num3;
            return (double)num4;
        }
    }
}
