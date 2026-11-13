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
        public static CoolingCoilResult CoolingCoil(AirStream inlet, CoolingCoilSection section, Guid designCaseId = default(Guid))
        {
            AirsideMath.Stream(inlet);
            if (section == null) throw new ArgumentNullException(nameof(section));
            AirState state;
            if (section.CalculationMode == CoolingCoilCalculationMode.SpecifiedLeavingState)
            {
                if (section.ApparatusDewPoint.HasValue || section.BypassFactor.HasValue)
                    throw new ArgumentException("ADP/BPF inputs are incompatible with specified-state mode.");
                AirsideMath.State(section.RequiredLeavingState);
                state = AirsideMath.NewState(section.RequiredLeavingState.DryBulbTemperature,
                    section.RequiredLeavingState.HumidityRatio, section.RequiredLeavingState.Pressure);
            }
            else if (section.CalculationMode == CoolingCoilCalculationMode.ApparatusDewPointAndBypassFactor)
            {
                if (section.RequiredLeavingState != null || !section.ApparatusDewPoint.HasValue || !section.BypassFactor.HasValue)
                    throw new ArgumentException("ADP/BPF mode requires only an explicit ADP and bypass factor.");
                double adp = section.ApparatusDewPoint.Value, b = section.BypassFactor.Value;
                AirsideMath.Temperature(adp); AirsideMath.Fraction(b, "BypassFactor");
                if (adp < 0) throw new NotSupportedException("Frosting/subzero ADP coils are outside this model.");
                if (adp > inlet.State.DryBulbTemperature) throw new ArgumentException("ADP exceeds inlet temperature.");
                var contact = Create.AirStateFromRelativeHumidity(adp, 1, inlet.State.Pressure);
                if (contact.HumidityRatio > inlet.State.HumidityRatio + AirsideMath.HumidityTolerance)
                    throw new NotSupportedException("Dry-coil performance cannot be inferred from this wet-coil ADP/BPF model. Use specified leaving state.");
                double w = b * inlet.State.HumidityRatio + (1 - b) * contact.HumidityRatio;
                double h = b * Query.Enthalpy(inlet.State) + (1 - b) * Query.Enthalpy(contact);
                state = AirsideMath.NewState(AirsideMath.TFromHW(h, w), w, inlet.State.Pressure);
            }
            else throw new ArgumentException("A supported cooling calculation mode is required.");
            AirsideMath.SamePressure(inlet.State.Pressure, state.Pressure);
            if (state.DryBulbTemperature > inlet.State.DryBulbTemperature + 1e-8 || state.HumidityRatio > inlet.State.HumidityRatio + 1e-12)
                throw new ArgumentException("Cooling must not add heat or moisture; use separate reheat/humidification processes.");
            double q = inlet.DryAirMassFlowRate * (Query.Enthalpy(inlet.State) - Query.Enthalpy(state));
            if (q < -1e-6) throw new ArgumentException("Leaving enthalpy exceeds entering enthalpy.");
            double condensate = inlet.DryAirMassFlowRate * (inlet.State.HumidityRatio - state.HumidityRatio);
            var result = new CoolingCoilResult { SectionId = section.BHoM_Guid, DesignCaseId = designCaseId,
                Inlet = AirsideMath.Copy(inlet), Outlet = new AirStream { State = state, DryAirMassFlowRate = inlet.DryAirMassFlowRate },
                HeatTransferToAir = -q, MoistureTransferToAir = -condensate, TotalCoolingDuty = q,
                CondensateMassFlowRate = condensate, IsDehumidifying = condensate > 1e-10,
                AppliedApparatusDewPoint = section.ApparatusDewPoint, AppliedBypassFactor = section.BypassFactor };
            result.CalculationMessages.Add("Idealised air-stream duty; condensate enthalpy, coil geometry and water-side performance are not modelled.");
            if (section.CalculationMode == CoolingCoilCalculationMode.SpecifiedLeavingState)
                result.CalculationMessages.Add("Specified leaving state is a design requirement, not proof that a particular coil can achieve it.");
            return result;
        }
    }
}
