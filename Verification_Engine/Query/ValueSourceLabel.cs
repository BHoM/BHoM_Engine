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

using BH.oM.Base.Attributes;
using BH.oM.Verification.Conditions;
using BH.oM.Verification.Reporting;
using System.ComponentModel;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        [Description("Generates a human readable label for a value source based on provided value condition reporting config.")]
        [Input("valueSource", "Value source to get the label for.")]
        [Input("reportingConfig", "Reporting config to apply when generating the label.")]
        [Output("label", "Human readable label generated for the input value source.")]
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

        [Description("Generates a human readable label for a value source embedded in a value condition, based on provided value condition reporting config.")]
        [Input("condition", "Value condition containing value source to get the label for.")]
        [Input("reportingConfig", "Reporting config to apply when generating the label.")]
        [Output("label", "Human readable label generated for the value source embedded in the input value condition.")]
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

        [Description("Generates a human readable label for a value source based on provided value condition reporting config.")]
        [Input("valueSource", "Value source to get the label for.")]
        [Input("reportingConfig", "Reporting config to apply when generating the label.")]
        [Output("label", "Human readable label generated for the input value source.")]
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
