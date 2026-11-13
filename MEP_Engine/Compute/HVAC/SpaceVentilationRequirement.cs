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
using System.Globalization;
using System.Linq;
using BH.oM.MEP.Airside;
using BH.oM.MEP.Criteria;
using BH.oM.MEP.Elements;
using BH.oM.MEP.Results;
using BH.oM.MEP.Systems;
namespace BH.Engine.MEP
{
    public static partial class Compute
    {
        [Description("Calculates one space outdoor-air requirement using existing rate priority, explicitly interpreting the volume at the room design state.")]
        public static SpaceVentilationRequirementResult SpaceVentilationRequirement(
            Space space, HVACSpaceCriteria criteria, Guid designCaseId,
            SpaceDesignCondition designCondition, double pressure = 101325)
        {
            if(space == null) throw new ArgumentNullException(nameof(space));
            if(criteria == null) throw new ArgumentNullException(nameof(criteria));
            return VentilationRequirementFromOutdoorAirFlows(space, criteria,
                OutdoorAirFlow(new[] { space }, criteria), designCaseId, designCondition, pressure);
        }

        [Description("Requires exactly one matching space flow; interprets its volume at the selected room design state. Caller establishes source criteria/case compatibility.")]
        public static SpaceVentilationRequirementResult VentilationRequirementFromOutdoorAirFlows(
            Space space, HVACSpaceCriteria criteria, IEnumerable<OutdoorAirFlow> flows,
            Guid designCaseId, SpaceDesignCondition designCondition, double pressure = 101325)
        {
            if(space == null) throw new ArgumentNullException(nameof(space));
            if(criteria == null) throw new ArgumentNullException(nameof(criteria));
            if(flows == null) throw new ArgumentNullException(nameof(flows));
            if(space.BHoM_Guid == Guid.Empty || criteria.BHoM_Guid == Guid.Empty || designCaseId == Guid.Empty)
                throw new ArgumentException("Non-empty space, criteria and case identities are required.");
            var items=flows.ToList();
            if(items.Count != 1 || items[0] == null) throw new ArgumentException("Exactly one valid outdoor-air result is required.", nameof(flows));
            var source=items[0];
            if(!(source.ObjectId is Guid id) || id != space.BHoM_Guid)
                throw new ArgumentException("Outdoor-air source must match canonical space Guid.", nameof(flows));
            AirsideMath.Finite(source.Flow, nameof(source.Flow));
            if(source.Flow < 0) throw new ArgumentOutOfRangeException(nameof(flows), "Flow cannot be negative.");
            var room=Create.SpaceDesignAirState(criteria, designCondition, pressure);
            AirsideMath.State(room);
            double volume=AirsideMath.Volume(room), mass=source.Flow/volume;
            AirsideMath.Finite(volume, nameof(volume)); AirsideMath.Finite(mass, nameof(mass));
            if(volume <= 0) throw new ArgumentOutOfRangeException(nameof(volume));
            return new SpaceVentilationRequirementResult
            {
                SpaceId=space.BHoM_Guid, CriteriaId=criteria.BHoM_Guid, DesignCaseId=designCaseId,
                DesignCondition=designCondition,
                ReferenceState=AirsideMath.NewState(room.DryBulbTemperature,room.HumidityRatio,room.Pressure),
                RequiredOutdoorAirFlowAtRoomState=source.Flow, ReferenceSpecificVolume=volume,
                RequiredDryAirMassFlowRate=mass, SourceResultCase=Convert.ToString(source.ResultCase,CultureInfo.InvariantCulture),
                Basis="Caller explicitly adopts room design state as volumetric reference; mass = volume / specific volume. Source ResultCase is provenance, not proof of design-case compatibility.",
                CalculationMessages=new List<string>
                {
                    "Requirement only; not regulatory compliance or thermally sized supply. No diversity applied.",
                    "Existing rate priority: positive ACH, then specified per-person, then specified per-area. ACH zero or NaN is unset; per-person/per-area zero is explicit. Invalid ACH rejects without fallback.",
                    "Zero requirement is retained; ventilation-sized zero-flow AHU operation is unsupported.",
                    "No source calculation Guid fabricated; source space and result-case label retained."
                }
            };
        }

        [Description("Selects strictly positive ventilation-only supply dry-air mass flow. Zero requirement is not silently inflated.")]
        public static double VentilationSupplyMassFlow(SpaceVentilationRequirementResult requirement)
        {
            ValidateVentilationRequirement(requirement);
            if(requirement.RequiredDryAirMassFlowRate == 0)
                throw new NotSupportedException("Zero ventilation requirement retained; zero-flow AHU operation is unsupported. Use an explicit positive flow or assess the requirement alone.");
            return requirement.RequiredDryAirMassFlowRate;
        }

        [Description("Validates ventilation identity and reference-state compatibility before the existing full-stream one-space balance.")]
        public static SpaceAirBalanceResult SpaceAirBalanceWithVentilation(
            Space space, HVACSpaceCriteria criteria, SpaceConditioningDemand demand,
            AHUAirsideResult ahuResult, SpaceVentilationRequirementResult ventilation,
            double airflowTolerance = 1e-9)
        {
            ValidateVentilationRequirement(ventilation);
            if(space == null || criteria == null || demand == null || ahuResult == null) throw new ArgumentNullException("Balance inputs are required.");
            if(ventilation.SpaceId != space.BHoM_Guid || ventilation.DesignCaseId != demand.DesignCaseId
                || ventilation.DesignCaseId != ahuResult.DesignCaseId || ventilation.CriteriaId != criteria.BHoM_Guid
                || ventilation.DesignCondition != demand.DesignCondition)
                throw new ArgumentException("Ventilation space, case, criteria and condition must match the room balance.");
            if(ahuResult.FinalSupply == null) throw new ArgumentException("Final supply is required.");
            AirsideMath.Stream(ahuResult.FinalSupply);
            var room=Create.SpaceDesignAirState(criteria,demand.DesignCondition,ahuResult.FinalSupply.State.Pressure);
            AirsideMath.SamePressure(room.Pressure,ventilation.ReferenceState.Pressure);
            if(Math.Abs(room.DryBulbTemperature-ventilation.ReferenceState.DryBulbTemperature)>1e-8
                || Math.Abs(room.HumidityRatio-ventilation.ReferenceState.HumidityRatio)>1e-12)
                throw new ArgumentException("Ventilation reference state differs from current room state; recalculate requirement.");
            var result=SpaceAirBalance(space,criteria,demand,ahuResult,ventilation.RequiredOutdoorAirFlowAtRoomState,airflowTolerance);
            result.CalculationMessages.Add("Outdoor-air comparison is against the supplied room-reference requirement, not general regulatory compliance.");
            return result;
        }
        private static void ValidateVentilationRequirement(SpaceVentilationRequirementResult r)
        {
            if(r == null) throw new ArgumentNullException(nameof(r));
            if(r.SpaceId==Guid.Empty || r.DesignCaseId==Guid.Empty || r.CriteriaId==Guid.Empty || string.IsNullOrWhiteSpace(r.Basis))
                throw new ArgumentException("Ventilation identities and basis are required.");
            AirsideMath.State(r.ReferenceState);
            AirsideMath.Finite(r.RequiredOutdoorAirFlowAtRoomState,"RequiredOutdoorAirFlowAtRoomState");
            AirsideMath.Finite(r.RequiredDryAirMassFlowRate,"RequiredDryAirMassFlowRate");
            AirsideMath.Finite(r.ReferenceSpecificVolume,"ReferenceSpecificVolume");
            if(r.RequiredOutdoorAirFlowAtRoomState<0 || r.RequiredDryAirMassFlowRate<0 || r.ReferenceSpecificVolume<=0)
                throw new ArgumentOutOfRangeException(nameof(r));
            double v=AirsideMath.Volume(r.ReferenceState), expected=r.RequiredOutdoorAirFlowAtRoomState/v;
            if(Math.Abs(v-r.ReferenceSpecificVolume)>1e-12+1e-9*Math.Abs(v)
                || Math.Abs(expected-r.RequiredDryAirMassFlowRate)>1e-12+1e-9*Math.Abs(expected))
                throw new ArgumentException("Ventilation conversion values are inconsistent; recalculate requirement.");
        }
    }
}
