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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Base;
using BH.oM.Structure.Elements;
using BH.oM.Structure.Fragments;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Structure.Results;
using BH.oM.Base.Attributes;
using BH.Engine.Base;

namespace BH.Engine.Structure
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Calculates the total required area from BarRequiredArea summating the top, bottom, perimeter, shear and torsion reinforcement areas.")]
        [Input("barRequiredArea", "The BarRequiredArea to evaluate.")]
        [Output("total", "The total required area.")]
        public static double SumRequiredArea(this BarRequiredArea barRequiredArea)
        {
            return barRequiredArea.IsNull() ? 0 : barRequiredArea.Top + barRequiredArea.Bottom + barRequiredArea.Perimeter + barRequiredArea.Shear + barRequiredArea.Torsion;
        }

        /***************************************************/

        [Description("Calculates the total required area from MeshRequiredArea summating the top, bottom, perimeter, shear and torsion reinforcement areas.")]
        [Input("meshRequiredArea", "The BarRequiredArea to evaluate.")]
        [Output("total", "The total required area.")]
        public static double SumRequiredArea(this MeshRequiredArea meshRequiredArea)
        {
            return meshRequiredArea.IsNull() ? 0 : meshRequiredArea.TopPrimary + meshRequiredArea.TopSecondary + meshRequiredArea.BottomPrimary + meshRequiredArea.BottomSecondary + meshRequiredArea.Shear + meshRequiredArea.Torsion;
        }

        /***************************************************/


    }
}




