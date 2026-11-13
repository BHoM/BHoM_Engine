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
using System.ComponentModel;
using BH.oM.MEP.Airside;
using BH.oM.MEP.Criteria;
namespace BH.Engine.MEP
{
    public static partial class Create
    {
        [Description("Creates a room design-state snapshot using consolidated HVACSpaceCriteria. RH is a state assumption, not automatic control requirement.")]
        public static AirState SpaceDesignAirState(HVACSpaceCriteria criteria, SpaceDesignCondition condition, double pressure)
        {
            if (criteria == null) throw new ArgumentNullException(nameof(criteria));
            if (condition == SpaceDesignCondition.Heating)
                return AirStateFromRelativeHumidity(criteria.HeatingDesignTemperature, criteria.HeatingDesignRelativeHumidity, pressure);
            if (condition == SpaceDesignCondition.Cooling)
                return AirStateFromRelativeHumidity(criteria.CoolingDesignTemperature, criteria.CoolingDesignRelativeHumidity, pressure);
            throw new ArgumentException("Choose heating or cooling.", nameof(condition));
        }
    }
}
