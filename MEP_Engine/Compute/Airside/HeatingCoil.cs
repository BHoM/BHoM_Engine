/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
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
using BH.oM.MEP.Airside;
using BH.oM.MEP.Results;

namespace BH.Engine.MEP
{
    public static partial class Compute
    {
        public static AirProcessResult HeatingCoil(AirStream inlet, HeatingCoilSection section, Guid designCaseId = default(Guid))
        {
            AirsideMath.Stream(inlet);
            if (section == null) throw new ArgumentNullException(nameof(section));
            AirsideMath.Temperature(section.LeavingDryBulbTemperature);
            if (section.LeavingDryBulbTemperature < inlet.State.DryBulbTemperature)
                throw new ArgumentException("Heating target must not be colder than inlet.");
            var output = new AirStream { State = AirsideMath.NewState(section.LeavingDryBulbTemperature,
                inlet.State.HumidityRatio, inlet.State.Pressure), DryAirMassFlowRate = inlet.DryAirMassFlowRate };
            return new AirProcessResult { SectionId = section.BHoM_Guid, DesignCaseId = designCaseId,
                Inlet = AirsideMath.Copy(inlet), Outlet = output,
                HeatTransferToAir = inlet.DryAirMassFlowRate * (Query.Enthalpy(output.State) - Query.Enthalpy(inlet.State)),
                MoistureTransferToAir = 0 };
        }
    }
}
