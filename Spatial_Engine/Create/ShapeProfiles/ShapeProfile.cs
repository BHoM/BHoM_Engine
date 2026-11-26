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

using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace BH.Engine.Spatial
{
    public static partial class Create
    {

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        [Description("Records an error for an input that is invalid in relation to one or more other inputs, given as their names.")]
        private static void InvalidRatioError(string first, string second)
        {
            Engine.Base.Compute.RecordError("The ratio of the " + first + " in relation to the " + second + " makes section inconceivable");
        }

        /***************************************************/

        [Description("Returns false and records an error if any of the values is NaN or infinite. Such values pass all the comparison based input checks, as any comparison with NaN is false, and would otherwise give profiles with invalid or no edges.")]
        private static bool AreFinite(params double[] values)
        {
            foreach (double value in values)
            {
                if (double.IsNaN(value) || double.IsInfinity(value))
                {
                    Engine.Base.Compute.RecordError("All dimensions of the profile must be finite numbers, i.e. not NaN or infinity.");
                    return false;
                }
            }

            return true;
        }

        /***************************************************/

        [Description("Returns false and records an error if any of the values is 0 or negative. Used for dimensions the profile cannot exist without, such as heights, widths and thicknesses.")]
        private static bool ArePositive(params double[] values)
        {
            foreach (double value in values)
            {
                if (value <= 0)
                {
                    Engine.Base.Compute.RecordError("Input length less or equal to 0");
                    return false;
                }
            }

            return true;
        }

        /***************************************************/

        [Description("Returns false and records an error if any of the values is negative. Used for optional dimensions where 0 means the feature is not there, such as radii and weld sizes.")]
        private static bool AreNonNegative(params double[] values)
        {
            foreach (double value in values)
            {
                if (value < 0)
                {
                    Engine.Base.Compute.RecordError("Input length less than 0");
                    return false;
                }
            }

            return true;
        }

        /***************************************************/
        /**** Private Methods - Profile curve generation ****/
        /***************************************************/

        // All parametric profiles are described as one or more closed loops of ProfileCorners in the XY-plane, where a corner with a Radius > 0
        // is rounded with an arc tangent to both adjacent edges. Outer loops are given counter-clockwise and inner loops (voids) clockwise.
        // Each profile only lists the corners that exist for its inputs, so no two consecutive corners coincide and no corner lies in the middle
        // of a straight edge.
        // This gives the following for all profiles:
        //  - Each loop is returned as a continuous chain where the end point of each curve is the start point of the next.
        //  - Consistent orientation, meaning signed region integrals of the loops give net (solid minus void) properties.
        //  - The centre of area is computed exactly in a single pass over the loops, without joining or validating any curves.
        //
        // The corners are first resolved into FilletedCorners, holding the point where the incoming edge ends (Start), the point where the
        // outgoing edge begins (End) and the arc joining them. Each loop is then the sequence: line to Start, arc to End, line to next Start, ...

        /***************************************************/

        [Description("Converts the closed loops of corners to curves. If centre is true, the curves are placed so that the combined centre of area of the loops is at the origin.")]
        private static List<ICurve> ProfileCurves(bool centre, params List<ProfileCorner>[] loops)
        {
            List<FilletedCorner[]> filletedLoops = new List<FilletedCorner[]>(loops.Length);
            int curveCount = 0;
            foreach (List<ProfileCorner> loop in loops)
            {
                FilletedCorner[] filleted = FilletCorners(loop);
                filletedLoops.Add(filleted);
                curveCount += 2 * filleted.Length;
            }

            double dx = 0, dy = 0;
            if (centre)
            {
                double area = 0, momentX = 0, momentY = 0;
                foreach (FilletedCorner[] loop in filletedLoops)
                    AddAreaMoments(loop, ref area, ref momentX, ref momentY);

                if (area != 0)
                {
                    dx = momentX / area;
                    dy = momentY / area;
                }
            }

            List<ICurve> curves = new List<ICurve>(curveCount);
            foreach (FilletedCorner[] loop in filletedLoops)
                AddLoopCurves(loop, dx, dy, curves);

            return curves;
        }

        /***************************************************/

        [Description("Mirrors the corners about the local axes of the profile. The order of the corners is reversed if the mirroring changes the orientation of the loop, so that outer loops stay counter-clockwise and inner loops clockwise.")]
        private static List<ProfileCorner> MirrorCorners(this List<ProfileCorner> corners, bool mirrorAboutLocalZ, bool mirrorAboutLocalY)
        {
            if (!mirrorAboutLocalZ && !mirrorAboutLocalY)
                return corners;

            double sx = mirrorAboutLocalZ ? -1 : 1;
            double sy = mirrorAboutLocalY ? -1 : 1;
            List<ProfileCorner> mirrored = new List<ProfileCorner>(corners.Count);
            foreach (ProfileCorner c in corners)
                mirrored.Add(new ProfileCorner(sx * c.X, sy * c.Y, c.Radius));

            if (mirrorAboutLocalZ != mirrorAboutLocalY)
                mirrored.Reverse();

            return mirrored;
        }

        /***************************************************/

        [Description("Corners of a rectangle centred at the origin, all rounded by the same radius.")]
        private static List<ProfileCorner> RectangleCorners(double width, double height, double radius, bool clockwise = false)
        {
            double x = width / 2;
            double y = height / 2;
            if (clockwise)
                return new List<ProfileCorner> { new ProfileCorner(-x, -y, radius), new ProfileCorner(-x, y, radius), new ProfileCorner(x, y, radius), new ProfileCorner(x, -y, radius) };
            else
                return new List<ProfileCorner> { new ProfileCorner(-x, -y, radius), new ProfileCorner(x, -y, radius), new ProfileCorner(x, y, radius), new ProfileCorner(-x, y, radius) };
        }

        /***************************************************/

        [Description("Resolves the rounding of each corner of the closed loop. A corner is left sharp if its arc does not fit on both adjacent edges.")]
        private static FilletedCorner[] FilletCorners(List<ProfileCorner> pts)
        {
            int n = pts.Count;
            FilletedCorner[] result = new FilletedCorner[n];

            for (int i = 0; i < n; i++)
            {
                ProfileCorner c = pts[i];

                if (c.Radius > 0)
                {
                    ProfileCorner prev = pts[(i + n - 1) % n];
                    ProfileCorner next = pts[(i + 1) % n];

                    //Unit directions of the incoming (a) and outgoing (b) edges
                    double ax = c.X - prev.X, ay = c.Y - prev.Y;
                    double bx = next.X - c.X, by = next.Y - c.Y;
                    double la = Math.Sqrt(ax * ax + ay * ay);
                    double lb = Math.Sqrt(bx * bx + by * by);
                    ax /= la; ay /= la;
                    bx /= lb; by /= lb;

                    double cross = ax * by - ay * bx;
                    double dot = ax * bx + ay * by;

                    //Signed turning angle at the corner, and the distance from the corner point to the tangent points of the arc
                    double sweep, trim;
                    if (Math.Abs(dot) < 1e-12)
                    {
                        //Right angle corner - no trigonometry needed
                        sweep = cross > 0 ? Math.PI / 2 : -Math.PI / 2;
                        trim = c.Radius;
                    }
                    else
                    {
                        sweep = Math.Atan2(cross, dot);
                        trim = c.Radius * Math.Tan(Math.Abs(sweep) / 2);
                    }

                    //Only round the corner if the fillet fits on both adjacent edges
                    if (trim <= la + Tolerance.Distance && trim <= lb + Tolerance.Distance)
                    {
                        //Centre is offset from the start point perpendicular to the incoming edge, towards the inside of the turn
                        double side = cross > 0 ? 1 : -1;
                        double x0 = c.X - ax * trim;
                        double y0 = c.Y - ay * trim;
                        result[i] = new FilletedCorner(x0, y0, c.X + bx * trim, c.Y + by * trim, x0 - side * ay * c.Radius, y0 + side * ax * c.Radius, c.Radius, sweep);
                        continue;
                    }
                }

                result[i] = new FilletedCorner(c.X, c.Y);
            }

            return result;
        }

        /***************************************************/

        [Description("Adds the signed area and first moments of area of the region enclosed by the loop, using Green's theorem. Counter-clockwise loops give positive area.")]
        private static void AddAreaMoments(FilletedCorner[] loop, ref double area, ref double momentX, ref double momentY)
        {
            FilletedCorner prev = loop[loop.Length - 1];
            foreach (FilletedCorner f in loop)
            {
                //Straight edge from the previous corner
                AddChordAreaMoments(prev.EndX, prev.EndY, f.StartX, f.StartY, ref area, ref momentX, ref momentY);

                if (f.Radius > 0)
                {
                    //Chord of the arc
                    AddChordAreaMoments(f.StartX, f.StartY, f.EndX, f.EndY, ref area, ref momentX, ref momentY);

                    //Circular segment between the chord and the arc, with its centroid on the bisector of the arc
                    double segmentArea = f.Radius * f.Radius / 2 * (f.Sweep - Math.Sin(f.Sweep));
                    double h = Math.Abs(f.Sweep) / 2;
                    double sinH = Math.Sin(h);
                    double m = Math.Sign(f.Sweep) * f.Radius * f.Radius * sinH * sinH * sinH / (3 * Math.Cos(h));
                    area += segmentArea;
                    momentX += segmentArea * f.CentreX + m * (f.StartX + f.EndX - 2 * f.CentreX);
                    momentY += segmentArea * f.CentreY + m * (f.StartY + f.EndY - 2 * f.CentreY);
                }

                prev = f;
            }
        }

        /***************************************************/

        [Description("Adds the signed area and first moments of area of the triangle between the origin and the straight segment (x0, y0) -> (x1, y1).")]
        private static void AddChordAreaMoments(double x0, double y0, double x1, double y1, ref double area, ref double momentX, ref double momentY)
        {
            double k = x0 * y1 - x1 * y0;
            area += k / 2;
            momentX += (x0 + x1) * k / 6;
            momentY += (y0 + y1) * k / 6;
        }

        /***************************************************/

        [Description("Adds the curves of the loop to the list, translated by (-dx, -dy). Zero length lines are skipped, and consecutive curves share end and start points.")]
        private static void AddLoopCurves(FilletedCorner[] loop, double dx, double dy, List<ICurve> curves)
        {
            int n = loop.Length;
            Point first = new Point { X = loop[n - 1].EndX - dx, Y = loop[n - 1].EndY - dy };
            Point current = first;
            double sqTol = Tolerance.Distance * Tolerance.Distance;

            for (int i = 0; i < n; i++)
            {
                FilletedCorner f = loop[i];
                bool last = i == n - 1;

                //Straight edge from the end of the previous corner to the start of this one
                double ex = f.StartX - dx, ey = f.StartY - dy;
                double lx = ex - current.X, ly = ey - current.Y;
                if (lx * lx + ly * ly > sqTol)
                {
                    Point end = last && f.Radius == 0 ? first : new Point { X = ex, Y = ey };
                    curves.Add(new Line { Start = current, End = end });
                    current = end;
                }

                if (f.Radius > 0)
                {
                    //Arc with its local x-axis towards the start point, and its local z-axis set by the direction of the sweep
                    Point centre = new Point { X = f.CentreX - dx, Y = f.CentreY - dy };
                    double ux = (current.X - centre.X) / f.Radius;
                    double uy = (current.Y - centre.Y) / f.Radius;
                    double z = f.Sweep > 0 ? 1 : -1;
                    curves.Add(new Arc
                    {
                        CoordinateSystem = new Cartesian(centre, new Vector { X = ux, Y = uy }, new Vector { X = -z * uy, Y = z * ux }, new Vector { Z = z }),
                        Radius = f.Radius,
                        StartAngle = 0,
                        EndAngle = Math.Abs(f.Sweep),
                    });
                    current = last ? first : new Point { X = f.EndX - dx, Y = f.EndY - dy };
                }
            }
        }

        /***************************************************/

    }
}
