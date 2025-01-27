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
using BH.oM.Base.Attributes;
using BH.oM.Verification.Reporting;
using System.ComponentModel;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        [Description("Generates a string representation of a value based on provided value condition reporting config.")]
        [Input("value", "Value to query string representation.")]
        [Input("config", "Reporting config to apply when generating string representation of the input value.")]
        [Output("formattedString", "String representation of the input value based on provided value reporting config.")]
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

        [Description("Generates a string representation of a value based on provided value condition reporting config.")]
        [Input("value", "Value to query string representation.")]
        [Input("config", "Reporting config to apply when generating string representation of the input value.")]
        [Output("formattedString", "String representation of the input value based on provided value reporting config.")]
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

        [Description("Generates a string representation of a value based on provided value condition reporting config.")]
        [Input("value", "Value to query string representation.")]
        [Input("config", "Reporting config to apply when generating string representation of the input value.")]
        [Output("formattedString", "String representation of the input value based on provided value reporting config.")]
        public static string FormattedValueString(this object value, ValueConditionReportingConfig config)
        {
            return value?.ToString() ?? "null";
        }

        /***************************************************/
    }
}

