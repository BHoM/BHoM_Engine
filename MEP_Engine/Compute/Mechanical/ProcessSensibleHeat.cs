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
using BH.oM.Base.Attributes;
using BH.oM.Quantities.Attributes;

namespace BH.Engine.MEP.Mechanical
{
    public static partial class Compute
    {
        /***************************************************/
        /****   Public Methods                          ****/
        /***************************************************/

        [Description("Calculates the change in sensible heat during a process given mass flow rate and two temperature points.")]
        [Input("massFlowRate", "Mass flow rate [kg/s].")]
        [Input("temperatureIn", "Entering temperature value [C].")]
        [Input("temperatureOut", "Leavings temperature value [C].")]
        [Input("specificHeat", "Specific Heat value [kJ/kg-K].")]
        [Output("sensibleHeat", "Sensible heat value [kW].")]
        public static double ProcessSensibleHeat(double massFlowRate, double temperatureIn, double temperatureOut, double specificHeat)
        {
            if(massFlowRate == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the sensible heat from a null massFlowRate value.");
                return -1;
            }

            if(temperatureIn == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the sensible heat from a null temperatureIn value.");
                return -1;
            }

            if (temperatureOut == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the sensible heat from a null temperatureOut value.");
                return -1;
            }
            
            if (specificHeat == double.NaN)
            {
                BH.Engine.Base.Compute.RecordError("Cannot compute the sensible heat from a null specificHeat value.");
                return -1;
            }

            return specificHeat * massFlowRate * (temperatureIn - temperatureOut);
        }

        /***************************************************/

    }
}
