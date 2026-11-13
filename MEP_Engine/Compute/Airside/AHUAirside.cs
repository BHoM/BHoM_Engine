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
using BH.oM.MEP.Airside;
using BH.oM.MEP.Results;

namespace BH.Engine.MEP
{
    public static partial class Compute
    {
        // Requires the existing foundation/component package, including AirsideMath.
        // No input mutation and no equipment selection or service allocation validation.
        public static AHUAirsideResult AHUAirside(
            AHUAirsideDefinition definition,
            AHUDesignCase designCase)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            if (designCase == null) throw new ArgumentNullException(nameof(designCase));

            var outdoor = new AirStream
            {
                State = designCase.OutdoorState,
                DryAirMassFlowRate = designCase.SupplyDryAirMassFlowRate
            };
            AirsideMath.Stream(outdoor);
            AirStream extract = null;
            if (definition.HeatRecovery != null)
            {
                if (designCase.ExtractInletState == null || !designCase.ExtractDryAirMassFlowRate.HasValue)
                    throw new ArgumentException("Heat recovery requires an explicit extract inlet state and dry-air mass flow.", nameof(designCase));
                extract = new AirStream
                {
                    State = designCase.ExtractInletState,
                    DryAirMassFlowRate = designCase.ExtractDryAirMassFlowRate.Value
                };
                AirsideMath.Stream(extract);
            }

            var result = new AHUAirsideResult
            {
                EquipmentId = definition.EquipmentId,
                AirsideDefinitionId = definition.BHoM_Guid,
                DesignCaseId = designCase.BHoM_Guid,
                OutdoorInlet = AirsideMath.Copy(outdoor)
            };
            AirStream current = AirsideMath.Copy(outdoor);
            double sumHeat = 0, sumMoisture = 0;

            if (definition.HeatRecovery != null)
            {
                result.RecoveryResult = HeatRecovery(current, extract, definition.HeatRecovery, designCase.BHoM_Guid);
                current = result.RecoveryResult.SupplyProcess.Outlet;
                result.ExtractLeavingRecovery = AirsideMath.Copy(result.RecoveryResult.ExtractProcess.Outlet);
                sumHeat += result.RecoveryResult.SupplyProcess.HeatTransferToAir;
                sumMoisture += result.RecoveryResult.SupplyProcess.MoistureTransferToAir;
                foreach (string message in result.RecoveryResult.CalculationMessages)
                    result.CalculationMessages.Add("Heat recovery: " + message);
            }
            else
            {
                result.CalculationMessages.Add("Heat recovery intentionally omitted. Extract inputs, if supplied, are unused and have not been validated. No exhaust state is predicted.");
            }

            if (definition.CoolingCoil != null)
            {
                var process = CoolingCoil(current, definition.CoolingCoil, designCase.BHoM_Guid);
                AHUAppendProcess(result, process, "Cooling", ref sumHeat, ref sumMoisture);
                current = process.Outlet;
            }
            if (definition.HeatingCoil != null)
            {
                var process = HeatingCoil(current, definition.HeatingCoil, designCase.BHoM_Guid);
                AHUAppendProcess(result, process, "Heating/reheat", ref sumHeat, ref sumMoisture);
                current = process.Outlet;
            }
            if (definition.SupplyFan != null)
            {
                var process = FanAirProcess(current, definition.SupplyFan, designCase.BHoM_Guid);
                AHUAppendProcess(result, process, "Supply fan", ref sumHeat, ref sumMoisture);
                current = process.Outlet;
            }

            result.FinalSupply = AirsideMath.Copy(current);
            result.NetSupplyHeatTransfer = outdoor.DryAirMassFlowRate *
                (Query.Enthalpy(current.State) - Query.Enthalpy(outdoor.State));
            result.NetSupplyMoistureTransfer = outdoor.DryAirMassFlowRate *
                (current.State.HumidityRatio - outdoor.State.HumidityRatio);
            result.SupplyEnergyBalanceResidual = result.NetSupplyHeatTransfer - sumHeat;
            result.SupplyMoistureBalanceResidual = result.NetSupplyMoistureTransfer - sumMoisture;

            if (Math.Abs(result.SupplyEnergyBalanceResidual) > 1e-6 + 1e-9 * Math.Abs(sumHeat)
                || Math.Abs(result.SupplyMoistureBalanceResidual) > 1e-12 + 1e-9 * Math.Abs(sumMoisture))
                throw new InvalidOperationException("AHU process chain failed its supply-side balance check.");

            result.CalculationMessages.Add("100% outdoor air; no recirculation, space response, duct gains/leakage, extract fan, humidifier, controls or water-side model. Pressure treatment and other limitations are inherited from the standalone component methods.");
            result.CalculationMessages.Add("Heating/cooling setpoints are component leaving targets. Final supply includes downstream fan heat; no automatic supply-temperature compensation is applied.");
            if (definition.EquipmentId == Guid.Empty)
                result.CalculationMessages.Add("AHU equipment is unassigned. Equipment references and service allocations have not been validated.");
            return result;
        }

        private static void AHUAppendProcess(AHUAirsideResult result, AirProcessResult process,
            string label, ref double heat, ref double moisture)
        {
            result.SupplyProcesses.Add(process);
            heat += process.HeatTransferToAir;
            moisture += process.MoistureTransferToAir;
            foreach (string message in process.CalculationMessages)
                result.CalculationMessages.Add(label + ": " + message);
        }
    }
}
