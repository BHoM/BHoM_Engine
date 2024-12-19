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
using System.ComponentModel;

namespace BH.Engine.Verification
{
    public static partial class Create
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Creates a property value condition with comparison requirement set to 'less than'. If the property under given name is not found, other sources are also searched:" +
                     "\n- CustomData for key matching the input name (only for IBHoMObjects)" +
                     "\n- Fragment with type name matching the input name (only for IBHoMObjects)" +
                     "\n- Parameterless extension method with name matching the input name")]
        [Input("propertyName", "Name of the property (or CustomData key or Fragment type name or extension method name) to extract the value from.")]
        [Input("referenceValue", "Value to compare the extracted value against.")]
        [Input("tolerance", "Tolerance value to be applied in comparison.")]
        [Output("condition", nameof(ValueCondition) + " created based on the provided inputs.")]
        public static ValueCondition IsLessThan(string propertyName, object referenceValue, object tolerance = null)
        {
            return new ValueCondition
            {
                ValueSource = new PropertyValueSource { PropertyName = propertyName },
                ReferenceValue = referenceValue,
                ComparisonType = ValueComparisonType.LessThan,
                Tolerance = tolerance,
            };
        }

        /***************************************************/
    }
}
