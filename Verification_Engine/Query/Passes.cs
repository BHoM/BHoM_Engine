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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        [Description("Verifies an object against a condition and returns result in a form of a Boolean.")]
        [Input("obj", "Object to check against the condition.")]
        [Input("condition", "Condition to check the object against.")]
        [Output("result", "True if the input object passed the condition, false otherwise. Null in case of inconclusive check.")]
        public static bool? IPasses(this object obj, ICondition condition)
        {
            if (condition is IsNotNull notNullCondition)
                return Passes(obj, notNullCondition);

            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition because it was null.");
                return null;
            }

            object result;
            if (!BH.Engine.Base.Compute.TryRunExtensionMethod(obj, nameof(Passes), new object[] { condition }, out result))
            {
                BH.Engine.Base.Compute.RecordError($"Condition check failed because condition of type {condition.GetType().Name} is currently not supported.");
                return null;
            }

            return (bool?)result;
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Verifies an object against " + nameof(IValueCondition) + " and returns result in a form of a Boolean.")]
        [Input("obj", "Object to check against the condition.")]
        [Input("condition", "Condition to check the object against.")]
        [Output("result", "True if the input object passed the condition, false otherwise. Null in case of inconclusive check.")]
        public static bool? Passes(this object obj, IValueCondition condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition because it was null.");
                return null;
            }

            return obj.IVerifyCondition(condition).Passed;
        }

        /***************************************************/

        [Description("Verifies an object against " + nameof(IsOfType) + " condition and returns result in a form of a Boolean.")]
        [Input("obj", "Object to check against the condition.")]
        [Input("condition", "Condition to check the object against.")]
        [Output("result", "True if the input object passed the condition, false otherwise. Null in case of inconclusive check.")]
        public static bool? Passes(this object obj, IsOfType condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition because it was null.");
                return null;
            }

            return obj.VerifyCondition(condition).Passed;
        }

        /***************************************************/

        [Description("Verifies an object against " + nameof(HasId) + " condition and returns result in a form of a Boolean.")]
        [Input("obj", "Object to check against the condition.")]
        [Input("condition", "Condition to check the object against.")]
        [Output("result", "True if the input object passed the condition, false otherwise. Null in case of inconclusive check.")]
        public static bool? Passes(this object obj, HasId condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition because it was null.");
                return null;
            }

            return obj.VerifyCondition(condition).Passed;
        }

        /***************************************************/

        [Description("Verifies an object against " + nameof(IsNotNull) + " condition and returns result in a form of a Boolean.")]
        [Input("obj", "Object to check against the condition.")]
        [Input("condition", "Condition to check the object against.")]
        [Output("result", "True if the input object passed the condition, false otherwise. Null in case of inconclusive check.")]
        public static bool? Passes(this object obj, IsNotNull condition)
        {
            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition because it was null.");
                return null;
            }

            return obj != null;
        }

        /***************************************************/

        [Description("Verifies an object against " + nameof(LogicalNotCondition) + " and returns result in a form of a Boolean.")]
        [Input("obj", "Object to check against the condition.")]
        [Input("condition", "Condition to check the object against.")]
        [Output("result", "True if the input object passed the condition, false otherwise. Null in case of inconclusive check.")]
        public static bool? Passes(this object obj, LogicalNotCondition condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition because it was null.");
                return null;
            }

            return !obj.IPasses(condition.Condition);
        }

        /***************************************************/

        [Description("Verifies an object against " + nameof(LogicalAndCondition) + " and returns result in a form of a Boolean.")]
        [Input("obj", "Object to check against the condition.")]
        [Input("condition", "Condition to check the object against.")]
        [Output("result", "True if the input object passed the condition, false otherwise. Null in case of inconclusive check.")]
        public static bool? Passes(this object obj, LogicalAndCondition condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition because it was null.");
                return null;
            }

            List<bool?> subResults = condition.Conditions.Select(x => obj.IPasses(x)).ToList();
            if (subResults.Any(x => x == null))
                return null;
            else
                return subResults.All(x => x == true);
        }

        /***************************************************/

        [Description("Verifies an object against " + nameof(LogicalOrCondition) + " and returns result in a form of a Boolean.")]
        [Input("obj", "Object to check against the condition.")]
        [Input("condition", "Condition to check the object against.")]
        [Output("result", "True if the input object passed the condition, false otherwise. Null in case of inconclusive check.")]
        public static bool? Passes(this object obj, LogicalOrCondition condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition because it was null.");
                return null;
            }

            return condition.Conditions.Any(x => obj.IPasses(x) == true);
        }

        /***************************************************/

        [Description("Verifies an object against " + nameof(FormulaCondition) + " and returns result in a form of a Boolean.")]
        [Input("obj", "Object to check against the condition.")]
        [Input("condition", "Condition to check the object against.")]
        [Output("result", "True if the input object passed the condition, false otherwise. Null in case of inconclusive check.")]
        public static bool? Passes(this object obj, FormulaCondition condition)
        {
            if (obj == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition against a null object.");
                return null;
            }

            if (condition == null)
            {
                BH.Engine.Base.Compute.RecordError("Could not check condition because it was null.");
                return null;
            }

            return obj.VerifyCondition(condition).Passed;
        }

        /***************************************************/
    }
}

