/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.Engine.Diffing;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Verification;
using BH.oM.Verification.Conditions;
using BH.oM.Verification.Reporting;
using BH.oM.Verification.Requirements;
using BH.oM.Verification.Results;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        [Description("Generates a human readable report based on condition check result combined with reporting config.")]
        [Input("condition", "Condition, from which the result was generated.")]
        [Input("result", "Condition check result to generate report for.")]
        [Input("config", "Reporting config to apply when generating the report.")]
        [Output("report", "Human readable report generated based on the input condition check result combined with reporting config.")]
        public static string IReportMessage(this ICondition condition, IConditionResult result, IConditionReportingConfig config = null)
        {
            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not generate report for a null condition.");
                return null;
            }

            if (result == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not generate report for a null condition result.");
                return null;
            }

            object message;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(condition, nameof(ReportMessage), new object[] { result, config }, out message))
            {
                BH.Engine.Base.Compute.RecordError($"Report generation failed because combination of types {condition.GetType().Name}, {result.GetType().Name} and {config?.GetType().Name} is currently not supported.");
                return null;
            }

            return message as string;
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Generates a human readable report based on requirement verification result.")]
        [Input("requirement", "Requirement, from which the result was generated.")]
        [Input("result", "Requirement verification result to generate report for.")]
        [Output("report", "Human readable report generated based on the input requirement verification result.")]
        public static string ReportMessage(this Requirement requirement, RequirementResult result)
        {
            if (requirement == null || result?.VerificationResult == null)
            {
                BH.Engine.Base.Compute.RecordError("Can't generate the report because either the requirement or result is null.");
                return null;
            }

            string status;
            switch (result.VerificationResult.Passed)
            {
                case true:
                    status = "success";
                    break;
                case false:
                    status = "failed";
                    break;
                default:
                    status = "inconclusive";
                    break;
            }

            string report = $"Status: {status}";
            
            if (result.VerificationResult.Passed != null)
                report += $"\n\nDetailed report:\n{requirement.Condition.IReportMessage(result.VerificationResult, requirement.ReportingConfig)}";

            return report;
        }

        /***************************************************/

        [Description("Generates a human readable report based on " + nameof(LogicalNotCondition) + " check result combined with reporting config.")]
        [Input("condition", "Condition, from which the result was generated.")]
        [Input("result", "Condition check result to generate report for.")]
        [Input("config", "Reporting config to apply when generating the report.")]
        [Output("report", "Human readable report generated based on the input condition check result combined with reporting config.")]
        public static string ReportMessage(this LogicalNotCondition condition, SingleLogicalConditionResult result, SingleLogicalConditionReportingConfig config = null)
        {
            if (condition == null || result == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not create report for the condition result because one of the required values was null.");
                return null;
            }

            if (result.Passed == null)
                return "Verification of condition was inconclusive.";

            return $"Logical NOT condition {((bool)result.Passed ? "passed" : "failed")} after inverting the following:\n[{condition.Condition.IReportMessage(result.Result, config?.NestedConfig)}]";
        }

        /***************************************************/

        [Description("Generates a human readable report based on " + nameof(ILogicalCollectionCondition) + " check result combined with reporting config.")]
        [Input("condition", "Condition, from which the result was generated.")]
        [Input("result", "Condition check result to generate report for.")]
        [Input("config", "Reporting config to apply when generating the report.")]
        [Output("report", "Human readable report generated based on the input condition check result combined with reporting config.")]
        public static string ReportMessage(this ILogicalCollectionCondition condition, LogicalCollectionConditionResult result, LogicalCollectionConditionReportingConfig config = null)
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
            {
                BH.Engine.Base.Compute.RecordError($"Report generation failed because condition of type {condition.GetType().Name} is currently not supported.");
                return null;
            }

            List<string> passes = new List<string>();
            List<string> fails = new List<string>();
            for (int i = 0; i < condition.Conditions.Count; i++)
            {
                ICondition subCondition = condition.Conditions[i];
                IConditionResult subResult = result.Results[i];
                IConditionReportingConfig subConfig = config?.NestedConfigs?.FirstOrDefault(x => x.Key.IsEqual(subCondition)).Value;

                string subReport = subCondition.IReportMessage(subResult, subConfig);
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

        /***************************************************/

        [Description("Generates a human readable report based on " + nameof(ValueCondition) + " check result combined with reporting config.")]
        [Input("condition", "Condition, from which the result was generated.")]
        [Input("result", "Condition check result to generate report for.")]
        [Input("config", "Reporting config to apply when generating the report.")]
        [Output("report", "Human readable report generated based on the input condition check result combined with reporting config.")]
        public static string ReportMessage(this ValueCondition condition, ValueConditionResult result, IValueConditionReportingConfig config = null)
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
            string comparison = condition.ComparisonType.ComparisonString();

            if (result.Passed.Value)
                return $"{sourceLabel} is {extractedValueLabel}, which meets the requirement to {comparison} {refValueLabel}.";
            else
                return $"{sourceLabel} must {comparison} {refValueLabel}, but is {extractedValueLabel}.";
        }

        /***************************************************/

        [Description("Generates a human readable report based on " + nameof(IsInDomain) + " condition check result combined with reporting config.")]
        [Input("condition", "Condition, from which the result was generated.")]
        [Input("result", "Condition check result to generate report for.")]
        [Input("config", "Reporting config to apply when generating the report.")]
        [Output("report", "Human readable report generated based on the input condition check result combined with reporting config.")]
        public static string ReportMessage(this IsInDomain condition, ValueConditionResult result, IValueConditionReportingConfig config = null)
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

        /***************************************************/

        [Description("Generates a human readable report based on " + nameof(HasId) + " condition check result combined with reporting config.")]
        [Input("condition", "Condition, from which the result was generated.")]
        [Input("result", "Condition check result to generate report for.")]
        [Input("config", "Reporting config to apply when generating the report.")]
        [Output("report", "Human readable report generated based on the input condition check result combined with reporting config.")]
        public static string ReportMessage(this HasId condition, ValueConditionResult result, IValueConditionReportingConfig config = null)
        {
            if (condition == null || result == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not create report for the condition result because one of the required values was null.");
                return null;
            }

            if (result.Passed == null)
                return "Verification of condition was inconclusive.";

            if (result.Passed.Value)
                return $"The object contains id equal to {result.ExtractedValue}.";
            else
                return $"The object does not contain any of the requested ids: {string.Join(" | ", condition.Ids.Select(v => v.ToString()))}.";
        }

        /***************************************************/

        [Description("Generates a human readable report based on " + nameof(IsInSet) + " condition check result combined with reporting config.")]
        [Input("condition", "Condition, from which the result was generated.")]
        [Input("result", "Condition check result to generate report for.")]
        [Input("config", "Reporting config to apply when generating the report.")]
        [Output("report", "Human readable report generated based on the input condition check result combined with reporting config.")]
        public static string ReportMessage(this IsInSet condition, ValueConditionResult result, IValueConditionReportingConfig config = null)
        {
            if (condition == null || result == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not create report for the condition result because one of the required values was null.");
                return null;
            }

            if (result.Passed == null)
                return "Verification of condition was inconclusive.";

            string sourceLabel = condition.ValueSourceLabel(config);
            object extractedValue = result.ExtractedValue;

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

        /***************************************************/

        [Description("Generates a human readable report based on " + nameof(IsNotNull) + " condition check result combined with reporting config.")]
        [Input("condition", "Condition, from which the result was generated.")]
        [Input("result", "Condition check result to generate report for.")]
        [Input("config", "Reporting config to apply when generating the report.")]
        [Output("report", "Human readable report generated based on the input condition check result combined with reporting config.")]
        public static string ReportMessage(this IsNotNull condition, IsNotNullResult result, IConditionReportingConfig config = null)
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

        /***************************************************/

        [Description("Generates a human readable report based on " + nameof(IsOfType) + " condition check result combined with reporting config.")]
        [Input("condition", "Condition, from which the result was generated.")]
        [Input("result", "Condition check result to generate report for.")]
        [Input("config", "Reporting config to apply when generating the report.")]
        [Output("report", "Human readable report generated based on the input condition check result combined with reporting config.")]
        public static string ReportMessage(this IsOfType condition, IsOfTypeResult result, IConditionReportingConfig config = null)
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


        /***************************************************/
        /****              Private Methods              ****/
        /***************************************************/

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
                    return comparisonType.ToString();
            }
        }

        /***************************************************/
    }
}

