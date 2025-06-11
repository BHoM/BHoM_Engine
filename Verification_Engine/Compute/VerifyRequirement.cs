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
using BH.oM.Base.Debugging;
using BH.oM.Verification.Requirements;
using BH.oM.Verification.Results;
using System;
using System.Collections.Generic;
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
            if (requirement == null)
            {
                Event error = new Event { Message = "Could not verify requirement against a null requirement.", Type = EventType.Error };
                BH.Engine.Base.Compute.RecordEvent(error);
                return new RequirementResult(null, obj?.IIdentifier(), null, new List<Event> { error });
            }

            if (obj == null)
            {
                Event error = new Event { Message = "Could not verify requirement against a null object.", Type = EventType.Error };
                BH.Engine.Base.Compute.RecordEvent(error);
                return new RequirementResult(requirement.IIdentifier(), null, null, new List<Event> { error });
            }

            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(obj, nameof(VerifyRequirement), new object[] { requirement }, out result))
            {
                Event error = new Event { Message = $"Verification failed because requirement of type {requirement.GetType().Name} is currently not supported.", Type = EventType.Error };
                BH.Engine.Base.Compute.RecordEvent(error);
                return new RequirementResult(requirement.IIdentifier(), obj.IIdentifier(), null, new List<Event> { error });
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
                Event error = new Event { Message = $"Requirement {requirement.Name} is null or its condition contains nulls.", Type = EventType.Error };
                BH.Engine.Base.Compute.RecordEvent(error);
                return new RequirementResult(requirement?.IIdentifier(), obj?.IIdentifier(), null, new List<Event> { error });
            }

            IComparable requirementId = null;
            IComparable objId = null;
            IConditionResult conditionResult = null;
            List<Event> events = new List<Event>();
            DateTime start = DateTime.UtcNow;
            try
            {
                requirementId = requirement.IIdentifier();
                objId = obj?.IIdentifier();
                conditionResult = obj.IVerifyCondition(requirement.Condition);
            }
            catch (Exception ex)
            {
                BH.Engine.Base.Compute.RecordError($"Verification failed due to an exception: {ex.Message}");
            }
            finally
            {
                events = BH.Engine.Base.Query.EventsSince(start);
                BH.Engine.Base.Compute.RemoveEvents(events);
            }

            return new RequirementResult(requirementId, objId, conditionResult, events);
        }

        /***************************************************/
    }
}
