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
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.MEP.Mechanical
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates the change in heat during a process for water given volumetric flow rate and two temperature points." +
            "Per ASHRAE Handbook HVAC Systems and Equipment 2020 (SI Edition) Chapter 13 Hydronic Heating and Cooling.")]
        [Input("volumetricFlowRate",  "Volumetric fluid flow rate.", typeof(VolumetricFlowRate))]
        [Input("temperatureIn", "Entering temperature value.", typeof(Temperature))]
        [Input("temperatureOut", "Leaving temperature value.", typeof(Temperature))]
        [Output("totalHeat", "Total heat value.", typeof(Energy))]
        public static double WaterSideProcessSensibleHeat(double volumetricFlowRate, double temperatureIn, double temperatureOut, double fluidSpecificHeat = double.MinValue, double fluiddensity = double.MinValue)
        {
            if (volumetricFlowRate == double.NaN)
            {
                Base.Compute.RecordError("Cannot compute the total heat from a null volumetricFlowRate value.");
                return -1;
            }

            if (temperatureIn == double.NaN)
            {
                Base.Compute.RecordError("Cannot compute the total heat from a null temperatureIn value.");
                return -1;
            }

            if (temperatureOut == double.NaN)
            {
                Base.Compute.RecordError("Cannot compute the total heat from a null temperatureOut value.");
                return -1;
            }

            if (fluiddensity == double.MinValue)
            {
                Base.Compute.RecordNote("Fluid density has been set to the default value of 1000 kg/m3 " +
                   "which is the value for water at standard temperature and pressure.");
                fluiddensity = 1000;
            }
            if (fluidSpecificHeat == double.MinValue)
            {
                Base.Compute.RecordNote("Fluid specific heat has been set to the default value of 4.18 kJ/kg-K " +
                    "which is the value for water at standard temperature and pressure.");
                fluidSpecificHeat = 4.18;
            }
            
            return fluidSpecificHeat * MassFlowRate(volumetricFlowRate,fluiddensity) * (temperatureIn- temperatureOut);
        }

        /***************************************************/

    }
}
