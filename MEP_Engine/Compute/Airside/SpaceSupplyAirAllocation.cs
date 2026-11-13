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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.MEP.Airside;
using BH.oM.MEP.Criteria;
using BH.oM.MEP.Elements;
using BH.oM.MEP.Results;
namespace BH.Engine.MEP
{
    public static partial class Compute
    {
        [Description("Creates a branch allocation from a validated ventilation requirement. Zero branch allowed; not a claim of thermal sizing.")]
        public static SpaceSupplyAirAllocation SpaceSupplyAirAllocationFromVentilation(
            SpaceVentilationRequirementResult ventilation, Guid equipmentId, Guid airsideDefinitionId)
        {
            ValidateVentilationRequirement(ventilation);
            if(equipmentId==Guid.Empty || airsideDefinitionId==Guid.Empty || ventilation.BHoM_Guid==Guid.Empty)
                throw new ArgumentException("Equipment, AHU definition and ventilation result IDs are required.");
            return new SpaceSupplyAirAllocation
            {
                SpaceId=ventilation.SpaceId, EquipmentId=equipmentId,
                AirsideDefinitionId=airsideDefinitionId, DesignCaseId=ventilation.DesignCaseId,
                DryAirMassFlowRate=ventilation.RequiredDryAirMassFlowRate,
                VentilationRequirementId=ventilation.BHoM_Guid,
                AllocationBasis="Ventilation-sized branch; source requirement interpreted at its own room reference state. No thermal sizing or diversity."
            };
        }

        [Description("Validates one complete branch set against an explicitly declared expected space set and returns summed dry-air mass flow. No implicit filtering or proportional redistribution.")]
        public static double AHUSupplyMassFlowFromAllocations(
            IEnumerable<SpaceSupplyAirAllocation> allocations, IEnumerable<Guid> expectedSpaceIds,
            Guid equipmentId, Guid airsideDefinitionId, Guid designCaseId)
        {
            var items=ValidatedSupplyAllocations(allocations,expectedSpaceIds,equipmentId,airsideDefinitionId,designCaseId);
            return PositiveAllocationTotal(items);
        }

        [Description("Balances one room against its allocated branch, after validating the entire supplied allocation set against expected membership and the real AHU flow. Retains real AHU identity; no synthetic branch AHU result.")]
        public static SpaceAirBalanceResult SpaceAirBalanceFromAllocations(
            Space space, HVACSpaceCriteria criteria, SpaceConditioningDemand demand,
            AHUAirsideResult ahuResult, IEnumerable<SpaceSupplyAirAllocation> allocations,
            IEnumerable<Guid> expectedSpaceIds, SpaceVentilationRequirementResult ventilation = null,
            double airflowTolerance = 1e-9, double massFlowTolerance = 1e-9)
        {
            if(space==null || criteria==null || demand==null || ahuResult==null)
                throw new ArgumentNullException("Space, criteria, demand and AHU result are required.");
            if(space.BHoM_Guid==Guid.Empty || demand.SpaceId!=space.BHoM_Guid)
                throw new ArgumentException("Demand must match canonical space identity.");
            if(demand.DesignCaseId==Guid.Empty || demand.DesignCaseId!=ahuResult.DesignCaseId)
                throw new ArgumentException("Demand and AHU must have the same non-empty case ID.");
            if(ahuResult.BHoM_Guid==Guid.Empty || criteria.BHoM_Guid==Guid.Empty || demand.BHoM_Guid==Guid.Empty)
                throw new ArgumentException("Input record identities must be non-empty.");
            if(string.IsNullOrWhiteSpace(demand.DemandBasis))throw new ArgumentException("Demand basis must be explicit.");
            AirsideMath.Finite(demand.SensibleConditioningDemand,"SensibleConditioningDemand");
            if(!double.IsNaN(demand.MoistureConditioningDemand))AirsideMath.Finite(demand.MoistureConditioningDemand,"MoistureConditioningDemand");
            AirsideMath.Finite(airflowTolerance,nameof(airflowTolerance));
            AirsideMath.Finite(massFlowTolerance,nameof(massFlowTolerance));
            if(airflowTolerance<0 || massFlowTolerance<0)throw new ArgumentOutOfRangeException("Tolerances must be nonnegative.");
            AirsideMath.Stream(ahuResult.FinalSupply);AirsideMath.Stream(ahuResult.OutdoorInlet);
            AirsideMath.SamePressure(ahuResult.FinalSupply.State.Pressure,ahuResult.OutdoorInlet.State.Pressure);
            double ahuMass=ahuResult.FinalSupply.DryAirMassFlowRate;
            if(Math.Abs(ahuMass-ahuResult.OutdoorInlet.DryAirMassFlowRate)>massFlowTolerance*Math.Max(1,ahuMass))
                throw new ArgumentException("100% outdoor-air AHU inlet and final supply mass flows must agree.");
            var items=ValidatedSupplyAllocations(allocations,expectedSpaceIds,ahuResult.EquipmentId,ahuResult.AirsideDefinitionId,ahuResult.DesignCaseId);
            double total=PositiveAllocationTotal(items);
            if(Math.Abs(total-ahuMass)>massFlowTolerance*Math.Max(1,ahuMass))
                throw new ArgumentException("Allocated branch mass flows do not reconcile with AHU final supply.");
            var matching=items.Where(x=>x.SpaceId==space.BHoM_Guid).ToList();
            if(matching.Count!=1)throw new ArgumentException("Exactly one allocation for the assessed space is required.");
            var branch=matching[0];
            double? required=null;
            if(ventilation!=null)
            {
                ValidateVentilationRequirement(ventilation);
                if(ventilation.SpaceId!=space.BHoM_Guid || ventilation.DesignCaseId!=demand.DesignCaseId
                    || ventilation.CriteriaId!=criteria.BHoM_Guid || ventilation.DesignCondition!=demand.DesignCondition)
                    throw new ArgumentException("Ventilation identities and condition must match branch balance.");
                if(branch.VentilationRequirementId!=Guid.Empty && branch.VentilationRequirementId!=ventilation.BHoM_Guid)
                    throw new ArgumentException("Branch source ventilation result does not match supplied requirement.");
                var room=Create.SpaceDesignAirState(criteria,demand.DesignCondition,ahuResult.FinalSupply.State.Pressure);
                AirsideMath.SamePressure(room.Pressure,ventilation.ReferenceState.Pressure);
                if(Math.Abs(room.DryBulbTemperature-ventilation.ReferenceState.DryBulbTemperature)>1e-8
                    || Math.Abs(room.HumidityRatio-ventilation.ReferenceState.HumidityRatio)>1e-12)
                    throw new ArgumentException("Ventilation reference state differs from current room state; recalculate requirement.");
                required=ventilation.RequiredOutdoorAirFlowAtRoomState;
            }
            var delivered=AirsideMath.Copy(ahuResult.FinalSupply);
            delivered.DryAirMassFlowRate=branch.DryAirMassFlowRate;
            // Zero branch has no delivered conditioning; the AHU total remains positive.
            AirsideMath.Stream(delivered,true);
            var r=SpaceAirBalanceCore(space,criteria,demand,ahuResult,delivered,branch.BHoM_Guid,required,airflowTolerance);
            r.CalculationMessages.Add("Branch allocation basis: "+branch.AllocationBasis);
            r.CalculationMessages.Add("Expected membership supplied by caller; this check does not resolve service-group responsibility. Full AHU thermal contribution must be included once only.");
            if(ventilation!=null)r.CalculationMessages.Add("Outdoor-air comparison against supplied room-reference requirement, not general regulatory compliance.");
            return r;
        }
        private static List<SpaceSupplyAirAllocation> ValidatedSupplyAllocations(
            IEnumerable<SpaceSupplyAirAllocation> allocations,IEnumerable<Guid> expectedSpaceIds,
            Guid equipmentId,Guid airsideDefinitionId,Guid designCaseId)
        {
            if(allocations==null)throw new ArgumentNullException(nameof(allocations));
            if(expectedSpaceIds==null)throw new ArgumentNullException(nameof(expectedSpaceIds));
            if(equipmentId==Guid.Empty || airsideDefinitionId==Guid.Empty || designCaseId==Guid.Empty)
                throw new ArgumentException("Non-empty AHU equipment, definition and case IDs required.");
            var expected=expectedSpaceIds.ToList();var items=allocations.ToList();
            if(expected.Count==0 || expected.Any(x=>x==Guid.Empty) || expected.Distinct().Count()!=expected.Count)
                throw new ArgumentException("Expected space set must be non-empty, unique and contain valid IDs.");
            if(items.Any(x=>x==null))throw new ArgumentException("Null branch allocation is invalid.");
            if(items.Any(x=>x.BHoM_Guid==Guid.Empty || x.SpaceId==Guid.Empty || string.IsNullOrWhiteSpace(x.AllocationBasis)))
                throw new ArgumentException("Branch allocation, space identities and allocation basis are required.");
            if(items.Select(x=>x.BHoM_Guid).Distinct().Count()!=items.Count || items.Select(x=>x.SpaceId).Distinct().Count()!=items.Count)
                throw new ArgumentException("Duplicate branch allocation or space; possible double counting.");
            foreach(var a in items)
            {
                if(a.EquipmentId!=equipmentId || a.AirsideDefinitionId!=airsideDefinitionId || a.DesignCaseId!=designCaseId)
                    throw new ArgumentException("All branches must match AHU equipment, definition and design case.");
                AirsideMath.Finite(a.DryAirMassFlowRate,"DryAirMassFlowRate");
                if(a.DryAirMassFlowRate<0)throw new ArgumentOutOfRangeException(nameof(allocations),"Branch flow cannot be negative.");
            }
            if(!new HashSet<Guid>(expected).SetEquals(items.Select(x=>x.SpaceId)))
                throw new ArgumentException("Branch allocation set does not match expected spaces; missing or unexpected branch.");
            return items;
        }
        private static double PositiveAllocationTotal(List<SpaceSupplyAirAllocation> items)
        {
            double sum=items.Sum(x=>x.DryAirMassFlowRate);AirsideMath.Finite(sum,"TotalDryAirMassFlowRate");
            if(sum<=0)throw new NotSupportedException("Positive total AHU flow required; zero-flow AHU operation unsupported.");
            return sum;
        }
    }
}
