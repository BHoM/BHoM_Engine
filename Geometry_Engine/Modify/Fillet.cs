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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Fillets a Polyline by inserting a circular arc at each internal vertex; the end points of an open Polyline stay sharp. The trim taken from either side of a corner is capped at half the adjacent segment, so an arc never runs past a segment midpoint. A corner that cannot be rounded - a joint running straight through or doubling back, or one where the requested radius leaves no room - is left sharp rather than dropped, so the result is always a single contiguous curve.")]
        [Input("polyline", "Polyline to fillet.")]
        [Input("radius", "Target fillet radius (> 0). The radius achieved at a corner reduces where the trim-length cap bites.")]
        [Input("distTol", "Distance tolerance.")]
        [Input("angleTol", "Angle tolerance (radians). A joint is left sharp where it comes within this of running straight through, or of doubling back on itself.")]
        [Output("polyCurve", "The filleted curve, as trimmed Lines joined by Arc fillets, or null where there is nothing to fillet - a null Polyline, fewer than two control points, fewer than three distinct vertices, or a radius that is not a positive number.")]
        public static PolyCurve Fillet(this Polyline polyline, double radius,
            double distTol = Tolerance.Distance, double angleTol = Tolerance.Angle)
        {
            if (polyline == null)
            {
                Base.Compute.RecordError("Cannot fillet a null Polyline.");
                return null;
            }

            IList<Point> pts = polyline.ControlPoints;
            if (pts == null || pts.Count < 2)
            {
                Base.Compute.RecordError("The Polyline needs at least two control points to be filleted.");
                return null;
            }

            if (double.IsNaN(radius))
            {
                Base.Compute.RecordError("The fillet radius is not a number, so no corner has been rounded.");
                return null;
            }

            if (radius <= 0)
            {
                Base.Compute.RecordError("The fillet radius is zero or negative, so no corner has been rounded.");
                return null;
            }

            bool closed = polyline.IsClosed(distTol);

            List<Point> vertices = FilletVertices(pts, closed, distTol);
            if (vertices.Count < 3)
            {
                Base.Compute.RecordError("The Polyline has fewer than three distinct vertices, so it has no corner to round.");
                return null;
            }

            // A closed polyline carries a segment from its last vertex back to its first; an open one does not.
            int segCount = closed ? vertices.Count : vertices.Count - 1;

            double[] trim = FilletTrimLengths(vertices, segCount, closed, radius, distTol, angleTol);
            Arc[] arcs = FilletArcs(vertices, trim, distTol);

            return FilletedCurve(vertices, trim, arcs, segCount, closed, distTol);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        // Fraction of a segment's own length that its two trims may occupy, on a segment too short for
        // the distTol margin to leave any room at all.
        private const double ShortSegmentFallbackFraction = 0.99;

        /***************************************************/

        // The polyline's vertices with coincident points collapsed, so that every returned vertex is
        // further than distTol from both its neighbours - the closing pair of a closed polyline included.
        // Everything downstream relies on that: it is what makes every segment long enough to trim and
        // build on, and so what keeps the assembled curve contiguous.
        private static List<Point> FilletVertices(IList<Point> pts, bool closed, double distTol)
        {
            List<Point> vertices = new List<Point>();

            foreach (Point pt in pts)
            {
                // Measured against the last vertex KEPT rather than the last one seen. Against the raw
                // predecessor, a there-and-back jitter passes both tests and leaves two retained vertices
                // closer together than distTol.
                if (vertices.Count == 0 || pt.Distance(vertices[vertices.Count - 1]) > distTol)
                    vertices.Add(pt);
            }

            // A closed polyline repeats its first point last. Drop that repeat, and any further vertex
            // that has collapsed onto the first, so the closing segment is no shorter than the rest.
            while (closed && vertices.Count > 1 && vertices[0].Distance(vertices[vertices.Count - 1]) <= distTol)
                vertices.RemoveAt(vertices.Count - 1);

            return vertices;
        }

        /***************************************************/

        // How far back along each adjacent segment a corner is cut before its fillet arc starts, per
        // vertex, and zero where no fillet applies. This owns every trim decision: the cap from the
        // requested radius, the balancing of the two trims a segment carries, and the final check that
        // a segment keeps some length between them.
        private static double[] FilletTrimLengths(List<Point> vertices, int segCount, bool closed, double radius, double distTol, double angleTol)
        {
            int nVerts = vertices.Count;

            double[] segLengths = new double[segCount];
            for (int s = 0; s < segCount; s++)
                segLengths[s] = vertices[s].Distance(vertices[(s + 1) % nVerts]);

            double[] trim = new double[nVerts];

            for (int i = 0; i < nVerts; i++)
            {
                if (!closed && (i == 0 || i == nVerts - 1))
                    continue;

                Point corner = vertices[i];
                Vector v1 = vertices[(i - 1 + nVerts) % nVerts] - corner;
                Vector v2 = vertices[(i + 1) % nVerts] - corner;

                double len1 = v1.Length();
                double len2 = v2.Length();
                if (len1 < distTol || len2 < distTol)
                    continue;

                v1 /= len1;
                v2 /= len2;

                // Clamped so that rounding on near-parallel vectors cannot push Acos outside its domain.
                double d = Math.Max(-1.0, Math.Min(1.0, v1.DotProduct(v2)));
                double theta = Math.Acos(d);

                // There is nothing to round on a joint that runs straight through, or that doubles back
                // on itself.
                if (theta < angleTol || Math.Abs(Math.PI - theta) < angleTol)
                    continue;

                // The segment leaving vertex i is segment i, and the one arriving is segment i-1. Capping
                // at half of each keeps an arc clear of the corner at the far end of either segment.
                double tDesired = radius / Math.Tan(theta / 2.0);
                double prevHalf = 0.5 * segLengths[(i - 1 + segCount) % segCount];
                double nextHalf = 0.5 * segLengths[i];

                double t = Math.Min(tDesired, Math.Min(prevHalf, nextHalf));
                if (t > distTol)
                    trim[i] = t;
            }

            // Balance the two trims a segment carries against its length. The half-segment cap above
            // already holds their sum to the segment length, so this only bites inside the distTol margin
            // band - but that band is reachable on a near-regular polygon with a large radius.
            double margin = distTol * 2.0;
            for (int s = 0; s < segCount; s++)
            {
                int vEnd = (s + 1) % nVerts;
                double sum = trim[s] + trim[vEnd];

                double maxAllowed = segLengths[s] - margin;
                if (maxAllowed < distTol)
                    maxAllowed = segLengths[s] * ShortSegmentFallbackFraction;

                if (sum <= maxAllowed || (trim[s] <= distTol && trim[vEnd] <= distTol))
                    continue;

                double scale = maxAllowed / sum;
                trim[s] *= scale;
                trim[vEnd] *= scale;

                if (trim[s] <= distTol)
                    trim[s] = 0;
                if (trim[vEnd] <= distTol)
                    trim[vEnd] = 0;
            }

            // A segment short enough to have fallen back to ShortSegmentFallbackFraction above can still
            // be left with no straight length between its two trims. Give up the smaller corner first,
            // and both only if that is not enough - a sharp corner is recoverable, a zero-length or
            // reversed segment is not.
            for (int s = 0; s < segCount; s++)
            {
                int vEnd = (s + 1) % nVerts;
                if (trim[s] + trim[vEnd] < segLengths[s] - distTol)
                    continue;

                if (trim[s] >= trim[vEnd])
                    trim[vEnd] = 0;
                else
                    trim[s] = 0;

                if (trim[s] + trim[vEnd] >= segLengths[s] - distTol)
                {
                    trim[s] = 0;
                    trim[vEnd] = 0;
                }
            }

            return trim;
        }

        /***************************************************/

        // The fillet arc at each vertex, or null where the vertex is not filleted or its arc could not be
        // built. Resolving every arc before any curve is assembled is what keeps the result contiguous:
        // assembly treats a null arc as an untrimmed corner, so a corner can never be cut back and then
        // left open by an arc that failed.
        private static Arc[] FilletArcs(List<Point> vertices, double[] trim, double distTol)
        {
            int nVerts = vertices.Count;
            Arc[] arcs = new Arc[nVerts];

            for (int i = 0; i < nVerts; i++)
            {
                if (trim[i] <= 0)
                    continue;

                arcs[i] = FilletArc(vertices[(i - 1 + nVerts) % nVerts], vertices[i], vertices[(i + 1) % nVerts], trim[i], distTol);
            }

            return arcs;
        }

        /***************************************************/

        // The arc rounding a single corner, tangent to both adjacent segments at the trim distance from
        // the corner, or null where no valid arc exists.
        private static Arc FilletArc(Point prev, Point corner, Point next, double trim, double distTol)
        {
            Vector v1 = (prev - corner).Normalise();
            Vector v2 = (next - corner).Normalise();

            // Clamped so that rounding on near-parallel vectors cannot push Acos outside its domain.
            double d = Math.Max(-1.0, Math.Min(1.0, v1.DotProduct(v2)));
            double theta = Math.Acos(d);

            // The arc sweeps pi - theta. Create.ArcByCentre throws out of CartesianCoordinateSystem
            // rather than returning null when asked for a sweep below Tolerance.Angle, and the joint test
            // in FilletTrimLengths only rules that out while the caller leaves angleTol at its default.
            if (Math.PI - theta < Tolerance.Angle)
                return null;

            double R = trim * Math.Tan(theta / 2.0);
            if (R <= distTol)
                return null;

            // Non-zero because the sweep check above holds theta away from pi.
            Vector bis = (v1 + v2).Normalise();
            Point centre = corner + bis * (R / Math.Sin(theta / 2.0));

            return BH.Engine.Geometry.Create.ArcByCentre(centre, corner + v1 * trim, corner + v2 * trim, distTol);
        }

        /***************************************************/

        // The trimmed Lines and corner Arcs as a single PolyCurve, with a closing Line where a closed
        // polyline's last curve does not already meet its first. Reads the trims and arcs it is given
        // and changes neither.
        private static PolyCurve FilletedCurve(List<Point> vertices, double[] trim, Arc[] arcs, int segCount, bool closed, double distTol)
        {
            int nVerts = vertices.Count;
            List<ICurve> output = new List<ICurve>();

            for (int s = 0; s < segCount; s++)
            {
                int vEnd = (s + 1) % nVerts;

                Point pA = vertices[s];
                Point pB = vertices[vEnd];
                Vector dir = (pB - pA).Normalise();

                // A corner is only cut back where its arc was actually built, so that the line always
                // runs to something that closes it.
                double tStart = arcs[s] == null ? 0 : trim[s];
                double tEnd = arcs[vEnd] == null ? 0 : trim[vEnd];

                output.Add(new Line { Start = pA + dir * tStart, End = pB - dir * tEnd });

                if (arcs[vEnd] != null)
                    output.Add(arcs[vEnd]);
            }

            if (closed)
            {
                Point startPoint = output.First().IStartPoint();
                Point endPoint = output.Last().IEndPoint();
                if (startPoint.Distance(endPoint) > distTol)
                    output.Add(BH.Engine.Geometry.Create.Line(endPoint, startPoint));
            }

            return new PolyCurve { Curves = output };
        }

    }
}
