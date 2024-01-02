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

using BH.oM.Structure.Constraints;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;
using System.ComponentModel;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Gets the fixitiy values from a constraint as a bool array. The value returned will be true if the DOFType is fixed. Values returned in the following order: TransX, TranxY, TransZ, RotX, RotY, RotZ.")]
        [Input("constraint", "The constraint to check for fixities.")]
        [Output("springValues", "The fixity values from a constraint as a bool array, where true indicates a fixity, in the following order: TransX, TransY, TransZ, RotX, RotY, RotZ.")]
        public static bool[] Fixities(this Constraint6DOF constraint)
        {
            return constraint.IsNull() ? null : new bool[]
            {
                constraint.TranslationX == DOFType.Fixed,
                constraint.TranslationY == DOFType.Fixed,
                constraint.TranslationZ == DOFType.Fixed,
                constraint.RotationX == DOFType.Fixed,
                constraint.RotationY == DOFType.Fixed,
                constraint.RotationZ == DOFType.Fixed
            };
        }

        /***************************************************/
    }
}


