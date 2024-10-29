using BH.oM.Base;
using BH.oM.Verification;
using BH.oM.Verification.Conditions;
using BH.oM.Verification.Reporting;
using BH.oM.Verification.Requirements;
using BH.oM.Verification.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        public static string ReportMessage(this Requirement requirement, RequirementResult result)
        {
            return requirement.Condition.IReportMessage(result.VerificationResult, requirement.ReportingConfig);
        }

        public static string IReportMessage(this ICondition condition, IConditionResult result, IConditionReportingConfig config)
        {
            object message;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(condition, nameof(ReportMessage), new object[] { result, config }, out message))
            {
                //TODO: error
                return null;
            }

            return (string)message;
        }

        public static string ReportMessage(this LogicalNotCondition condition, SingleLogicalConditionResult result, SingleLogicalConditionReportingConfig config)
        {
            if (condition == null || result == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not create report for the condition result because one of the required values was null.");
                return null;
            }

            if (result.Passed == null)
                return "Verification of condition was inconclusive.";

            return $"Logical NOT condition {((bool)result.Passed ? "passed" : "failed")} after inverting the following: [{condition.Condition.IReportMessage(result.Result, config.NestedConfig)}]";
        }

        public static string ReportMessage(this ILogicalCollectionCondition condition, LogicalCollectionConditionResult result, LogicalCollectionConditionReportingConfig config)
        {
            if (condition == null || result == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not create report for the condition result because one of the required values was null.");
                return null;
            }

            if (result.Passed == null)
                return "Verification of condition was inconclusive.";

            string prefix;
            string passed = (bool)result.Passed ? "passed" : "failed";
            if (condition is LogicalAndCondition)
            {
                prefix = $"Logical AND condition {passed}:\n[";
            }
            else if (condition is LogicalOrCondition)
            {
                prefix = $"Logical OR condition {passed}:\n[";
            }
            else
                throw new NotImplementedException();

            List<string> passes = new List<string>();
            List<string> fails = new List<string>();
            for (int i = 0; i < condition.Conditions.Count; i++)
            {
                ICondition subCondition = condition.Conditions[i];
                IConditionResult subResult = result.Results[i];
                
                //TODO: will dictionary work after deserialisation??
                IConditionReportingConfig subConfig;
                config.NestedConfigs.TryGetValue(subCondition, out subConfig);

                string subReport = subCondition.IReportMessage(subResult, config);
                if ((bool)subResult.Passed)
                    passes.Add(subReport);
                else
                    fails.Add(subReport);
            }

            string report = prefix;
            if (passes.Count == 0 && fails.Count == 0)
                report += "\n    No conditions evaluated";
            else
            {
                if (passes.Count != 0)
                    report += $"\n    Passes:\n{string.Join(",\n", passes.Select(x => "        " + x.Replace("\n", "\n        ")))}";

                if (fails.Count != 0)
                {
                    report += $"\n    Fails:\n{string.Join(",\n", fails.Select(x => "        " + x.Replace("\n", "\n        ")))}";
                }
            }

            report += "\n]";
            return report;
        }

        public static string ReportMessage(this ValueCondition condition, ValueConditionResult result, IValueConditionReportingConfig config)
        {
            if (condition == null || result == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not create report for the condition result because one of the required values was null.");
                return null;
            }

            if (result.Passed == null)
                return "Verification of condition was inconclusive.";

            string sourceLabel = condition.ValueSourceLabel(config);
            string extractedValueLabel = result.ExtractedValue.IFormattedValueString(config);
            string refValueLabel = condition.ReferenceValue.IFormattedValueString(config);

            if (result.Passed.Value)
                return $"{sourceLabel} is {extractedValueLabel}.";
            else
                return $"{sourceLabel} must {condition.ComparisonType.ComparisonString()} {refValueLabel}, but is {extractedValueLabel}.";
        }

        public static string ReportMessage(this IsInDomain condition, ValueConditionResult result, IValueConditionReportingConfig config)
        {
            if (condition == null || result == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not create report for the condition result because one of the required values was null.");
                return null;
            }

            if (result.Passed == null)
                return "Verification of condition was inconclusive.";

            string sourceLabel = condition.ValueSourceLabel(config);
            string extractedValueLabel = result.ExtractedValue.IFormattedValueString(config);
            string minLabel = condition.Domain.Min.IFormattedValueString(config);
            string maxLabel = condition.Domain.Max.IFormattedValueString(config);

            if (result.Passed.Value)
                return $"{sourceLabel} is {extractedValueLabel}, which is in ({minLabel}, {maxLabel}).";
            else
                return $"{sourceLabel} is {extractedValueLabel}, which is not in ({minLabel}, {maxLabel}).";
        }

        public static string ReportMessage(this HasId condition, ValueConditionResult result, IValueConditionReportingConfig config)
        {
            if (condition == null || result == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not create report for the condition result because one of the required values was null.");
                return null;
            }

            if (result.Passed == null)
                return "Verification of condition was inconclusive.";

            string sourceLabel = condition.ValueSourceLabel(config);

            if (result.Passed.Value)
                return $"{sourceLabel} contains one of the requested ids.";
            else
                return $"{sourceLabel} does not contain any of the requested ids: {string.Join(" | ", condition.Ids.Select(v => v.ToString()))}.";
        }

        public static string ReportMessage(this IsInSet condition, ValueConditionResult result, IValueConditionReportingConfig config)
        {
            if (condition == null || result == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not create report for the condition result because one of the required values was null.");
                return null;
            }

            if (result.Passed == null)
                return "Verification of condition was inconclusive.";

            string sourceLabel = condition.ValueSourceLabel(config);
            object extractedValue = result.ExtractedValue.ValueFromSource(condition);

            string extractedValueLabel;
            if (extractedValue is IEnumerable<object>)
                extractedValueLabel = string.Join(", ", ((IEnumerable<object>)extractedValue).Select(x => x.IFormattedValueString(config)));
            else
                extractedValueLabel = extractedValue.IFormattedValueString(config);

            if (result.Passed.Value)
                return $"{sourceLabel} is {extractedValueLabel}, which is among: {string.Join(" | ", condition.Set.Select(v => v.IFormattedValueString(config)))}.";
            else
                return $"{sourceLabel} is {extractedValueLabel}, which is not among: {string.Join(" | ", condition.Set.Select(v => v.IFormattedValueString(config)))}.";
        }

        public static string ReportMessage(this IsNotNull condition, IsNotNullResult result)
        {
            if (condition == null || result == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not create report for the condition result because one of the required values was null.");
                return null;
            }

            if (result.Passed == null)
                return "Verification of condition was inconclusive.";

            if ((bool)result.Passed)
                return "The object is not null.";
            else
                return "The object is null.";
        }

        public static string ReportMessage(this IsOfType condition, IsOfTypeResult result)
        {
            if (condition == null || result == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not create report for the condition result because one of the required values was null.");
                return null;
            }

            if (result.Passed == null)
                return "Verification of condition was inconclusive.";

            if ((bool)result.Passed)
                return $"The object is of type {result.ExtractedType.Name}.";
            else
            {
                string refType = condition.Type.GetType() == typeof(Type) ? ((Type)condition.Type).Name : condition.Type.ToString();
                return $"The object is of type {result.ExtractedType.Name}, not {refType}.";
            }
        }

        private static string ComparisonString(this ValueComparisonType comparisonType)
        {
            switch (comparisonType)
            {
                case ValueComparisonType.EqualTo:
                    return "be equal to";
                case ValueComparisonType.NotEqualTo:
                    return "be not equal to";
                case ValueComparisonType.LessThan:
                    return "be less than";
                case ValueComparisonType.LessThanOrEqualTo:
                    return "be less than or equal to";
                case ValueComparisonType.GreaterThanOrEqualTo:
                    return "be greater than or equal to";
                case ValueComparisonType.GreaterThan:
                    return "be greater than";
                case ValueComparisonType.Contains:
                    return "contain";
                case ValueComparisonType.StartsWith:
                    return "start with";
                case ValueComparisonType.EndsWith:
                    return "end with";
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
