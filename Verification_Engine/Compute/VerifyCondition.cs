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

using BH.Engine.Base;
using BH.Engine.Base.Objects;
using BH.Engine.Verification.Objects;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Verification;
using BH.oM.Verification.Conditions;
using BH.oM.Verification.Results;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace BH.Engine.Verification
{
    public static partial class Compute
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        [Description("Verifies an object against a condition and returns a result object containing details of the check.")]
        [Input("obj", "Object to check against the condition.")]
        [Input("condition", "Condition to check the object against.")]
        [Output("result", "Object containing the check result as a boolean as well as details of the check (extracted values etc.).")]
        public static IConditionResult IVerifyCondition(this object obj, ICondition condition)
        {
            if (condition is IsNotNull notNullCondition)
                return VerifyCondition(obj, notNullCondition);

            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition because it was null.");
                return null;
            }

            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(obj, nameof(VerifyCondition), new object[] { condition }, out result))
            {
                BH.Engine.Base.Compute.RecordError($"Verification failed because condition of type {condition.GetType().Name} is currently not supported.");
                return null;
            }

            return result as IConditionResult;
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Verifies an object against " + nameof(LogicalNotCondition) + " and returns a result object containing details of the check.")]
        [Input("obj", "Object to check against the condition.")]
        [Input("condition", "Condition to check the object against.")]
        [Output("result", "Object containing the check result as a boolean as well as details of the check (extracted values etc.).")]
        public static SingleLogicalConditionResult VerifyCondition(this object obj, LogicalNotCondition condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition because it was null.");
                return null;
            }

            IConditionResult result = obj.IVerifyCondition(condition.Condition);
            bool? inverted;
            if (result.Passed == null)
                inverted = null;
            else
                inverted = !(result.Passed.Value);

            return new SingleLogicalConditionResult(inverted, result);
        }

        /***************************************************/

        [Description("Verifies an object against " + nameof(ILogicalCollectionCondition) + " and returns a result object containing details of the check.")]
        [Input("obj", "Object to check against the condition.")]
        [Input("condition", "Condition to check the object against.")]
        [Output("result", "Object containing the check result as a boolean as well as details of the check (extracted values etc.).")]
        public static LogicalCollectionConditionResult VerifyCondition(this object obj, ILogicalCollectionCondition condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition because it was null.");
                return null;
            }

            if (condition.Conditions.Count == 0)
                return new LogicalCollectionConditionResult(null, new List<IConditionResult>());

            List<IConditionResult> results = new List<IConditionResult>();
            foreach (ICondition f in condition.Conditions)
            {
                if (f == null)
                    continue;

                var r = obj.IVerifyCondition(f);
                results.Add(r);
            }

            bool? pass;
            if (condition is LogicalAndCondition)
            {
                if (results.Any(x => x == null))
                    pass = null;
                else
                    pass = results.All(x => x.Passed == true);
            }
            else if (condition is LogicalOrCondition)
                pass = results.Any(x => x.Passed == true);
            else
            {
                BH.Engine.Base.Compute.RecordError($"Verification failed because condition of type {condition.GetType().Name} is currently not supported.");
                return null;
            }

            return new LogicalCollectionConditionResult(pass, results);
        }

        /***************************************************/

        [Description("Verifies an object against " + nameof(IsInDomain) + " condition and returns a result object containing details of the check.")]
        [Input("obj", "Object to check against the condition.")]
        [Input("condition", "Condition to check the object against.")]
        [Output("result", "Object containing the check result as a boolean as well as details of the check (extracted value etc.).")]
        public static ValueConditionResult VerifyCondition(this object obj, IsInDomain condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition because it was null.");
                return null;
            }

            // Try to extract the value from value source
            Output<bool, object> valueFromSource = obj.TryGetValueFromSource(condition);
            bool found = valueFromSource?.Item1 == true;
            if (!found)
                return new ValueConditionResult(null, null);

            // If value found, check the actual condition
            object value = valueFromSource.Item2;
            bool? pass = false;
            
            double tolerance;
            double.TryParse(condition.Tolerance.ToString(), out tolerance);
            if (double.IsNaN(tolerance) || tolerance == 0)
            {
                BH.Engine.Base.Compute.RecordNote("Tolerance has not been set, default value of 1e-6 is being used.");
                tolerance = 1e-6;
            }

            double numericalValue;
            if (double.TryParse(value?.ToString(), out numericalValue))
                pass = Query.IsInDomain(numericalValue, condition.Domain, tolerance);
            else if (value is DateTime)
            {
                DateTime? dt = value as DateTime?;
                pass = Query.IsInDomain(dt.Value.Ticks, condition.Domain, tolerance);
            }
            else
                pass = null;

            return new ValueConditionResult(pass, value);
        }

        /***************************************************/

        [Description("Verifies an object against " + nameof(HasId) + " condition and returns a result object containing details of the check.")]
        [Input("obj", "Object to check against the condition.")]
        [Input("condition", "Condition to check the object against.")]
        [Output("result", "Object containing the check result as a boolean as well as details of the check (extracted value etc.).")]
        public static ValueConditionResult VerifyCondition(this object obj, HasId condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition because it was null.");
                return null;
            }

            object id = ((obj as IBHoMObject)?.Fragments.FirstOrDefault(x => x is IAdapterId) as IAdapterId)?.Id;
            if (id == null)
                id = ((obj as IBHoMObject)?.Fragments.FirstOrDefault(x => x is IPersistentAdapterId) as IPersistentAdapterId)?.PersistentId;

            bool? pass = id != null && condition.Ids.Contains(id);
            return new ValueConditionResult(pass, id);
        }

        /***************************************************/

        [Description("Verifies an object against " + nameof(IsInSet) + " condition and returns a result object containing details of the check.")]
        [Input("obj", "Object to check against the condition.")]
        [Input("condition", "Condition to check the object against.")]
        [Output("result", "Object containing the check result as a boolean as well as details of the check (extracted value etc.).")]
        public static ValueConditionResult VerifyCondition(this object obj, IsInSet condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition because it was null.");
                return null;
            }

            // Try to extract the value from value source
            Output<bool, object> valueFromSource = obj.TryGetValueFromSource(condition);
            bool found = valueFromSource?.Item1 == true;
            if (!found)
                return new ValueConditionResult(null, null);

            // If value found, check the actual condition
            object value = valueFromSource?.Item2;
            bool? pass;
            if (value is IEnumerable<object> ienumerable)
                pass = ienumerable.All(x => x.IsInSet(condition.Set, condition.ComparisonConfig));
            else
                pass = value.IsInSet(condition.Set, condition.ComparisonConfig);

            return new ValueConditionResult(pass, value);
        }

        /***************************************************/

        [Description("Verifies an object against " + nameof(IsNotNull) + " condition and returns a result object containing details of the check.")]
        [Input("obj", "Object to check against the condition.")]
        [Input("condition", "Condition to check the object against.")]
        [Output("result", "Object containing the check result as a boolean as well as details of the check.")]
        public static IsNotNullResult VerifyCondition(this object obj, IsNotNull condition)
        {
            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition because it was null.");
                return null;
            }

            bool? pass = obj.Passes(condition);
            return new IsNotNullResult(pass);
        }

        /***************************************************/

        [Description("Verifies an object against " + nameof(IsOfType) + " condition and returns a result object containing details of the check.")]
        [Input("obj", "Object to check against the condition.")]
        [Input("condition", "Condition to check the object against.")]
        [Output("result", "Object containing the check result as a boolean as well as details of the check (object type etc.).")]
        public static IsOfTypeResult VerifyCondition(this object obj, IsOfType condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition because it was null.");
                return null;
            }

            Type type = condition.Type is string ? BH.Engine.Base.Create.Type(condition.Type.ToString()) : condition.Type as Type;
            if (type == null)
                return new IsOfTypeResult(null, null);

            Type extractedType = obj.GetType();
            bool? pass = extractedType == type;
            return new IsOfTypeResult(pass, extractedType);
        }

        /***************************************************/

        [Description("Verifies an object against " + nameof(HasValue) + " condition and returns a result object containing details of the check.")]
        [Input("obj", "Object to check against the condition.")]
        [Input("condition", "Condition to check the object against.")]
        [Output("result", "Object containing the check result as a boolean as well as details of the check (object type etc.).")]
        public static ValueConditionResult VerifyCondition(this object obj, HasValue condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition because it was null.");
                return null;
            }

            Output<bool, object> valueFromSource = obj.TryGetValueFromSource(condition);
            bool found = valueFromSource?.Item1 == true;
            if (!found)
                return new ValueConditionResult(null, null);

            object value = valueFromSource.Item2;
            bool? pass = (value is double valueDouble && !double.IsNaN(valueDouble))
                      || (value is string valueString && !string.IsNullOrEmpty(valueString))
                      || !value.GetType().IsNullable()
                      || value != null;

            return new ValueConditionResult(pass, valueFromSource.Item2); 
        }

        /***************************************************/

        [Description("Verifies an object against " + nameof(ValueCondition) + " and returns a result object containing details of the check.")]
        [Input("obj", "Object to check against the condition.")]
        [Input("condition", "Condition to check the object against.")]
        [Output("result", "Object containing the check result as a boolean as well as details of the check (extracted value etc.).")]
        public static ValueConditionResult VerifyCondition(this object obj, ValueCondition condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify condition because it was null.");
                return null;
            }

            // Try to extract the value from value source
            Output<bool, object> valueFromSource = obj.TryGetValueFromSource(condition);
            bool found = valueFromSource?.Item1 == true;
            if (!found)
                return new ValueConditionResult(null, null);
            
            object value = valueFromSource?.Item2;

            // If value found, check the actual condition
            bool? pass = value.CompareValues(condition.ReferenceValue, condition.ComparisonType, condition.Tolerance);
            return new ValueConditionResult(pass, value);
        }

        /***************************************************/

        [Description("Verifies an object against " + nameof(FormulaCondition) + " and returns a result object containing details of the check.")]
        [Input("obj", "Object to check against the condition.")]
        [Input("condition", "Condition to check the object against.")]
        [Output("result", "Object containing the check result as a boolean as well as details of the check (extracted value etc.).")]
        public static FormulaConditionResult VerifyCondition(this object obj, FormulaCondition condition)
        {
            if (string.IsNullOrWhiteSpace(condition?.VerificationFormula?.Equation))
            {
                BH.Engine.Base.Compute.RecordError("The formula condition is null or verification formula is null or empty.");
                return new FormulaConditionResult(null, new Dictionary<string, object>());
            }

            if (condition.CalculationFormulae.Any(x => string.IsNullOrWhiteSpace(x.Value?.Equation)))
            {
                BH.Engine.Base.Compute.RecordError("At least one of the calculation formulae is null or empty.");
                return new FormulaConditionResult(null, new Dictionary<string, object>());
            }

            if (condition.Inputs.Any(x => x.Value == null))
            {
                BH.Engine.Base.Compute.RecordError("At least one of the inputs is null.");
                return new FormulaConditionResult(null, new Dictionary<string, object>());
            }

            Dictionary<string, object> components = condition.Inputs.ToDictionary(x => x.Key, x => (object)x.Value);
            foreach (var formula in condition.CalculationFormulae)
            {
                components.Add(formula.Key, (object)formula.Value);
            }

            object verification = condition.VerificationFormula.Solve(obj, components);
            return new FormulaConditionResult((verification as bool?), components.Where(x => !(x.Value is IValueSource) && !(x.Value is Formula)).ToDictionary(x => x.Key, x => x.Value));
        }


        /***************************************************/
        /****              Private Methods              ****/
        /***************************************************/

        private static bool IsInSet(this object value, List<object> set, ComparisonConfig comparisonConfig)
        {
            if (comparisonConfig != null)
                return set.Contains(value, new HashComparer<object>(comparisonConfig));
            else
                return set.Contains(value);
        }

        /***************************************************/

        private static object Solve(this Formula formula, object obj, Dictionary<string, object> variables)
        {
            try
            {
                // Weird setup of the globals class because neither dynamic nor anonymous types work with C# scripts
                Type genericType = typeof(FormulaVariables<,,,,,,,,,>);

                // Collect variables used in the equation
                string formulaToSolve = formula.Equation;
                var usedVariables = new List<(string, object)>();
                int k = 1;
                foreach (var key in variables.Keys.OrderByDescending(x => x))
                {
                    if (formulaToSolve.Contains(key))
                    {
                        // Update the equation with globals property name in Variable$$ format
                        string propName = $"Variable{k}";
                        formulaToSolve = formulaToSolve.Replace(key, propName);
                        k++;

                        // Make sure the variable is calculated - if not yet, then calculate
                        object value = variables[key];
                        if (value is IValueSource vs)
                        {
                            Output<bool, object> valueFromSource = obj.ITryGetValueFromSource(vs);
                            if (value == null || (value is double && double.IsNaN((double)value)))
                                return null;

                            variables[key] = value;
                        }
                        else if (value is Formula f)
                        {
                            value = f.Solve(obj, variables);
                            if (value == null || (value is double && double.IsNaN((double)value)))
                                return null;
                            
                            variables[key] = value;
                        }

                        // Cast enums to strings in the equation
                        if (value?.GetType().IsEnum == true)
                            value = value.ToString();

                        usedVariables.Add((key, value));
                    }
                }

                // Create globals object based on the variables
                Type[] variableTypes = usedVariables.Select(x => x.Item2?.GetType() ?? typeof(object)).ToArray();
                variableTypes = variableTypes.Concat(Enumerable.Repeat(typeof(object), 10 - variableTypes.Length)).ToArray();
                Type constructedType = genericType.MakeGenericType(variableTypes);
                object globals = Activator.CreateInstance(constructedType);

                // Set the properties of globals object based on variables
                k = 1;
                foreach (var kvp in usedVariables)
                {
                    string propName = $"Variable{k}";
                    constructedType.GetProperty(propName).SetValue(globals, kvp.Item2);
                    k++;
                }

                // Temporarily replace strings with placeholders
                // This way we make sure the strings will not changed
                Dictionary<string, string> stringReplacements = new Dictionary<string, string>();
                string stringRegex = @"'([^']*)'";
                int stringCount = 0;
                foreach (var match in Regex.Matches(formulaToSolve, stringRegex).Cast<Match>().Select(x => x.Value).Distinct().OrderByDescending(x => x.Length))
                {
                    stringCount++;
                    string replacement = $"\"%%TempString{stringCount}\"";
                    formulaToSolve = formulaToSolve.Replace(match, replacement);
                    stringReplacements.Add(replacement, match.Replace("'", "\""));
                }

                // Trim trailing whitespaces, lowercase ifs and elses etc.
                formulaToSolve = formulaToSolve.Trim();
                formulaToSolve = Regex.Replace(formulaToSolve, "if", "if", RegexOptions.IgnoreCase);
                formulaToSolve = Regex.Replace(formulaToSolve, "else", "else", RegexOptions.IgnoreCase);
                formulaToSolve = formulaToSolve.Replace("and", "&&");
                formulaToSolve = formulaToSolve.Replace("or", "||");

                // Wrap enum values in quotes (compared as strings)
                string toWrapPattern = @"\b(?!\bif\b|\belse\b|\bVariable\d+|\bTempString\d+\b|\bnull\b)(?=\w*[a-zA-Z])\w+\b";
                formulaToSolve = Regex.Replace(formulaToSolve, toWrapPattern, "\"$&\"");

                // Conditional statements need a bit more attention
                if (formulaToSolve.StartsWith("if"))
                {
                    // Convert the pseudocode into valid C# code
                    string[] conditions = formulaToSolve.Split(',');
                    string csharpCode = "object result;\n";

                    foreach (string condition in conditions)
                    {
                        string trimmedCondition = condition.Trim();

                        int ii = 0;
                        if (trimmedCondition.StartsWith("if"))
                            ii = 3;
                        else if (trimmedCondition.StartsWith("else if"))
                            ii = 7;

                        if (ii != 0)
                        {
                            int colonIndex = condition.IndexOf(':');
                            trimmedCondition = trimmedCondition.Substring(0, ii) + "(" + trimmedCondition.Substring(ii, colonIndex - ii) + ")" + trimmedCondition.Substring(colonIndex);
                        }

                        csharpCode += trimmedCondition.Replace(":", "{ result =") + "; }\n";
                    }

                    csharpCode += "return result;";
                    formulaToSolve = csharpCode;
                }

                if (formula.Tolerance != null)
                {
                    if (formula.Tolerance is double tolerance)
                    {
                        if (double.IsNaN(tolerance) || tolerance < 0)
                        {
                            BH.Engine.Base.Compute.RecordError($"Formula {formula.Equation} could not be solved because of invalid value of tolerance - it needs to be a positive number.");
                            return null;
                        }
                        formulaToSolve = formulaToSolve.IntroduceTolerance(tolerance);
                    }
                    else
                    {
                        BH.Engine.Base.Compute.RecordError($"Formula {formula.Equation} could not be solved because of unsupported type of tolerance.");
                        return null;
                    }
                }

                // Bring back the original strings
                foreach (var key in stringReplacements.Keys.OrderByDescending(x => x.Length))
                {
                    formulaToSolve = formulaToSolve.Replace(key, stringReplacements[key]);
                }

                return CSharpScript.EvaluateAsync(formulaToSolve, globals: globals).Result;
            }
            catch
            {
                BH.Engine.Base.Compute.RecordError($"Formula {formula.Equation} could not be solved, please make sure it does not contain errors.");
                return null;
            }
        }

        /***************************************************/

        private static string ComparisonSign(this ValueComparisonType comparison)
        {
            switch (comparison)
            {
                case ValueComparisonType.EqualTo:
                    return "==";
                case ValueComparisonType.NotEqualTo:
                    return "!=";
                case ValueComparisonType.GreaterThan:
                    return ">";
                case ValueComparisonType.GreaterThanOrEqualTo:
                    return ">=";
                case ValueComparisonType.LessThan:
                    return "<";
                case ValueComparisonType.LessThanOrEqualTo:
                    return "<=";
                default:
                    return "";
            }
        }

        /***************************************************/

        private static string ComparisonReplacement(string left, string right, ValueComparisonType comparison, double tolerance)
        {
            switch (comparison)
            {
                case ValueComparisonType.EqualTo:
                    return $"(System.Math.Abs({left} - {right}) <= {tolerance})";
                case ValueComparisonType.NotEqualTo:
                    return $"(System.Math.Abs({left} - {right}) > {tolerance})";
                case ValueComparisonType.GreaterThan:
                    return $"({left} - {right} > {tolerance})";
                case ValueComparisonType.GreaterThanOrEqualTo:
                    return $"({left} - {right} >= -{tolerance})";
                case ValueComparisonType.LessThan:
                    return $"({left} - {right} < -{tolerance})";
                case ValueComparisonType.LessThanOrEqualTo:
                    return $"({left} - {right} <= {tolerance})";
                default:
                    return null;
            }
        }

        /***************************************************/

        private static string IntroduceTolerance(this string equation, double tolerance)
        {
            equation = equation.IntroduceTolerance(ValueComparisonType.GreaterThan, tolerance);
            equation = equation.IntroduceTolerance(ValueComparisonType.GreaterThanOrEqualTo, tolerance);
            equation = equation.IntroduceTolerance(ValueComparisonType.LessThan, tolerance);
            equation = equation.IntroduceTolerance(ValueComparisonType.LessThanOrEqualTo, tolerance);
            equation = equation.IntroduceTolerance(ValueComparisonType.EqualTo, tolerance);
            equation = equation.IntroduceTolerance(ValueComparisonType.NotEqualTo, tolerance);
            return equation;
        }

        /***************************************************/

        private static string IntroduceTolerance(this string equation, ValueComparisonType comparison, double tolerance)
        {
            string comparisonSign = comparison.ComparisonSign();
            if (string.IsNullOrWhiteSpace(comparisonSign))
                return equation;

            Dictionary<Match, string> replacements = new Dictionary<Match, string>();
            string regex = @"([^&|<>=!{};]+)\s*(" + comparisonSign + @")\s*([^&|<>=!{};]+)";

            foreach (Match match in Regex.Matches(equation, regex))
            {
                string left = match.Groups[1].Value;
                string right = match.Groups[3].Value;

                int leftCutoff = -1;
                int rightCutoff = -1;

                if (left.Contains("return "))
                {
                    leftCutoff = left.IndexOf("return ") + 6;
                    left = left.Substring(leftCutoff + 1);
                }

                if (left.Count(c => c == '(') > left.Count(c => c == ')'))
                {
                    int newCutoff = left.IndexOf('(');
                    if (leftCutoff == -1)
                        leftCutoff = newCutoff;
                    else
                        leftCutoff += newCutoff;

                    left = left.Substring(newCutoff + 1);
                }

                if (right.Count(c => c == ')') > right.Count(c => c == '('))
                {
                    rightCutoff = right.LastIndexOf(')');
                    right = right.Substring(0, rightCutoff);
                }

                if (left.Contains('"') || right.Contains('"'))
                    continue;

                string replacement = ComparisonReplacement(left, right, comparison, tolerance);
                if (leftCutoff != -1)
                    replacement = match.Groups[1].Value.Substring(0, leftCutoff + 1) + replacement;

                if (rightCutoff != -1)
                    replacement = replacement + match.Groups[3].Value.Substring(rightCutoff);

                replacements[match] = replacement;
            }

            string result = equation;
            foreach (var kvp in replacements.OrderByDescending(x => x.Key.Index))
            {
                result = result.Remove(kvp.Key.Index, kvp.Key.Length).Insert(kvp.Key.Index, kvp.Value);
            }

            return result;
        }

        /***************************************************/
    }
}

