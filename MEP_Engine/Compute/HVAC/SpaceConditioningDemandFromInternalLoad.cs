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
        [Description("Converts internal gains into cooling demand before supply credit. Internal gains only; not a complete room load. Latent watts are interpreted using the room-temperature moisture-energy convention.")]
        public static SpaceConditioningDemand SpaceConditioningDemandFromInternalLoad(
            Space space, HVACSpaceCriteria criteria, SpaceLoad load, Guid designCaseId)
        {
            if (space == null) throw new ArgumentNullException(nameof(space));
            if (criteria == null) throw new ArgumentNullException(nameof(criteria));
            if (load == null) throw new ArgumentNullException(nameof(load));
            if (space.BHoM_Guid == Guid.Empty) throw new ArgumentException("Canonical space ID is required.", nameof(space));
            if (criteria.BHoM_Guid == Guid.Empty) throw new ArgumentException("Criteria ID is required.", nameof(criteria));
            if (designCaseId == Guid.Empty) throw new ArgumentException("Design-case ID is required.", nameof(designCaseId));
            if (!(load.ObjectId is Guid sourceSpaceId) || sourceSpaceId != space.BHoM_Guid)
                throw new ArgumentException("Source load ObjectId must equal the canonical space Guid.", nameof(load));
            AirsideMath.Finite(load.SensibleLoad, nameof(load.SensibleLoad));
            AirsideMath.Finite(load.LatentLoad, nameof(load.LatentLoad));
            if (load.SensibleLoad < 0 || load.LatentLoad < 0)
                throw new ArgumentOutOfRangeException(nameof(load), "Non-negative internal gains required; losses require a separate net-demand composition method.");
            double t = criteria.CoolingDesignTemperature;
            AirsideMath.Temperature(t);
            double coefficient = 2501000.0 + 1860.0 * t;
            AirsideMath.Finite(coefficient, nameof(coefficient));
            if (coefficient <= 0) throw new ArgumentOutOfRangeException(nameof(criteria));
            double sensible = load.SensibleLoad == 0 ? 0 : -load.SensibleLoad;
            double moisture = load.LatentLoad == 0 ? 0 : -load.LatentLoad / coefficient;
            AirsideMath.Finite(moisture, nameof(moisture));
            return new SpaceConditioningDemand
            {
                SpaceId = space.BHoM_Guid,
                DesignCaseId = designCaseId,
                DesignCondition = SpaceDesignCondition.Cooling,
                SensibleConditioningDemand = sensible,
                MoistureConditioningDemand = moisture,
                SourceId = Guid.Empty,
                DemandBasis = string.Format(CultureInfo.InvariantCulture,
                    "Internal-gains-only SpaceLoad conversion. Space={0:D}; conversion criteria={1:D}; assigned case={2:D}; sensible gain={3:R} W; latent gain={4:R} W; moisture-energy coefficient={5:R} J/kg at {6:R} degC (2501000+1860*T). Excludes outdoor-air treatment, solar, fabric and infiltration. No supply pre-credit, factors or equipment selection. Caller establishes source criteria/case compatibility. SourceId unset: source calculation Guid unavailable.",
                    space.BHoM_Guid, criteria.BHoM_Guid, designCaseId, load.SensibleLoad, load.LatentLoad, coefficient, t)
            };
        }

        [Description("Single-space collection adapter for Compute.SpaceLoad output. Requires exactly one result; never selects silently from incomplete or multi-space output.")]
        public static SpaceConditioningDemand SpaceConditioningDemandFromInternalLoad(
            Space space, HVACSpaceCriteria criteria, IEnumerable<SpaceLoad> loads, Guid designCaseId)
        {
            if (loads == null) throw new ArgumentNullException(nameof(loads));
            var items = loads.ToList();
            if (items.Count != 1) throw new ArgumentException("Exactly one internal load result is required.", nameof(loads));
            return SpaceConditioningDemandFromInternalLoad(space, criteria, items[0], designCaseId);
        }
    }
}
