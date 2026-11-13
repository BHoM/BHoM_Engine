/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2026, the respective contributors. All rights reserved.
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 * The BHoM is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3.0 of the License, or
 * (at your option) any later version.
 * The BHoM is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 * GNU Lesser General Public License for more details.
 * You should have received a copy of the GNU Lesser General Public License
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.
 */
using System;
using System.ComponentModel;
using BH.oM.MEP.Airside;
using BH.oM.MEP.Criteria;
using BH.oM.MEP.Elements;
using BH.oM.MEP.Results;
namespace BH.Engine.MEP
{
    public static partial class Compute
    {
        [Description("Assigns the full final supply of one 100% outdoor-air AHU to one space. No branch splitting, room prediction, controls or equipment-capacity check.")]
        public static SpaceAirBalanceResult SpaceAirBalance(Space space, HVACSpaceCriteria criteria,
            SpaceConditioningDemand demand, AHUAirsideResult ahuResult,
            double? requiredOutdoorAirFlowAtRoomState = null, double airflowTolerance = 1e-9)
        {
            if (space == null || criteria == null || demand == null || ahuResult == null)
                throw new ArgumentNullException("Space, criteria, demand and AHU result are required.");
            if (space.BHoM_Guid == Guid.Empty || demand.SpaceId != space.BHoM_Guid)
                throw new ArgumentException("Demand must match canonical space identity.");
            if (demand.DesignCaseId == Guid.Empty || demand.DesignCaseId != ahuResult.DesignCaseId)
                throw new ArgumentException("Demand and AHU must have the same non-empty case ID.");
            if (ahuResult.AirsideDefinitionId == Guid.Empty) throw new ArgumentException("AHU definition reference is required.");
            if (string.IsNullOrWhiteSpace(demand.DemandBasis)) throw new ArgumentException("Demand basis must be explicit.");
            AirsideMath.Finite(demand.SensibleConditioningDemand, "SensibleConditioningDemand");
            if (!double.IsNaN(demand.MoistureConditioningDemand)) AirsideMath.Finite(demand.MoistureConditioningDemand, "MoistureConditioningDemand");
            AirsideMath.Stream(ahuResult.FinalSupply); AirsideMath.Stream(ahuResult.OutdoorInlet);
            if(ahuResult.BHoM_Guid==Guid.Empty || criteria.BHoM_Guid==Guid.Empty || demand.BHoM_Guid==Guid.Empty)
                throw new ArgumentException("Input record identities must be non-empty.");
            AirsideMath.SamePressure(ahuResult.FinalSupply.State.Pressure, ahuResult.OutdoorInlet.State.Pressure);
            double m = ahuResult.FinalSupply.DryAirMassFlowRate;
            if (Math.Abs(m - ahuResult.OutdoorInlet.DryAirMassFlowRate) > 1e-9 * Math.Max(1, m))
                throw new ArgumentException("Full supply must equal outdoor dry-air flow for this model.");
            AirsideMath.Finite(airflowTolerance, nameof(airflowTolerance));
            if (airflowTolerance < 0) throw new ArgumentOutOfRangeException(nameof(airflowTolerance));
            return SpaceAirBalanceCore(space, criteria, demand, ahuResult,
                ahuResult.FinalSupply, Guid.Empty, requiredOutdoorAirFlowAtRoomState, airflowTolerance);
        }

        private static SpaceAirBalanceResult SpaceAirBalanceCore(
            Space space, HVACSpaceCriteria criteria, SpaceConditioningDemand demand,
            AHUAirsideResult ahuResult, BH.oM.MEP.Airside.AirStream delivered,
            Guid allocationId, double? requiredOutdoorAirFlowAtRoomState, double airflowTolerance)
        {
            var room = Create.SpaceDesignAirState(criteria, demand.DesignCondition, delivered.State.Pressure);
            var supply = AirsideMath.Copy(delivered);
            double m = supply.DryAirMassFlowRate;
            double sensible = m * AirsideMath.Cp(supply.State.HumidityRatio) * (supply.State.DryBulbTemperature - room.DryBulbTemperature);
            double moisture = m * (supply.State.HumidityRatio - room.HumidityRatio);
            var r = new SpaceAirBalanceResult {
                SupplyAllocationId=allocationId, UsesFullAHUSupply=allocationId==Guid.Empty,
                AHUSupplyFraction=m/ahuResult.FinalSupply.DryAirMassFlowRate,
                SpaceId=space.BHoM_Guid, DesignCaseId=demand.DesignCaseId, CriteriaId=criteria.BHoM_Guid,
                DemandId=demand.BHoM_Guid, AHUResultId=ahuResult.BHoM_Guid, EquipmentId=ahuResult.EquipmentId,
                AirsideDefinitionId=ahuResult.AirsideDefinitionId, DesignCondition=demand.DesignCondition,
                RoomDesignState=room, DeliveredSupply=supply, SensibleConditioningDemand=demand.SensibleConditioningDemand,
                SupplySensibleContribution=sensible, ResidualSensibleDemand=demand.SensibleConditioningDemand-sensible,
                MoistureConditioningDemand=demand.MoistureConditioningDemand, SupplyMoistureContribution=moisture,
                ResidualMoistureDemand=double.IsNaN(demand.MoistureConditioningDemand)?double.NaN:demand.MoistureConditioningDemand-moisture,
                MoistureEnergyCoefficient=2501000+1860*room.DryBulbTemperature,
                DeliveredOutdoorAirFlowAtRoomState=m*AirsideMath.Volume(room)
            };
            AirsideMath.Finite(r.ResidualSensibleDemand,"ResidualSensibleDemand");
            AirsideMath.Finite(moisture,"SupplyMoistureContribution");
            if (!double.IsNaN(r.ResidualMoistureDemand)) AirsideMath.Finite(r.ResidualMoistureDemand,"ResidualMoistureDemand");
            if (requiredOutdoorAirFlowAtRoomState.HasValue) {
                double q=requiredOutdoorAirFlowAtRoomState.Value; AirsideMath.Finite(q,"requiredOutdoorAirFlowAtRoomState");
                if(q<0)throw new ArgumentOutOfRangeException(nameof(requiredOutdoorAirFlowAtRoomState));
                r.RequiredOutdoorAirFlowAtRoomState=q; r.OutdoorAirFlowMargin=r.DeliveredOutdoorAirFlowAtRoomState-q;
                r.OutdoorAirRequirementSatisfied=r.OutdoorAirFlowMargin>=-airflowTolerance;
            }
            r.CalculationMessages.Add(allocationId==Guid.Empty
                ? "Full AHU stream assigned to one space only; do not call once per branch with the same full stream. No duct gains, infiltration/transfer-air model, service validation or capacity proof."
                : "Allocated branch only; full allocation set reconciled to source AHU. No duct gains, leakage, transfer-air model or equipment-capacity proof. AHU coil duty remains a single system contribution.");
            r.CalculationMessages.Add("Sensible split uses Cp at supply humidity ratio. Moisture reporting coefficient at room temperature makes the sensible plus moisture-energy contribution equal the stream enthalpy difference.");
            r.CalculationMessages.Add("Room RH defines a reference state, not a humidity-control requirement. Residual signs are preserved. No terminal capacity selection.");
            if(!requiredOutdoorAirFlowAtRoomState.HasValue)r.CalculationMessages.Add("Outdoor-air requirement not assessed; caller must establish reference-state conversion.");
            return r;
        }
    }
}
