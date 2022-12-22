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

        [Description("Calculates the change in heat during a process for air given mass flow rate and two enthalpy points.")]
        [Input("volumetricFlowRate", "Mass flow rate of fluid (air) of the process. [KG Dry Air/s].")]
        [Input("enthalpyIn", "Entering enthalpy value for process [KJ/KG Dry Air.")]
        [Input("enthalpyOut", "Leaving enthalpy value for process [KJ/KG Dry Air.")]
        [Input("fluidDensity", "Fluid density value [kg/m3].")]
        [Output("totalHeat", "Total heat change value during process [kW].")]
        public static double AirSideProcessHeatChange(double volumetricFlowRate, double enthalpyIn, double enthalpyOut, double fluidDensity = double.MinValue)
        {
            if(volumetricFlowRate == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the total heat from a null volumetricFlowRate value");
                return -1;
            }

            if(enthalpyIn == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the total heat from a null enthalpyIn value");
                return -1;
            }

            if (enthalpyOut == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the total heat from a null enthalpyOut value");
                return -1;
            }

            if (fluidDensity == double.MinValue)
            {
                BH.Engine.Base.Compute.RecordNote("Fluid density has been set to the default value of 1.202 kg/m3 " +
                   "which is density of air at standard temperature and pressure.");
                fluidDensity = 1.202;
            }

            return MassFlowRate(volumetricFlowRate, fluidDensity) * (enthalpyIn -enthalpyOut);
        }

        /***************************************************/

    }
}
