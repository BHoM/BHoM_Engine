/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Physical.Materials;
using BH.oM.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [ToBeRemoved("3.1","ToBeRemoved as method only used for early proof of concept, not in use any longer.")]
        [Description("Damping ratio of the material. Gets the damping ratio from the structural material fragment.")]
        [Output("ζ", "Damping ratio of the material. .")]
        public static double DampingRatio(this Material material)
        {
            IMaterialFragment fragment = material.StructuralMaterialFragment();
            return fragment != null ? fragment.DampingRatio : double.NaN;
        }

        /***************************************************/

    }
}




