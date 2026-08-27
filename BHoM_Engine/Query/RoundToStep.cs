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

using System.ComponentModel;

using BH.oM.Base;
using BH.oM.Base.Attributes;

namespace BH.Engine.Base
{
    public static partial class Query
    {
        /*******************************************/
        /**** Public Methods                    ****/
        /*******************************************/

        [Description("Rounds a value to the nearest multiple of a step. Unit-agnostic — the caller may feed " +
            "any consistent unit.")]
        [Input("value", "The value to round.")]
        [Input("step", "The increment to round to. A non-positive step leaves the value unchanged. For " +
            "mode == SignificantFigures, this is instead truncated to an integer figures count.")]
        [Input("mode", "Round to the nearest step, always Ceiling/Floor, or to N significant figures.")]
        [Input("preserveSign", "If true (the default), Ceiling always grows the magnitude (away from zero) and " +
            "Floor always shrinks it (towards zero), regardless of whether the value is positive or negative. " +
            "If false, Ceiling/Floor/Round follow the standard mathematical convention instead (toward +∞/−∞/nearest " +
            "on the raw signed value). Not applicable to SignificantFigures.")]
        [Output("rounded", "The rounded value.")]
        public static double RoundToStep(this double value, double step, RoundingMode mode = RoundingMode.Round, bool preserveSign = true)
        {
            if (step <= 0)
                return value;

            switch (mode)
            {
                case RoundingMode.Ceiling:
                    return value.RoundToCeiling(step, preserveSign);
                case RoundingMode.Floor:
                    return value.RoundToFloor(step, preserveSign);
                case RoundingMode.SignificantFigures:
                    return value.RoundToSignificantFigures((int)step);
                default:
                    return value.Round(step, preserveSign);
            }
        }

        /*******************************************/
    }
}
