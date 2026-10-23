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
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Calculates outdoor-air flow for each space using the first specified rate in this order: air changes per hour, per-person rate, then per-area rate. Rates are not added or compared numerically. Explicit criteria take precedence over assigned space-type defaults. Spaces with missing or invalid required inputs are skipped.")]
        [Input("spaces", "Approved calculation spaces with valid floor surfaces. Height is required only for the air-change calculation.")]
        [Input("criteria", "Optional complete criteria set used for all spaces. If null, each space uses its assigned SpaceType.HVACCriteria. NaN means a rate is unspecified; zero is an explicitly specified rate. The selected criteria name is stored as ResultCase.")]
        [Output("flows", "Outdoor-air flow in m3/s for each calculated space.")]
        public static List<OutdoorAirFlow> OutdoorAirFlow(
            IEnumerable<Space> spaces,
            HVACSpaceCriteria criteria = null)
        {
            List<OutdoorAirFlow> flows = new List<OutdoorAirFlow>();

            if (spaces == null)
            {
                Base.Compute.RecordError(
                    "Outdoor-air flow requires spaces.");
                return flows;
            }

            foreach (Space space in spaces)
            {
                if (space == null)
                {
                    Base.Compute.RecordWarning(
                        "Outdoor-air flow skipped a null space.");
                    continue;
                }

                string label = string.IsNullOrWhiteSpace(space.Number)
                    ? space.BHoM_Guid.ToString()
                    : space.Number;

                HVACSpaceCriteria selectedCriteria =
                    criteria ?? space.SpaceType?.HVACCriteria;

                if (selectedCriteria == null)
                {
                    Base.Compute.RecordWarning(
                        $"Space '{label}' was skipped: no explicit criteria or space-type HVAC criteria were supplied.");
                    continue;
                }

                double ach =
                    selectedCriteria.OutdoorAirChangesPerHour;

                double perPerson =
                    selectedCriteria.OutdoorAirPerPerson;

                double perArea =
                    selectedCriteria.OutdoorAirPerArea;

                bool useAch = !double.IsNaN(ach);
                bool usePerPerson = !useAch && !double.IsNaN(perPerson);
                bool usePerArea =
                    !useAch && !usePerPerson && !double.IsNaN(perArea);

                if (!useAch && !usePerPerson && !usePerArea)
                {
                    Base.Compute.RecordWarning(
                        $"Space '{label}' was skipped: no outdoor-air rate was supplied.");
                    continue;
                }

                double selectedRate = useAch
                    ? ach
                    : usePerPerson ? perPerson : perArea;

                string basis = useAch
                    ? "air changes per hour"
                    : usePerPerson ? "per-person rate" : "per-area rate";

                if (double.IsInfinity(selectedRate) || selectedRate < 0)
                {
                    Base.Compute.RecordWarning(
                        $"Space '{label}' was skipped: the selected {basis} must be finite and non-negative. Lower-priority rates were not used.");
                    continue;
                }

                if (space.Surface == null ||
                    space.Surface.ExternalBoundary == null)
                {
                    Base.Compute.RecordWarning(
                        $"Space '{label}' was skipped: a floor surface with an external boundary is required.");
                    continue;
                }

                double area =
                    BH.Engine.Geometry.Query.Area(space.Surface);

                if (double.IsNaN(area) ||
                    double.IsInfinity(area) ||
                    area <= 0)
                {
                    Base.Compute.RecordWarning(
                        $"Space '{label}' was skipped: floor area must be finite and greater than zero.");
                    continue;
                }

                double flow;

                if (useAch)
                {
                    double height = space.Height;

                    if (double.IsNaN(height) ||
                        double.IsInfinity(height) ||
                        height <= 0)
                    {
                        Base.Compute.RecordWarning(
                            $"Space '{label}' was skipped: the selected ACH calculation requires an approved, finite height greater than zero. Lower-priority rates were not used.");
                        continue;
                    }

                    flow = area * height * ach / 3600.0;
                }
                else if (usePerPerson)
                {
                    double areaPerPerson =
                        selectedCriteria.AreaPerPerson;

                    if (double.IsNaN(areaPerPerson) ||
                        double.IsInfinity(areaPerPerson) ||
                        areaPerPerson <= 0)
                    {
                        Base.Compute.RecordWarning(
                            $"Space '{label}' was skipped: the selected per-person calculation requires a finite AreaPerPerson greater than zero. The per-area rate was not used.");
                        continue;
                    }

                    double occupantCount = area / areaPerPerson;

                    if (double.IsNaN(occupantCount) ||
                        double.IsInfinity(occupantCount))
                    {
                        Base.Compute.RecordWarning(
                            $"Space '{label}' was skipped: the calculated occupant count was not finite.");
                        continue;
                    }

                    flow = occupantCount * perPerson;
                }
                else
                {
                    flow = area * perArea;
                }

                if (double.IsNaN(flow) ||
                    double.IsInfinity(flow) ||
                    flow < 0)
                {
                    Base.Compute.RecordWarning(
                        $"Space '{label}' was skipped: the calculated outdoor-air flow was not finite and non-negative.");
                    continue;
                }

                flows.Add(new BH.oM.MEP.Results.OutdoorAirFlow(
                    space.BHoM_Guid,
                    selectedCriteria.Name,
                    flow));
            }

            return flows;
        }

        /***************************************************/
    }
}