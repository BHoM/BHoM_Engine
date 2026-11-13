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
using System.Linq;
using System.ComponentModel;
using BH.oM.MEP.Airside;
using BH.oM.MEP.Systems;
using BH.oM.MEP.Results;
namespace BH.Engine.MEP
{
    public static partial class Create
    {
        [Description("Allocates the actual cooling or heating-coil process duty, not net AHU enthalpy change or fan/recovery heat. Uses the existing definition to identify the section.")]
        public static ThermalDutyContribution AHUThermalDutyContribution(AHUAirsideResult result,
            AHUAirsideDefinition definition, ThermalDutyKind dutyKind, IEnumerable<ServiceGroup> groups,
            Guid explicitServiceGroupId, IEnumerable<EquipmentThermalServiceLink> defaultLinks)
        {
            if(result==null||definition==null)throw new ArgumentNullException("AHU result and definition required.");
            if(result.AirsideDefinitionId!=definition.BHoM_Guid||result.EquipmentId!=definition.EquipmentId)
                throw new ArgumentException("Result and definition identities differ.");
            Guid section; double duty; string label;
            if(dutyKind==ThermalDutyKind.Cooling) {
                if(definition.CoolingCoil==null)throw new ArgumentException("Cooling section omitted; no contribution created.");
                section=definition.CoolingCoil.BHoM_Guid;label="AHU.CoolingCoil";
                var matches=result.SupplyProcesses.Where(x=>x!=null&&x.SectionId==section).ToList();
                if(matches.Count!=1||!(matches[0] is CoolingCoilResult))throw new ArgumentException("Exactly one cooling-coil process required.");
                duty=((CoolingCoilResult)matches[0]).TotalCoolingDuty;
            } else if(dutyKind==ThermalDutyKind.Heating) {
                if(definition.HeatingCoil==null)throw new ArgumentException("Heating section omitted; no contribution created.");
                section=definition.HeatingCoil.BHoM_Guid;label="AHU.HeatingCoil";
                var matches=result.SupplyProcesses.Where(x=>x!=null&&x.SectionId==section).ToList();
                if(matches.Count!=1)throw new ArgumentException("Exactly one heating-coil process required.");
                duty=matches[0].HeatTransferToAir;
            } else throw new ArgumentException("Heating or cooling required.");
            return AllocatedThermalDuty(result.BHoM_Guid,label,result.DesignCaseId,dutyKind,duty,groups,
                explicitServiceGroupId,result.EquipmentId,defaultLinks);
        }
    }
}
