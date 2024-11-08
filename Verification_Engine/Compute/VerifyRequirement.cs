/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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

using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Verification.Requirements;
using BH.oM.Verification.Results;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Verification
{
    public static partial class Compute
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        [Description("Verifies an object against a condition embedded in the requirement and returns a result object with all necessary details.")]
        [Input("obj", "Object to check against the requirement.")]
        [Input("requirement", "Requirement to check the object against.")]
        [Output("result", "Result object containing references to the input object and requirement as well as condition verification result.")]
        public static RequirementResult IVerifyRequirement(this object obj, IRequirement requirement)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not verify requirement against a null object.");
                return null;
            }

            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(obj, nameof(VerifyRequirement), new object[] { requirement }, out result))
            {
                BH.Engine.Base.Compute.RecordError($"Verification failed because requirement of type {requirement.GetType().Name} is currently not supported.");
                return null;
            }

            return result as RequirementResult;
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Verifies an object against a condition embedded in the requirement and returns a result object with all necessary details.")]
        [Input("obj", "Object to check against the requirement.")]
        [Input("requirement", "Requirement to check the object against.")]
        [Output("result", "Result object containing references to the input object and requirement as well as condition verification result.")]
        public static RequirementResult VerifyRequirement(this object obj, Requirement requirement)
        {
            if (requirement == null || requirement.Condition.INestedConditions().Any(x => x == null))
            {
                BH.Engine.Base.Compute.RecordError($"Requirement {requirement.Name} is null or its condition contains nulls.");
                return null;
            }

            IConditionResult conditionResult = obj.IVerifyCondition(requirement.Condition);
            return new RequirementResult(requirement.BHoM_Guid, obj.IIdentifier(), conditionResult);
        }

        /***************************************************/
    }
}
