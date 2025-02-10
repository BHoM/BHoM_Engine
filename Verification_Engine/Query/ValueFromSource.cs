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
using System.ComponentModel;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        [Description("Extracts a value from an object based on the instruction embedded in the provided " + nameof(IValueSource) + ".")]
        [Input("obj", "Object to extract the value from.")]
        [Input("valueSource", "Object defining how to extract the value from the input object.")]
        [Output("value", "Value extracted from the input object based on the provided instruction.")]
        public static object ValueFromSource(this object obj, IValueSource valueSource)
        {
            return obj?.ITryGetValueFromSource(valueSource)?.Item2;
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Extracts a value from an object based on the instruction embedded in the provided " + nameof(IValueCondition) + ".")]
        [Input("obj", "Object to extract the value from.")]
        [Input("valueCondition", "Condition containing an object defining how to extract the value from the input object.")]
        [Output("value", "Value extracted from the input object based on the provided instruction.")]
        public static object ValueFromSource(this object obj, IValueCondition valueCondition)
        {
            return obj?.ValueFromSource(valueCondition?.ValueSource);
        }

        /***************************************************/
    }
}
