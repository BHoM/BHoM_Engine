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
using BH.oM.MEP.Results;
namespace BH.Engine.MEP
{
    public static partial class Compute
    {
        [Description("Aggregates positive contributions to one service group/kind/case. Rejects unresolved assignments in the selected case and duplicate source/component/kind keys. No diversity, peak envelope or capacity selection.")]
        public static ServiceGroupThermalDutyResult ServiceGroupThermalDuties(ServiceGroup group, Guid designCaseId,
            ThermalDutyKind dutyKind, IEnumerable<ThermalDutyContribution> contributions)
        {
            if(group==null||contributions==null)throw new ArgumentNullException("Group and contributions required.");
            if(group.BHoM_Guid==Guid.Empty||designCaseId==Guid.Empty)throw new ArgumentException("Non-empty group and case required.");
            if(dutyKind!=ThermalDutyKind.Heating&&dutyKind!=ThermalDutyKind.Cooling)throw new ArgumentException("Heating or cooling required.");
            var all=contributions.ToList();if(all.Any(x=>x==null))throw new ArgumentException("Null contribution cannot be silently excluded.");
            var selectedCase=all.Where(x=>x.DesignCaseId==designCaseId).ToList();
            if(selectedCase.Any(x=>x.ServiceGroupId==Guid.Empty))throw new ArgumentException("Unresolved contribution in this case; aggregate blocked.");
            if(selectedCase.GroupBy(x=>new {x.SourceResultId,x.SourceComponent,x.DutyKind}).Any(g=>g.Count()!=1))throw new ArgumentException("Duplicate source component; possible double counting.");
            var selected=selectedCase.Where(x=>x.ServiceGroupId==group.BHoM_Guid&&x.DutyKind==dutyKind).ToList();
            if(selected.Count==0)throw new ArgumentException("No matching contributions; do not interpret missing data as zero.");
            var r=new ServiceGroupThermalDutyResult {ServiceGroupId=group.BHoM_Guid,DesignCaseId=designCaseId,DutyKind=dutyKind,Duty=0};
            foreach(var c in selected) {
                if(c.SourceResultId==Guid.Empty||string.IsNullOrWhiteSpace(c.SourceComponent)||r.ContributionIds.Contains(c.BHoM_Guid))throw new ArgumentException("Invalid or repeated contribution identity.");
                AirsideMath.Finite(c.Duty,"Duty");if(c.Duty<0)throw new ArgumentException("Contributions must be nonnegative.");
                r.Duty+=c.Duty;r.ContributionIds.Add(c.BHoM_Guid);
                foreach(var message in c.CalculationMessages)r.CalculationMessages.Add(c.SourceComponent+": "+message);
            }
            AirsideMath.Finite(r.Duty,"aggregate duty");
            r.CalculationMessages.Add("Sum of supplied allocated contributions only. Caller must provide a complete non-overlapping set at compatible conditions. No claim of time-coincidence, diversity, resilience or plant selection.");
            return r;
        }
    }
}
