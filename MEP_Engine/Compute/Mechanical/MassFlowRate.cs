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

using System.ComponentModel;
using System.Data;
using BH.Engine.Reflection;
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.MEP.Mechanical
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates the mass flow rate of a fluid from the volumetric flow rate and density of the fluid.")]
        [Input("volumetricFlowRate", "Volumetric fluid flow rate.", typeof(VolumetricFlowRate))]
        [Input("fluidDensity", "Fluid density value [kg/m3].")]
        [Output("massFlowRate", "Mass flow rate [m3/s].")]
        public static double MassFlowRate(double volumetricFlowRate, double fluidDensity)
        {
            if(volumetricFlowRate == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the latent heat from a null airflow value");
                return -1;
            }

            if (fluidDensity == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the latent heat from a null fluidDensity value");
                return -1;
            }

            return volumetricFlowRate * fluidDensity;
        }

        /***************************************************/

    }
}
