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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment;
using BHM = BH.oM.Environment.MaterialFragments;
using BH.oM.Physical.Constructions;

using BH.oM.Base.Attributes;
using System.ComponentModel;

using BH.oM.Physical.Materials;
using BH.oM.Environment.MaterialFragments;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns the average roughness of a construction")]
        [Input("construction", "A Construction object")]
        [Output("roughness", "The average roughness of the construction")]
        public static BHM.Roughness Roughness(this Construction construction)
        {
            if (construction == null || construction.Layers.Count == 0)
                return BHM.Roughness.Undefined;

            BHM.IEnvironmentMaterial firstSide = construction.Layers.First().Material.Properties.Where(x => x is BHM.IEnvironmentMaterial).FirstOrDefault() as BHM.IEnvironmentMaterial;
            BHM.IEnvironmentMaterial finalSide = construction.Layers.Last().Material.Properties.Where(x => x is BHM.IEnvironmentMaterial).FirstOrDefault() as BHM.IEnvironmentMaterial;

            if (firstSide == null || finalSide == null)
                return BHM.Roughness.Undefined;

            int totalRoughness = (int)firstSide.Roughness + (int)finalSide.Roughness;
            return (BHM.Roughness)(totalRoughness / 2);
        }

        [Description("Returns the numerical roughness of a material")]
        [Input("material", "A Material object")]
        [Output("roughness", "The numerical roughness of the material")]
        public static double Roughness(this Material material)
        {
            if (material == null)
                return 0.0;

            IEnvironmentMaterial materialProperties = material.Properties.Where(x => x is IEnvironmentMaterial).FirstOrDefault() as IEnvironmentMaterial;

            if (materialProperties == null)
                return 0.0;

            switch (materialProperties.Roughness)
            {
                case oM.Environment.MaterialFragments.Roughness.VerySmooth:
                    return 0.0;
                case oM.Environment.MaterialFragments.Roughness.MediumSmooth:
                    return 0.04;
                case oM.Environment.MaterialFragments.Roughness.Smooth:
                    return 0.08;
                case oM.Environment.MaterialFragments.Roughness.Rough:
                    return 0.12;
                case oM.Environment.MaterialFragments.Roughness.MediumRough:
                    return 0.16;
                case oM.Environment.MaterialFragments.Roughness.VeryRough:
                    return 0.2;
                default:
                    return 0.0;
            }
        }
    }
}





