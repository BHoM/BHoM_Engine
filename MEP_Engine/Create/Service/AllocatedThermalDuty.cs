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
using BH.oM.MEP.Systems;
namespace BH.Engine.MEP
{
    public static partial class Create
    {
        [Description("Assigns thermal duty explicitly, otherwise inherits an unambiguous corresponding water-service link from the serving AHU. Missing/ambiguous default stays unresolved. No equipment selection.")]
        public static ThermalDutyContribution AllocatedThermalDuty(Guid sourceResultId, string sourceComponent,
            Guid designCaseId, ThermalDutyKind dutyKind, double duty, IEnumerable<ServiceGroup> groups,
            Guid explicitServiceGroupId, Guid servingEquipmentId, IEnumerable<EquipmentThermalServiceLink> defaultLinks,
            Guid spaceId = default(Guid))
        {
            if(sourceResultId==Guid.Empty||designCaseId==Guid.Empty||string.IsNullOrWhiteSpace(sourceComponent))throw new ArgumentException("Source result, component and case required.");
            if(dutyKind!=ThermalDutyKind.Heating&&dutyKind!=ThermalDutyKind.Cooling)throw new ArgumentException("Heating or cooling required.");
            AirsideMath.Finite(duty,nameof(duty));if(duty<0)throw new ArgumentOutOfRangeException(nameof(duty));
            if(groups==null)throw new ArgumentNullException(nameof(groups));
            var known=groups.ToList();
            if(known.Any(x=>x==null||x.BHoM_Guid==Guid.Empty)||known.GroupBy(x=>x.BHoM_Guid).Any(g=>g.Count()!=1))throw new ArgumentException("Known groups require unique non-empty identities.");
            var r=new ThermalDutyContribution {SourceResultId=sourceResultId,SourceComponent=sourceComponent,DesignCaseId=designCaseId,DutyKind=dutyKind,Duty=duty,SpaceId=spaceId};
            if(explicitServiceGroupId!=Guid.Empty) {
                if(!known.Any(x=>x.BHoM_Guid==explicitServiceGroupId))throw new ArgumentException("Explicit target group does not exist; no default fallback.");
                r.ServiceGroupId=explicitServiceGroupId;return r;
            }
            var links=(defaultLinks??Enumerable.Empty<EquipmentThermalServiceLink>()).Where(x=>x!=null&&x.EquipmentId==servingEquipmentId&&x.DutyKind==dutyKind).ToList();
            if(servingEquipmentId==Guid.Empty||links.Count!=1||links[0].ServiceGroupId==Guid.Empty||!known.Any(x=>x.BHoM_Guid==links[0].ServiceGroupId)) {
                r.CalculationMessages.Add("Assignment unresolved: provide an explicit group or one valid heating/cooling water-service link for the serving AHU.");return r;
            }
            r.ServiceGroupId=links[0].ServiceGroupId;r.UsedDefaultAssignment=true;
            r.CalculationMessages.Add("Inherited from the serving AHU's corresponding water-service link, not its airside service group.");return r;
        }
    }
}
