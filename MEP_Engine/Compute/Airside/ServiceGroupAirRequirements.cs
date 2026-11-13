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
    public static partial class Compute
    {
        [Description("Strict sum of requirements for exactly the spaces allocated to a service group and role in one case. Missing/duplicate membership or requirements reject the aggregate. No diversity.")]
        public static ServiceGroupAirRequirementResult ServiceGroupAirRequirements(ServiceGroup group, Guid designCaseId,
            ServiceRole role, IEnumerable<ServiceGroupAllocation> allocations, IEnumerable<SpaceAirRequirement> requirements)
        {
            if(group==null||allocations==null||requirements==null)throw new ArgumentNullException("Group, allocations and requirements required.");
            if(group.BHoM_Guid==Guid.Empty||designCaseId==Guid.Empty||role==ServiceRole.Unspecified||!Enum.IsDefined(typeof(ServiceRole),role))
                throw new ArgumentException("Valid identities and declared role required.");
            var membership=allocations.Where(x=>x!=null&&x.ServiceGroupId==group.BHoM_Guid&&x.ServiceRole==role).ToList();
            if(membership.Count==0||membership.Any(x=>x.SpaceId==Guid.Empty)||membership.GroupBy(x=>x.SpaceId).Any(g=>g.Count()!=1))
                throw new ArgumentException("Nonempty unique space membership required.");
            var all=requirements.ToList();var r=new ServiceGroupAirRequirementResult {ServiceGroupId=group.BHoM_Guid,DesignCaseId=designCaseId,ServiceRole=role,RequiredOutdoorDryAirMassFlowRate=0,SelectedSupplyDryAirMassFlowRate=0};
            foreach(var member in membership) {
                var matches=all.Where(x=>x!=null&&x.SpaceId==member.SpaceId&&x.DesignCaseId==designCaseId).ToList();
                if(matches.Count!=1)throw new ArgumentException("Exactly one compatible requirement required for space "+member.SpaceId);
                var q=matches[0];AirsideMath.Finite(q.RequiredOutdoorDryAirMassFlowRate,"RequiredOutdoorDryAirMassFlowRate");AirsideMath.Finite(q.SelectedSupplyDryAirMassFlowRate,"SelectedSupplyDryAirMassFlowRate");
                if(q.RequiredOutdoorDryAirMassFlowRate<0||q.SelectedSupplyDryAirMassFlowRate<0)throw new ArgumentException("Negative airflow unsupported.");
                if(r.SourceRequirementIds.Contains(q.BHoM_Guid))throw new ArgumentException("Duplicate source requirement identity.");
                r.SpaceIds.Add(q.SpaceId);r.SourceRequirementIds.Add(q.BHoM_Guid);
                r.RequiredOutdoorDryAirMassFlowRate+=q.RequiredOutdoorDryAirMassFlowRate;r.SelectedSupplyDryAirMassFlowRate+=q.SelectedSupplyDryAirMassFlowRate;
            }
            AirsideMath.Finite(r.RequiredOutdoorDryAirMassFlowRate,"aggregate outdoor flow");AirsideMath.Finite(r.SelectedSupplyDryAirMassFlowRate,"aggregate supply flow");
            return r;
        }
    }
}
