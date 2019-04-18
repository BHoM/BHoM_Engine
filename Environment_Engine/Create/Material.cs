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
using BH.oM.Physical.Properties;

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
        [Input("density", "The density of the material, default 0.0")]
        [Output("An Environment Gas Material object")]
        public static Material GasMaterial(string name = "", double conductivity = 0.0, double specificHeat = 0.0, double additionalHeatTransfer = 0.0, double vapourDiffusionFactor = 0.0, string description = "", Absorptance absorptance = null, double convectionCoefficient = 0.0, Gas gas = Gas.Undefined, double density = 0.0)
        {
            GasMaterial gasProperties = new GasMaterial
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

            return BH.Engine.Physical.Create.Material(name, density, new List<IMaterialProperties>() { gasProperties });
        }

        [Description("BH.Engine.Environment.Create.SolidMaterial => Returns an Environment Solid Material object")]
        [Input("name", "The name of the solid material, default empty string")]
        [Input("conductivity", "The amount of conductivity the material should have, default 0.0")]
        [Input("specificHeat", "The unit of specific heat the material should have, default 0.0")]
        [Input("additionalHeatTransfer", "The amount of additional heat transfer the material should have, default 0.0")]
        [Input("vapourDiffusionFactor", "The amount of vapour diffusion factor the material should have, default 0.0")]
        [Input("description", "A description of this material, default empty string")]
        [Input("absorptance", "The absorptance factor of this material, default null")]
        [Input("solarReflectanceExternal", "The amount of external solar reflectance of this solid material, default 0.0")]
        [Input("solarReflectanceInternal", "The amount of internal solar reflectance of this solid material, default 0.0")]
        [Input("solarTransmittance", "The amount of solar transmittance of this solid material, default 0.0")]
        [Input("lightReflectanceExternal", "The amount of external light reflectance of this solid material, default 0.0")]
        [Input("lightReflectanceInternal", "The amount of internal light reflectance of this solid material, default 0.0")]
        [Input("lightTransmittance", "The amount of light transmittance of this solid material, default 0.0")]
        [Input("emissivityExternal", "The external emissivity of this solid material, default 0.0")]
        [Input("emissivityInternal", "The internal emissivity of this solid material, default 0.0")]
        [Input("transparency", "The percentage transparancy of this solid material, defined as being between 0 and 1 (0 being no transparancy, 1 being fully transparent), default 0.)")]
        [Input("ignoreInUValudCalculation", "Define whether or not this material should be ignored in any uValue calculations, default false")]
        [Input("density", "The density of the material, default 0.0")]
        [Output("An Environment Solid Material object")]
        public static Material SolidMaterial(string name = "", double conductivity = 0.0, double specificHeat = 0.0, double additionalHeatTransfer = 0.0, double vapourDiffusionFactor = 0.0, string description = "", Absorptance absorptance = null, double solarReflectanceExternal = 0.0, double solarReflectanceInternal = 0.0, double solarTransmittance = 0.0, double lightReflectanceExternal = 0.0, double lightReflectanceInternal = 0.0, double lightTransmittance = 0.0, double emissivityExternal = 0.0, double emissivityInternal = 0.0, double transparency = 0.0, bool ignoreInUValueCalculation = false, double density = 0.0)
        {
            SolidMaterial solidProperties = new SolidMaterial
            {
                Name = name,
                Conductivity = conductivity,
                SpecificHeat = specificHeat,
                AdditionalHeatTransfer = additionalHeatTransfer,
                VapourDiffusionFactor = vapourDiffusionFactor,
                Description = description,
                Absorptance = absorptance,
                SolarReflectanceExternal = solarReflectanceExternal,
                SolarReflectanceInternal = solarReflectanceInternal,
                SolarTransmittance = solarTransmittance,
                LightReflectanceExternal = lightReflectanceExternal,
                LightReflectanceInternal = lightReflectanceInternal,
                LightTransmittance = lightTransmittance,
                EmissivityExternal = emissivityExternal,
                EmissivityInternal = emissivityInternal,
                Transparency = transparency,
                IgnoreInUValueCalculation = ignoreInUValueCalculation,
            };

            return BH.Engine.Physical.Create.Material(name, density, new List<IMaterialProperties>() { solidProperties });
        }
    }
}
