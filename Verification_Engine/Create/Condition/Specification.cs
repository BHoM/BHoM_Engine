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
using BH.oM.Verification.Requirements;
using BH.oM.Verification.Specifications;
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

        [Description("Creates Specification based on the provided filtering conditions, requirements and metadata.")]
        [Input("filterConditions", "Filter conditions to be embedded in the extraction instruction " + nameof(ConditionBasedFilter) + " object.")]
        [Input("requirements", "Collection of requirements to check the extracted object against.")]
        [Input("name", "Name of the created specification.")]
        [Input("description", "Description of the created specification.")]
        [Input("clause", "Human readable identifier to reference the specification.")]
        [Output("specification", "Specification created based on the provided inputs.")]
        public static Specification Specification(List<ICondition> filterConditions, List<IRequirement> requirements, string name = "", string description = "", string clause = "")
        {
            ICondition filterCondition = filterConditions.Count == 1 ? filterConditions[0] : new LogicalAndCondition { Conditions = filterConditions };

            Specification specification = new Specification()
            {
                Extraction = new ConditionBasedFilter { Condition = filterCondition },
                Requirements = requirements.Where(c => c != null).ToList(),
                Name = name,
                Description = description,
                Clause = clause
            };

            return specification;
        }

        /***************************************************/
    }
}

