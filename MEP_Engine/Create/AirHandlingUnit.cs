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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Reflection.Attributes;
using System.ComponentModel;

using BH.oM.MEP.Equipment;
using BH.oM.MEP.Parts;

namespace BH.Engine.MEP
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Returns an MEP Air Handling Unit object")]
        [Input("type", "The type of air handling unit, default empty string")]
        [Input("totalSupplyAirFlow", "Default 0")]
        [Input("totalSupplyAirExternalStaticPressure", "Default 0")]
        [Input("totalReturnAirFlow", "Default 0")]
        [Input("totalReturnAirExternalStaticPressure", "Default 0")]
        [Input("totalDesignOutdoorAirFlow", "Default 0")]
        [Input("demandControlledVentilationMinimumOutdoorAirFlow", "Default 0")]
        [Input("totalOutdoorAirFlowExternalStaticPressure", "Default 0")]
        [Input("totalReliefAirFlow", "Default 0")]
        [Input("totalReliefExternalStaticPressure", "Default 0")]
        [Input("supplyAirEcoomiser", "Default 0")]
        [Input("waterEconomiser", "Default 0")]
        [Input("parts", "A collection of MEP Parts which are housed by the air handling unit, default null")]
        [Output("airHandlingUnit", "An MEP Air Handling Unit")]
        public static AirHandlingUnit AirHandlingUnit(string type = "", double totalSupplyAirFlow = 0.0, double totalSupplyAirExternalStaticPressure = 0.0, double totalReturnAirFlow = 0.0, double totalReturnAirExternalStaticPressure = 0.0, double totalDesignOutdoorAirFlow = 0.0, double demandControlledVentilationMinimumOutdoorAirFlow = 0.0, double totalOutdoorAirFlowExternalStaticPressure = 0.0, double totalReliefAirFlow = 0.0, double totalReliefExternalStaticPressure = 0.0, bool supplyAirEconomiser = false, bool waterEconomiser = false, List<IPart> parts = null)
        {
            parts = parts ?? new List<IPart>();

            return new AirHandlingUnit
            {
                Type = type,
                TotalSupplyAirFlow = totalSupplyAirFlow,
                TotalSupplyAirExternalStaticPressure = totalSupplyAirExternalStaticPressure,
                TotalReturnAirFlow = totalReturnAirFlow,
                TotalReturnAirExternalStaticPressure = totalReturnAirExternalStaticPressure,
                TotalDesignOutdoorAirFlow = totalDesignOutdoorAirFlow,
                DemandControlledVentilationMinimumOutdoorAirFlow = demandControlledVentilationMinimumOutdoorAirFlow,
                TotalOutdoorAirFlowExternalStaticPressure = totalOutdoorAirFlowExternalStaticPressure,
                TotalReliefAirFlow = totalReliefAirFlow,
                TotalReliefExternalStaticPressure = totalReliefExternalStaticPressure,
                SupplyAirEconomiser = supplyAirEconomiser,
                WaterEconomiser = waterEconomiser,
                Parts = parts,
            };
        }
    }
}

