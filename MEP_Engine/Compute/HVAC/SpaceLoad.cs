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
using System.Linq;
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

        [Description("Calculates internal sensible and latent loads for each space. Explicit criteria take precedence over the assigned space-type defaults. Spaces with missing or invalid inputs are skipped.")]
        [Input("spaces", "Approved calculation spaces with valid floor surfaces.")]
        [Input("criteria", "Optional complete criteria set used for all spaces. If null, each space uses its assigned SpaceType.HVACCriteria. The selected criteria name is stored as ResultCase.")]
        [Output("loads", "Internal load for each calculated space.")]
        public static List<SpaceLoad> SpaceLoad(
            IEnumerable<Space> spaces,
            HVACSpaceCriteria criteria = null)
        {
            List<SpaceLoad> loads = new List<SpaceLoad>();

            if (spaces == null)
            {
                Base.Compute.RecordError("Space load requires spaces.");
                return loads;
            }

            foreach (Space space in spaces)
            {
                if (space == null)
                {
                    Base.Compute.RecordWarning(
                        "Space load skipped a null space.");
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

                double areaPerPerson = selectedCriteria.AreaPerPerson;

                if (double.IsNaN(areaPerPerson) ||
                    double.IsInfinity(areaPerPerson) ||
                    areaPerPerson <= 0)
                {
                    Base.Compute.RecordWarning(
                        $"Space '{label}' was skipped: AreaPerPerson must be supplied, finite and greater than zero.");
                    continue;
                }

                double[] gains =
                {
                    selectedCriteria.SensibleGainPerPerson,
                    selectedCriteria.LatentGainPerPerson,
                    selectedCriteria.LightingGainPerArea,
                    selectedCriteria.EquipmentSensiblePerArea,
                    selectedCriteria.EquipmentLatentPerArea
                };

                if (gains.Any(x =>
                    double.IsNaN(x) ||
                    double.IsInfinity(x) ||
                    x < 0))
                {
                    Base.Compute.RecordWarning(
                        $"Space '{label}' was skipped: all internal-gain rates must be supplied, finite and non-negative. Use an explicit zero where a gain does not apply.");
                    continue;
                }

                double occupantCount = area / areaPerPerson;

                double sensibleLoad =
                    occupantCount * selectedCriteria.SensibleGainPerPerson
                    + area * selectedCriteria.LightingGainPerArea
                    + area * selectedCriteria.EquipmentSensiblePerArea;

                double latentLoad =
                    occupantCount * selectedCriteria.LatentGainPerPerson
                    + area * selectedCriteria.EquipmentLatentPerArea;

                double[] calculatedValues =
                {
                    occupantCount,
                    sensibleLoad,
                    latentLoad
                };

                if (calculatedValues.Any(x =>
                    double.IsNaN(x) ||
                    double.IsInfinity(x)))
                {
                    Base.Compute.RecordWarning(
                        $"Space '{label}' was skipped: the calculated values were not finite.");
                    continue;
                }

                loads.Add(new BH.oM.MEP.Results.SpaceLoad(
                    space.BHoM_Guid,
                    selectedCriteria.Name,
                    occupantCount,
                    sensibleLoad,
                    latentLoad));
            }

            return loads;
        }

        /***************************************************/
    }
}