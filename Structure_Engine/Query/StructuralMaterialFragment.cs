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

        [Description("Gets a structural material fragment from a material class")]
        public static IMaterialFragment StructuralMaterialFragment(this Material material)
        {
            if (!material.IsStructural())
            {
                Engine.Reflection.Compute.RecordWarning("Material with name " + material.Name + " does not contain an structural material fragment");
                return null;
            }
            
            return material.Properties.Where(x => x is IMaterialFragment).Cast<IMaterialFragment>().FirstOrDefault();
        }

        /***************************************************/

        [Description("Gets a isotropic structural material fragment from a material class")]
        public static IIsotropic IsotropicMaterialFragment(this Material material)
        {
            if (!material.IsIsotropic())
            {
                Engine.Reflection.Compute.RecordWarning("Material with name " + material.Name + " does not contain an isotropic structural material fragment");
                return null;
            }
            return material.Properties.Where(x => x is IIsotropic).Cast<IIsotropic>().FirstOrDefault();
        }

        /***************************************************/

        [Description("Gets a Orthotropic structural material fragment from a material class")]
        public static IOrthotropic OrthotropicMaterialFragment(this Material material)
        {
            if (!material.IsOrthotropic())
            {
                Engine.Reflection.Compute.RecordWarning("Material with name " + material.Name + " does not contain an orthotropic structural material fragment");
                return null;
            }
            return material.Properties.Where(x => x is IOrthotropic).Cast<IOrthotropic>().FirstOrDefault();
        }

        /***************************************************/
    }
}
