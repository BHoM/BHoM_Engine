/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

using BH.oM.Reflection.Attributes;
using System.ComponentModel;
using BH.oM.MEP.Equipment.Parts;

namespace BH.Engine.MEP
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Returns an MEP Heating Coil part")]
        [Input("sensibleCapacity", "Default 0")]
        [Input("enteringDryBulbAirTemperature", "Default 0")]
        [Input("leavingDryBulbAirTemperature", "Default 0")]
        [Input("enteringWaterTemperature", "Default 0")]
        [Input("leavingWaterTemperature", "Default 0")]
        [Input("pressureDrop", "Default 0")]
        [Input("numberOfRows", "Default 0")]
        [Output("heatingCoil", "An MEP Heating Coil part")]
        public static HeatingCoil HeatingCoil(double sensibleCapacity = 0.0, double enteringDryBulbAirTemperature = 0.0, double leavingDryBulbAirTemperature = 0.0, double enteringWaterTemperature = 0.0, double leavingWaterTemperature = 0.0, double pressureDrop = 0.0, int numberOfRows = 0)
        {
            return new HeatingCoil
            {
                SensibleCapacity = sensibleCapacity,
                EnteringDryBulbAirTemperature = enteringDryBulbAirTemperature,
                LeavingDryBulbAirTemperature = leavingDryBulbAirTemperature,
                EnteringWaterTemperature = enteringWaterTemperature,
                LeavingWaterTemperature = leavingWaterTemperature,
                PressureDrop = pressureDrop,
                NumberOfRows = numberOfRows,
            };
        }
        /***************************************************/
    }
}

