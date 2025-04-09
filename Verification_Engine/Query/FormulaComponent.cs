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
using System.ComponentModel;

namespace BH.Engine.Verification
{
    public static partial class Query
    {
        /***************************************************/
        /****             Interface Methods             ****/
        /***************************************************/

        [Description("Unpacks the object in preparation to solve a formula.")]
        [Input("obj", "Object to unpack.")]
        [Output("unpacked", "Input object unpacked and ready to use in formula solution.")]
        public static object IFormulaComponent(this object obj)
        {
            if (obj == null)
                return null;

            return BH.Engine.Base.Compute.RunExtensionMethod(obj, nameof(FormulaComponent));
        }


        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Unpacks the object in preparation to solve a formula.")]
        [Input("obj", "Object to unpack.")]
        [Output("unpacked", "Input object unpacked and ready to use in formula solution.")]
        public static object FormulaComponent(this object obj)
        {
            return obj;
        }

        /***************************************************/
    }
}
