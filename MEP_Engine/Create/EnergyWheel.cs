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

        [Description("Returns an MEP Energy Wheel part")]
        [Input("outdoorSummerEnteringDryBulbTemperature", "Default 0")]
        [Input("outdoorSummerEnteringWetBulbTemperature", "Default 0")]
        [Input("outdoorSummerLeavingDryBulbTemperature", "Default 0")]
        [Input("outdoorSummerLeavingWetBulbTemperature", "Default 0")]
        [Input("reliefSummerEnteringDryBulbTemperature", "Default 0")]
        [Input("reliefSummerEnteringWetBulbTemperature", "Default 0")]
        [Input("reliefSummerLeavingDryBulbTemperature", "Default 0")]
        [Input("reliefSummerLeavingWetBulbTemperature", "Default 0")]
        [Input("summerSensibleEffectiveness", "Default 0")]
        [Input("summerTotalEffectiveness", "Default 0")]
        [Input("outdoorWinterEnteringDryBulbTemperature", "Default 0")]
        [Input("outdoorWinterEnteringWetBulbTemperature", "Default 0")]
        [Input("outdoorWinterLeavingDryBulbTemperature", "Default 0")]
        [Input("outdoorWinterLeavingWetBulbTemperature", "Default 0")]
        [Input("reliefWinterEnteringDryBulbTemperature", "Default 0")]
        [Input("reliefWinterEnteringWetBulbTemperature", "Default 0")]
        [Input("reliefWinterLeavingDryBulbTemperature", "Default 0")]
        [Input("reliefWinterLeavingWetBulbTemperature", "Default 0")]
        [Input("winterSensibleEffectiveness", "Default 0")]
        [Input("winterTotalEffectiveness", "Default 0")]
        [Input("type", "Default empty string")]
        [Input("control", "Default empty string")]
        [Output("energyWheel", "An MEP Energy Wheel part")]
        public static EnergyWheel EnergyWheel(double outdoorSummerEnteringDryBulbTemperature = 0, double outdoorSummerEnteringWetBulbTemperature = 0, double outdoorSummerLeavingDryBulbTemperature = 0, double outdoorSummerLeavingWetBulbTemperature = 0, double reliefSummerEnteringDryBulbTemperature = 0, double reliefSummerEnteringWetBulbTemperature = 0, double reliefSummerLeavingDryBulbTemperature = 0, double reliefSummerLeavingWetBulbTemperature = 0, double summerSensibleEffectiveness = 0, double summerTotalEffectiveness = 0, double outdoorWinterEnteringDryBulbTemperature = 0, double outdoorWinterEnteringWetBulbTemperature = 0, double outdoorWinterLeavingDryBulbTemperature = 0, double outdoorWinterLeavingWetBulbTemperature = 0, double reliefWinterEnteringDryBulbTemperature = 0, double reliefWinterEnteringWetBulbTemperature = 0, double reliefWinterLeavingDryBulbTemperature = 0, double reliefWinterLeavingWetBulbTemperature = 0, double winterSensibleEffectiveness = 0, double winterTotalEffectiveness = 0, string type = "", string control = "")
        {
            return new EnergyWheel
            {
                OutdoorSummerEnteringDryBulbTemperature = outdoorSummerEnteringDryBulbTemperature,
                OutdoorSummerEnteringWetBulbTemperature = outdoorSummerEnteringWetBulbTemperature,
                OutdoorSummerLeavingDryBulbTemperature = outdoorSummerLeavingDryBulbTemperature,
                OutdoorSummerLeavingWetBulbTemperature = outdoorSummerLeavingWetBulbTemperature,
                ReliefSummerEnteringDryBulbTemperature = reliefSummerEnteringDryBulbTemperature,
                ReliefSummerEnteringWetBulbTemperature = reliefSummerEnteringWetBulbTemperature,
                ReliefSummerLeavingDryBulbTemperature = reliefSummerLeavingDryBulbTemperature,
                ReliefSummerLeavingWetBulbTemperature = reliefSummerLeavingWetBulbTemperature,
                SummerSensibleEffectiveness = summerSensibleEffectiveness,
                SummerTotalEffectiveness = summerTotalEffectiveness,
                OutdoorWinterEnteringDryBulbTemperature = outdoorWinterEnteringDryBulbTemperature,
                OutdoorWinterEnteringWetBulbTemperature = outdoorWinterEnteringWetBulbTemperature,
                OutdoorWinterLeavingDryBulbTemperature = outdoorWinterLeavingDryBulbTemperature,
                OutdoorWinterLeavingWetBulbTemperature = outdoorWinterLeavingWetBulbTemperature,
                ReliefWinterEnteringDryBulbTemperature = reliefWinterEnteringDryBulbTemperature,
                ReliefWinterEnteringWetBulbTemperature = reliefWinterEnteringWetBulbTemperature,
                ReliefWinterLeavingDryBulbTemperature = reliefWinterLeavingDryBulbTemperature,
                ReliefWinterLeavingWetBulbTemperature = reliefWinterLeavingWetBulbTemperature,
                WinterSensibleEffectiveness = winterSensibleEffectiveness,
                WinterTotalEffectiveness = winterTotalEffectiveness,
                Type = type,
                Control = control,
            };
        }
    }
}
