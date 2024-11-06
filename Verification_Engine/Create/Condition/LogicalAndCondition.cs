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

using BH.oM.Base.Attributes;
using BH.oM.Verification;
using BH.oM.Verification.Conditions;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Verification
{
    public static partial class Create
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Creates a logical AND condition based on a pair of conditions.")]
        [Input("condition1", "First condition to nest in the output logical condition.")]
        [Input("condition2", "Second condition to nest in the output logical condition.")]
        [Output("condition", nameof(LogicalAndCondition) + " created based on the provided inputs.")]
        public static LogicalAndCondition LogicalAndCondition(ICondition condition1, ICondition condition2)
        {
            return new LogicalAndCondition()
            {
                Conditions = new List<ICondition>() { condition1, condition2 },
            };
        }

        /***************************************************/
    }
}
