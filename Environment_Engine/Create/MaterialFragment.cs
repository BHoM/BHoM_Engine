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

using BH.oM.Environment.MaterialFragments;
using BH.oM.Physical.Materials;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns an Environment Gas Material object")]
        [Input("density", "The density of the material, default 0.0")]
        [Input("name", "The name of the gas material, default empty string")]
        [Input("conductivity", "The amount of conductivity the material should have, default 0.0")]
        [Input("specificHeat", "The unit of specific heat the material should have, default 0.0")]
        [Input("vapourResistivity", "The amount of vapour resistance the material should have, default 0.0")]
        [Input("description", "A description of this material, default empty string")]
        [Input("roughness", "The roughness of the material from the Material Roughness enum, default undefined")]
        [Input("refraction", "The refraction of the material, default 0.0")]
        [Input("convectionCoefficient", "The convection coefficient of this gas material, default 0.0")]
        [Input("gas", "The type of gas this material is from the Gas Type enum, default undefined")]
        [Output("material", "A material object containing a Gas Material Fragment")]
        [Deprecated("3.0", "Deprecated in favour of default create components produced by BHoM")]
        public static Material GasMaterial(string name = "", double density = 0.0, double conductivity = 0.0, double specificHeat = 0.0, double vapourResistivity = 0.0, string description = "", Roughness roughness = Roughness.Undefined, double refraction = 0.0, double convectionCoefficient = 0.0, Gas gas = Gas.Undefined)
        {
            GasMaterial gasProperties = new GasMaterial
            {
                Name = name,
                Density = density,
                Conductivity = conductivity,
                SpecificHeat = specificHeat,
                VapourResistivity = vapourResistivity,
                Description = description,
                Roughness = roughness,
                Refraction = refraction,
                ConvectionCoefficient = convectionCoefficient,
                Gas = gas,
            };

            return BH.Engine.Physical.Create.Material(name, new List<IMaterialProperties> { gasProperties });
        }

        [Description("Returns an Environment Solid Material object")]
        [Input("name", "The name of the solid material, default empty string")]
        [Input("density", "The density of the material, default 0.0")]
        [Input("conductivity", "The amount of conductivity the material should have, default 0.0")]
        [Input("specificHeat", "The unit of specific heat the material should have, default 0.0")]
        [Input("vapourResistivity", "The amount of vapour resistance the material should have, default 0.0")]
        [Input("description", "A description of this material, default empty string")]
        [Input("roughness", "The roughness of the material from the Material Roughness enum, default undefined")]
        [Input("refraction", "The refraction of the material, default 0.0")]
        [Input("solarReflectanceExternal", "The amount of external solar reflectance of this solid material, default 0.0")]
        [Input("solarReflectanceInternal", "The amount of internal solar reflectance of this solid material, default 0.0")]
        [Input("solarTransmittance", "The amount of solar transmittance of this solid material, default 0.0")]
        [Input("lightReflectanceExternal", "The amount of external light reflectance of this solid material, default 0.0")]
        [Input("lightReflectanceInternal", "The amount of internal light reflectance of this solid material, default 0.0")]
        [Input("lightTransmittance", "The amount of light transmittance of this solid material, default 0.0")]
        [Input("emissivityExternal", "The external emissivity of this solid material, default 0.0")]
        [Input("emissivityInternal", "The internal emissivity of this solid material, default 0.0")]
        [Input("specularity", "The specularity of the solid material, where specularity is the proportion of directed light reflected from the material, default 0.0")]
        [Input("transmittedDiffusivity", "The amount of diffused light transmitted through the solid material, default 0.0")]
        [Input("transmittedSpecularity", "The amount of directed light transmitted through the solid material, default 0.0")]
        [Input("ignoreInUValueCalculation", "Define whether or not this material should be ignored in any uValue calculations, default false")]
        [Output("material", "A material object containing a Solid Material Fragment")]
        [Deprecated("3.0", "Deprecated in favour of default create components produced by BHoM")]
        public static Material SolidMaterial(string name = "", double density = 0.0, double conductivity = 0.0, double specificHeat = 0.0, double additionalHeatTransfer = 0.0, double vapourResistivity = 0.0, string description = "", Roughness roughness = Roughness.Undefined, double refraction = 0.0, double solarReflectanceExternal = 0.0, double solarReflectanceInternal = 0.0, double solarTransmittance = 0.0, double lightReflectanceExternal = 0.0, double lightReflectanceInternal = 0.0, double lightTransmittance = 0.0, double emissivityExternal = 0.0, double emissivityInternal = 0.0, double specularity = 0.0, double transmittedDiffusivity = 0.0, double transmittedSpecularity = 0.0, bool ignoreInUValueCalculation = false)
        {
            SolidMaterial solidProperties = new SolidMaterial
            {
                Name = name,
                Density = density,
                Conductivity = conductivity,
                SpecificHeat = specificHeat,
                VapourResistivity = vapourResistivity,
                Description = description,
                Roughness = roughness,
                Refraction = refraction,
                SolarReflectanceExternal = solarReflectanceExternal,
                SolarReflectanceInternal = solarReflectanceInternal,
                SolarTransmittance = solarTransmittance,
                LightReflectanceExternal = lightReflectanceExternal,
                LightReflectanceInternal = lightReflectanceInternal,
                LightTransmittance = lightTransmittance,
                EmissivityExternal = emissivityExternal,
                EmissivityInternal = emissivityInternal,
                Specularity = specularity,
                TransmittedDiffusivity = transmittedDiffusivity,
                TransmittedSpecularity = transmittedSpecularity,
                IgnoreInUValueCalculation = ignoreInUValueCalculation,
            };

            return BH.Engine.Physical.Create.Material(name, new List<IMaterialProperties> { solidProperties });
        }
    }
}
