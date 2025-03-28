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
using BH.oM.Verification.Extraction;
using BH.oM.Verification.Reporting;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Verification
{
    public static partial class Create
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Creates " + nameof(LogicalCollectionConditionReportingConfig) + " based on a set of conditions bundled with a set of correspondent reporting configs.")]
        [Input("nestedConditions", "Nested conditions of " + nameof(ILogicalCollectionCondition) + " to set the reporting configs for.")]
        [Input("nestedConfigs", "Reporting configs to bundle with their correspondent nested conditions.")]
        [Output("config", nameof(LogicalCollectionConditionReportingConfig) + " created based on the provided inputs.")]
        public static LogicalCollectionConditionReportingConfig LogicalCollectionConditionReportingConfig(IEnumerable<ICondition> nestedConditions, IEnumerable<IConditionReportingConfig> nestedConfigs)
        {
            if (nestedConditions == null || nestedConfigs == null)
            {
                BH.Engine.Base.Compute.RecordError("Config creation failed because at least one of the inputs is null.");
                return null;
            }

            if (nestedConditions.Count() != nestedConfigs.Count())
            {
                BH.Engine.Base.Compute.RecordError("Config creation failed because count of input conditions does not match the count of input configs.");
                return null;
            }

            LogicalCollectionConditionReportingConfig result = new LogicalCollectionConditionReportingConfig();
            
            IEnumerator<IConditionReportingConfig> configEnumerator = nestedConfigs.GetEnumerator();
            foreach (ICondition nestedCondition in nestedConditions)
            {
                configEnumerator.MoveNext();
                result.NestedConfigs.Add(nestedCondition, configEnumerator.Current);
            }

            return result;
        }

        /***************************************************/
    }
}

