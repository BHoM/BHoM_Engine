/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2019, the respective contributors. All rights reserved.
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
using BH.oM.Reflection.Attributes;
using BH.oM.Structure.MaterialFragments;
using BH.oM.Physical.Materials;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Checks if a material contains a structural material fragment")]
        public static bool IsStructural(this Material material)
        {
            return material.Properties.Where(x => x is IStructuralMaterial).Count() == 1;
        }

        /***************************************************/

        [Description("Checks if a material contains a isotropic structural material fragment")]
        public static bool IsIsotropic(this Material material)
        {
            return material.Properties.Where(x => x is IIsotropic).Count() == 1;
        }

        /***************************************************/

        [Description("Checks if a material contains a orthotropic structural material fragment")]
        public static bool IsOrthotropic(this Material material)
        {
            return material.Properties.Where(x => x is IOrthotropic).Count() == 1;
        }

        /***************************************************/

        [Description("Checks if a material contains a steel material fragment")]
        public static bool IsSteel(this Material material)
        {
            return material.Properties.Where(x => x is Steel).Count() == 1;
        }

        /***************************************************/

        [Description("Checks if a material contains a soncrete material fragment")]
        public static bool IsConcrete(this Material material)
        {
            return material.Properties.Where(x => x is Concrete).Count() == 1;
        }

        /***************************************************/

        [Description("Checks if a material contains a aluminium material fragment")]
        public static bool IsAluminium(this Material material)
        {
            return material.Properties.Where(x => x is Aluminium).Count() == 1;
        }

        /***************************************************/

        [Description("Checks if a material contains a timer material fragment")]
        public static bool IsTimber(this Material material)
        {
            return material.Properties.Where(x => x is Timber).Count() == 1;
        }

        /***************************************************/
    }
}
