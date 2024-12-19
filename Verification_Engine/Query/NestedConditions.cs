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
using BH.oM.Verification.Conditions;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        [Description("Extracts the conditions nested inside a given condition, e.g. conditions nested inside logical conditions.")]
        [Input("condition", "Condition to extract the nested conditions from.")]
        [Output("nested", "Conditions nested inside the input condition.")]
        public static IEnumerable<ICondition> INestedConditions(this ICondition condition)
        {
            return NestedConditions(condition as dynamic);
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Extracts the conditions nested inside a given logical condition.")]
        [Input("condition", "Condition to extract the nested conditions from.")]
        [Output("nested", "Conditions nested inside the input condition.")]
        public static IEnumerable<ICondition> NestedConditions(this ILogicalCollectionCondition condition)
        {
            foreach (ICondition c in condition.Conditions)
            {
                foreach (ICondition nested in c.INestedConditions())
                {
                    yield return nested;
                }
            }
        }

        /***************************************************/

        [Description("Extracts the conditions nested inside a given logical condition.")]
        [Input("condition", "Condition to extract the nested conditions from.")]
        [Output("nested", "Conditions nested inside the input condition.")]
        public static IEnumerable<ICondition> NestedConditions(this ISingleLogicalCondition condition)
        {
            yield return condition.Condition;
        }


        /***************************************************/
        /****              Private Methods              ****/
        /***************************************************/

        private static IEnumerable<ICondition> NestedConditions(this ICondition condition)
        {
            yield return condition;
        }

        /***************************************************/
    }
}
