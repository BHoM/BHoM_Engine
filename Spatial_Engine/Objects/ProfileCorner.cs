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

namespace BH.Engine.Spatial
{
    [Description("Internal struct used to generate the edge curves of parametric ShapeProfiles. Describes one corner of a closed polygon in the local XY-plane of the profile. " +
                 "A profile is defined as one or more closed loops of corners, with outer loops given counter-clockwise and inner loops (voids) clockwise. " +
                 "A corner with a Radius larger than 0 is rounded by an arc tangent to both adjacent edges, otherwise the corner is sharp. " +
                 "Implemented as an immutable struct to avoid a heap allocation per corner.")]
    internal readonly struct ProfileCorner
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        [Description("X-coordinate of the corner point, i.e. the intersection of the two edges meeting at the corner, before any rounding.")]
        public double X { get; }

        [Description("Y-coordinate of the corner point, i.e. the intersection of the two edges meeting at the corner, before any rounding.")]
        public double Y { get; }

        [Description("Radius of the arc rounding the corner. 0 gives a sharp corner. The rounding is skipped if the arc does not fit on the adjacent edges.")]
        public double Radius { get; }

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public ProfileCorner(double x, double y, double radius = 0)
        {
            X = x;
            Y = y;
            Radius = radius;
        }

        /***************************************************/
    }
}
