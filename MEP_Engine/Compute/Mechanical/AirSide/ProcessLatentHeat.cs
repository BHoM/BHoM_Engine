/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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

using System.ComponentModel;
using System.Data;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.MEP.Mechanical
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates the change in sensible heat during a process for air given volumetric flow rate and two humidityRatio points.")]
        [Input("volumetricFlowRate", "Volumetric fluid flow rate.", typeof(VolumetricFlowRate))]
        [Input("humidityRatioIn", "Entering humidity Ratio value [kg water/kg dry air].")]
        [Input("humidityRatioOut", "Leaving humidity Ratio value [kg water/kg dry air].")]
        [Input("latentHeatVaporizationWater", "Latent heat of vaporization of water value [kJ/kg].")]
        [Input("fluidDensity", "Fluid density value [kg/m3].")]
        [Output("latentHeat", "Latent heat value [kW].")]
        public static double AirProcessLatentHeat(double volumetricFlowRate, double humidityRatioIn, double humidityRatioOut, double latentHeatVaporizationWater = double.MinValue, double airDensity = double.MinValue)
        {
            if(volumetricFlowRate == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the latent heat from a null volumetricFlowRate value");
                return -1;
            }

            if(humidityRatioIn == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the latent heat from a null humidityRatioIn value");
                return -1;
            }

            if (humidityRatioOut == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the latent heat from a null humidityRatioOut value");
                return -1;
            }

            if (latentHeatVaporizationWater == double.MinValue)
            {
                BH.Engine.Base.Compute.RecordNote("Latent Heat of Vaporization of Water has been set to the default value of 2454 kJ/kg which is density of air at standard temperature and pressure.");
                latentHeatVaporizationWater = 2454;
            }

            if (airDensity == double.MinValue)
            {
                BH.Engine.Base.Compute.RecordNote("Air density has been set to the default value of 1.202 kg/m3 which is density of air at standard temperature and pressure.");
                airDensity = 1.202;
            }
            
            return latentHeatVaporizationWater * MassFlowRate(volumetricFlowRate, airDensity) * (humidityRatioIn-humidityRatioOut);
        }

        /***************************************************/

    }
}
