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
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using BH.oM.MEP.Criteria;
using BH.oM.MEP.Elements;
using BH.oM.MEP.Results;
namespace BH.Engine.MEP
{
    public static partial class Compute
    {
        [Description("Outdoor-air flow using ACH, then per-person, then per-area. ACH zero or NaN means unset. Other rate zeros remain explicit. Invalid ACH rejects the space rather than falling back.")]
        [Input("spaces", "Approved spaces with valid floor surfaces; height required for positive ACH.")]
        [Input("criteria", "Optional complete criteria override, otherwise SpaceType.HVACCriteria. ResultCase records criteria name.")]
        [Output("flows", "Outdoor-air flow in m3/s; no thermodynamic volume reference inferred.")]
        public static List<OutdoorAirFlow> OutdoorAirFlow(IEnumerable<Space> spaces, HVACSpaceCriteria criteria = null)
        {
            var result = new List<OutdoorAirFlow>();
            if(spaces==null){Base.Compute.RecordError("Outdoor-air flow requires spaces.");return result;}
            foreach(var space in spaces) {
                if(space==null){Base.Compute.RecordWarning("Outdoor-air flow skipped null space.");continue;}
                string label=string.IsNullOrWhiteSpace(space.Number)?space.BHoM_Guid.ToString():space.Number;
                var c=criteria??space.SpaceType?.HVACCriteria;
                string issue=null; double flow=double.NaN;
                if(c==null)issue="criteria missing";
                else if(!double.IsNaN(c.OutdoorAirChangesPerHour) && (double.IsInfinity(c.OutdoorAirChangesPerHour)||c.OutdoorAirChangesPerHour<0))
                    issue="ACH invalid; no lower-priority fallback";
                else {
                    bool ach=!double.IsNaN(c.OutdoorAirChangesPerHour)&&c.OutdoorAirChangesPerHour>0;
                    bool person=!ach&&!double.IsNaN(c.OutdoorAirPerPerson);
                    bool areaRate=!ach&&!person&&!double.IsNaN(c.OutdoorAirPerArea);
                    double rate=ach?c.OutdoorAirChangesPerHour:person?c.OutdoorAirPerPerson:c.OutdoorAirPerArea;
                    if(!ach&&!person&&!areaRate)issue="no outdoor-air rate supplied (ACH zero is unset)";
                    else if(double.IsInfinity(rate)||rate<0)issue="selected rate must be finite and nonnegative; no lower-priority fallback";
                    else if(space.Surface==null||space.Surface.ExternalBoundary==null)issue="floor surface missing";
                    else {
                        double area=BH.Engine.Geometry.Query.Area(space.Surface);
                        if(double.IsNaN(area)||double.IsInfinity(area)||area<=0)issue="floor area must be finite and positive";
                        else if(ach) {
                            if(double.IsNaN(space.Height)||double.IsInfinity(space.Height)||space.Height<=0)issue="positive ACH requires approved positive height";
                            else flow=area*space.Height*rate/3600;
                        } else if(person) {
                            if(double.IsNaN(c.AreaPerPerson)||double.IsInfinity(c.AreaPerPerson)||c.AreaPerPerson<=0)issue="per-person rate requires positive AreaPerPerson";
                            else flow=area/c.AreaPerPerson*rate;
                        } else flow=area*rate;
                    }
                }
                if(issue==null&&(double.IsNaN(flow)||double.IsInfinity(flow)||flow<0))issue="computed flow invalid";
                if(issue!=null){Base.Compute.RecordWarning("Space '"+label+"' skipped: "+issue+".");continue;}
                result.Add(new OutdoorAirFlow(space.BHoM_Guid,c.Name,flow));
            }
            return result;
        }
    }
}
