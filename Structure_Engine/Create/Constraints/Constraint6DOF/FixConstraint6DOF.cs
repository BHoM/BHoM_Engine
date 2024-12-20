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

using BH.oM.Structure.Constraints;
using System.Collections.Generic;
using BH.oM.Base.Attributes;
using BH.Engine.Base;
using System.ComponentModel;


namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Creates a fully fixed Constraint6DOF, i.e. a constraint that have all degrees of freedom, translational and rotational, fixed.")]
        [Input("name", "Name of the Constraint6DOF. Defaults to Fix. This is required by most structural analysis software to create the object.")]
        [Output("cons", "The created fully fixed Constraint6DOF.")]
        public static Constraint6DOF FixConstraint6DOF(string name = "Fix")
        {
            return new Constraint6DOF
            {
                Name = name,
                TranslationX = DOFType.Fixed,
                TranslationY = DOFType.Fixed,
                TranslationZ = DOFType.Fixed,
                RotationX = DOFType.Fixed,
                RotationY = DOFType.Fixed,
                RotationZ = DOFType.Fixed
            };
        }

        /***************************************************/
    }
}






