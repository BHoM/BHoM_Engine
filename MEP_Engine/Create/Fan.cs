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

        [Description("Returns an MEP Fan part.")]
        [Input("flowRate", "Default 0.")]
        [Input("externalStaticPressure", "Default 0.")]
        [Input("speed", "Default 0.")]
        [Input("driveType", "Default empty string.")]
        [Input("speedControl", "Default empty string.")]
        [Input("brakeHorsePower", "Default 0.")]
        [Input("horsePower", "Default 0.")]
        [Input("efficiency", "Default 0.")]
        [Output("fan", "An MEP Fan part.")]
        public static Fan Fan(double flowRate = 0.0, double externalStaticPressure = 0.0, double speed = 0.0, string driveType = "", string speedControl = "", double brakeHorsePower = 0.0, double horsePower = 0.0, double efficiency = 0)
        {
            return new Fan
            {
                FlowRate = flowRate,
                ExternalStaticPressure = externalStaticPressure,
                Speed = speed,
                DriveType = driveType,
                SpeedControl = speedControl,
                BrakeHorsePower = brakeHorsePower,
                HorsePower = horsePower,
                Efficiency = efficiency,
            };
        }
    }
}

