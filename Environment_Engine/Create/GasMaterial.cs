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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Materials;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("BH.Engine.Environment.Create.GasMaterial => Returns an Environment Gas Material object")]
        [Input("name", "The name of the gas material, default empty string")]
        [Input("conductivity", "The amount of conductivity the material should have, default 0.0")]
        [Input("specificHeat", "The unit of specific heat the material should have, default 0.0")]
        [Input("additionalHeatTransfer", "The amount of additional heat transfer the material should have, default 0.0")]
        [Input("vapourDiffusionFactor", "The amount of vapour diffusion factor the material should have, default 0.0")]
        [Input("description", "A description of this material, default empty string")]
        [Input("absorptance", "The absorptance factor of this material, default null")]
        [Input("convectionCoefficient", "The convection coefficient of this gas material, default 0.0")]
        [Input("gas", "The type of gas this material is from the Gas Type enum, default undefined")]
        [Output("An Environment Gas Material object")]
        public static GasMaterial GasMaterial(string name = "", double conductivity = 0.0, double specificHeat = 0.0, double additionalHeatTransfer = 0.0, double vapourDiffusionFactor = 0.0, string description = "", Absorptance absorptance = null, double convectionCoefficient = 0.0, Gas gas = Gas.Undefined)
        {
            return new GasMaterial
            {
                Name = name,
                Conductivity = conductivity,
                SpecificHeat = specificHeat,
                AdditionalHeatTransfer = additionalHeatTransfer,
                VapourDiffusionFactor = vapourDiffusionFactor,
                Description = description,
                Absorptance = absorptance,
                ConvectionCoefficient = convectionCoefficient,
                Gas = gas,
            };
        }
    }
}
