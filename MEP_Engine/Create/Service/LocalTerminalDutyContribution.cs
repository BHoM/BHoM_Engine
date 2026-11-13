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
using BH.oM.MEP.Systems;
using BH.oM.MEP.Results;
namespace BH.Engine.MEP
{
    public static partial class Create
    {
        [Description("Creates positive local thermal duty from signed residuals. Cooling includes moisture removal by default and rejects unknown moisture demand. No physical FCU required.")]
        public static ThermalDutyContribution LocalTerminalDutyContribution(SpaceAirBalanceResult balance,
            ThermalDutyKind dutyKind, IEnumerable<ServiceGroup> groups, Guid explicitServiceGroupId,
            IEnumerable<EquipmentThermalServiceLink> defaultLinks, bool includeMoistureCooling = true)
        {
            if(balance==null)throw new ArgumentNullException(nameof(balance));
            if(balance.SpaceId==Guid.Empty)throw new ArgumentException("Source space required.");
            AirsideMath.Finite(balance.ResidualSensibleDemand,"ResidualSensibleDemand");
            double duty;
            if(dutyKind==ThermalDutyKind.Heating)duty=Math.Max(0,balance.ResidualSensibleDemand);
            else if(dutyKind==ThermalDutyKind.Cooling) {
                duty=Math.Max(0,-balance.ResidualSensibleDemand);
                if(includeMoistureCooling) {
                    AirsideMath.Finite(balance.ResidualMoistureDemand,"ResidualMoistureDemand");AirsideMath.Finite(balance.MoistureEnergyCoefficient,"MoistureEnergyCoefficient");
                    if(balance.MoistureEnergyCoefficient<=0)throw new ArgumentException("Positive moisture reporting coefficient required.");
                    duty+=Math.Max(0,-balance.ResidualMoistureDemand)*balance.MoistureEnergyCoefficient;
                }
            }else throw new ArgumentException("Heating or cooling required.");
            var r=AllocatedThermalDuty(balance.BHoM_Guid,dutyKind==ThermalDutyKind.Heating?"LocalHeating":"LocalCooling",
                balance.DesignCaseId,dutyKind,duty,groups,explicitServiceGroupId,balance.EquipmentId,defaultLinks,balance.SpaceId);
            r.CalculationMessages.Add("Thermal requirement only; no terminal selection, wet/dry screening or acoustic verification. Humidification duty is not assigned as hydronic heating.");
            if(dutyKind==ThermalDutyKind.Cooling&&!includeMoistureCooling)r.CalculationMessages.Add("SENSIBLE-ONLY cooling contribution: moisture demand explicitly excluded.");
            return r;
        }
    }
}
