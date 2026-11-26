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
    [Description("Internal struct used to generate the edge curves of parametric ShapeProfiles. Describes a ProfileCorner after its rounding has been resolved, " +
                 "as the point where the incoming edge ends (Start), the point where the outgoing edge begins (End), and, for a rounded corner, the arc joining them. " +
                 "For a sharp corner, Start and End coincide with the corner point and Radius is 0. " +
                 "Coordinates are in the local XY-plane of the profile, before the profile is centred. " +
                 "Implemented as an immutable struct to avoid a heap allocation per corner.")]
    internal readonly struct FilletedCorner
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        [Description("X-coordinate of the point where the incoming edge ends, which is the start of the arc for a rounded corner.")]
        public double StartX { get; }

        [Description("Y-coordinate of the point where the incoming edge ends, which is the start of the arc for a rounded corner.")]
        public double StartY { get; }

        [Description("X-coordinate of the point where the outgoing edge begins, which is the end of the arc for a rounded corner.")]
        public double EndX { get; }

        [Description("Y-coordinate of the point where the outgoing edge begins, which is the end of the arc for a rounded corner.")]
        public double EndY { get; }

        [Description("X-coordinate of the centre of the arc. Not used for a sharp corner.")]
        public double CentreX { get; }

        [Description("Y-coordinate of the centre of the arc. Not used for a sharp corner.")]
        public double CentreY { get; }

        [Description("Radius of the arc. 0 for a sharp corner.")]
        public double Radius { get; }

        [Description("Signed angle swept by the arc from Start to End, in radians. Positive for a counter-clockwise arc, i.e. a convex corner on a counter-clockwise loop. 0 for a sharp corner.")]
        public double Sweep { get; }

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        [Description("Creates a sharp corner at the given point.")]
        public FilletedCorner(double x, double y)
        {
            StartX = EndX = x;
            StartY = EndY = y;
            CentreX = CentreY = Radius = Sweep = 0;
        }

        /***************************************************/

        [Description("Creates a corner rounded by an arc from (startX, startY) to (endX, endY) about (centreX, centreY).")]
        public FilletedCorner(double startX, double startY, double endX, double endY, double centreX, double centreY, double radius, double sweep)
        {
            StartX = startX;
            StartY = startY;
            EndX = endX;
            EndY = endY;
            CentreX = centreX;
            CentreY = centreY;
            Radius = radius;
            Sweep = sweep;
        }

        /***************************************************/
    }
}
