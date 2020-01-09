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

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

using BH.oM.MEP.Parts;

namespace BH.Engine.MEP
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns an MEP Electrical Connector part")]
        [Input("motorHorsePower", "Default 0")]
        [Input("brakeHorsePower", "Default 0")]
        [Input("fullLoadAmps", "Default 0")]
        [Input("maximumOvercurrentProtection", "Default 0")]
        [Input("phase", "Default 0")]
        [Input("frequency", "Default 0")]
        [Input("voltage", "Default 0")]
        [Input("emergencyPower", "Default false")]
        [Input("standBy", "Default false")]
        [Output("electricalConnector", "An MEP Electrical Connector part")]
        public static ElectricalConnector ElectricalConnector(double motorHorsePower = 0.0, double brakeHorsePower = 0.0, double fullLoadAmps = 0.0, double maximumOvercurrentProtection = 0.0, double phase = 0.0, double frequency = 0.0, double voltage = 0.0, bool emergencyPower = false, bool standBy = false)
        {
            return new ElectricalConnector
            {
                MotorHorsePower = motorHorsePower,
                BrakeHorsePower = brakeHorsePower,
                FullLoadAmps = fullLoadAmps,
                MaximumOvercurrentProtection = maximumOvercurrentProtection,
                Phase = phase,
                Frequency = frequency,
                Voltage = voltage,
                EmergencyPower = emergencyPower,
                StandBy = standBy,
            };
        }
    }
}
