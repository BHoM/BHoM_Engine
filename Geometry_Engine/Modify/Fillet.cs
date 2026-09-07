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
using BH.oM.Quantities.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Fillets a Polyline by inserting a circular arc at each internal vertex; the end points of an open Polyline stay sharp.\n" +
                     "Where two corners share a segment too short to carry both fillets, the two are reduced in proportion until they fit, and a warning is raised.\n" +
                     "A corner that cannot be rounded - a joint running straight through or doubling back, or one where there is no room at all - is left sharp rather than dropped, so the result is always a single contiguous curve.")]
        [Input("polyline", "Polyline to fillet.")]
        [Input("radius", "Target fillet radius, which must be greater than zero. A corner achieves less than this only where the adjacent segments are too short to carry the full radius.", typeof(Length))]
        [Input("distTol", "Distance tolerance used for checking point coincidence and segment lengths equal to zero.", typeof(Length))]
        [Input("angleTol", "Angle tolerance. A joint is left sharp where it comes within this of running straight through, or of doubling back on itself.", typeof(Angle))]
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

            // A closed polyline carries a segment from its last vertex back to its first, now that
            // FilletVertices has stripped the repeated closing point; an open one does not.
            int segCount = closed ? vertices.Count : vertices.Count - 1;

            double[] trim = FilletTrimLengths(vertices, segCount, closed, radius, distTol, angleTol);
            Arc[] arcs = FilletArcs(vertices, trim, distTol);

            WarnOnReducedRadius(arcs, radius, distTol);

            return FilletedCurve(vertices, trim, arcs, segCount, closed, distTol);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        // The polyline's vertices with coincident points collapsed, so that every returned vertex is
        // further than distTol from both its neighbours - the closing pair of a closed polyline included.
        // Everything downstream relies on that to keep the assembled curve contiguous.
        // Not Modify.RemoveShortSegments, which re-closes a closed polyline by repeating its first point,
        // whereas here the wrap is carried by segCount instead.
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

            // The loop above only ever compares a point against its predecessor, never against the first
            // vertex, so dropping the repeated closing point of a closed polyline is not enough on its own:
            // any vertex that has collapsed onto the first has to go too, or the closing segment comes back
            // shorter than distTol.
            while (closed && vertices.Count > 1 && vertices[0].Distance(vertices[vertices.Count - 1]) <= distTol)
                vertices.RemoveAt(vertices.Count - 1);

            return vertices;
        }

        /***************************************************/

        // How far back along each adjacent segment a corner is cut before its fillet arc starts, per
        // vertex, and zero where no fillet applies. This owns every trim decision: what the requested radius
        // costs at each corner, the sharing of a segment between the two corners that pull on it, and the
        // final check that a segment keeps some length between them.
        private static double[] FilletTrimLengths(List<Point> vertices, int segCount, bool closed, double radius, double distTol, double angleTol)
        {
            int nVerts = vertices.Count;

            double[] segLengths = new double[segCount];
            for (int s = 0; s < segCount; s++)
            {
                // The modulo wraps the closing segment of a closed polyline back round to the first vertex.
                segLengths[s] = vertices[s].Distance(vertices[(s + 1) % nVerts]);
            }

            // Trim consumed per unit of achieved radius at each corner, and zero where no fillet applies.
            // A sharp corner has a high rate: it pays a lot of segment for very little radius.
            double[] rate = new double[nVerts];

            for (int i = 0; i < nVerts; i++)
            {
                if (!closed && (i == 0 || i == nVerts - 1))
                    continue;

                Point corner = vertices[i];
                Vector v1 = vertices[(i - 1 + nVerts) % nVerts] - corner;
                Vector v2 = vertices[(i + 1) % nVerts] - corner;

                if (v1.Length() < distTol || v2.Length() < distTol)
                    continue;

                double theta = v1.Angle(v2);

                // There is nothing to round on a joint that runs straight through, or that doubles back
                // on itself.
                if (theta < angleTol || Math.Abs(Math.PI - theta) < angleTol)
                    continue;

                double cornerRate = 1.0 / Math.Tan(theta / 2.0);
                if (radius * cornerRate > distTol)
                    rate[i] = cornerRate;
            }

            double[] trim = ShareTrims(rate, segLengths, segCount, nVerts, radius, distTol);

            // A segment short enough to have fallen back to ShortSegmentFallbackFraction in ShareTrims can still
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

        // The trim allowed at each corner once the corners pulling on the same segment have been reconciled.
        // Every corner's radius is raised together from zero. A corner stops growing when it reaches the
        // radius that was asked for, or when a segment it sits on runs out of room; whatever a stopped corner
        // leaves behind is then available to those still growing. That is what keeps a corner held back on
        // one segment from squeezing its neighbour on the other with a trim it is never going to take.
        // Raising the achieved radius rather than the trim length is deliberate: the caller asked for a
        // radius, so corners that can reach it keep it and the rest settle at a common achievable radius.
        // Growing every corner together also makes the result independent of the order the segments happen
        // to be visited in, so a regular polygon fillets symmetrically.
        private static double[] ShareTrims(double[] rate, double[] segLengths, int segCount, int nVerts, double radius, double distTol)
        {
            double margin = distTol * 2.0;

            double[] capacity = new double[segCount];
            for (int s = 0; s < segCount; s++)
            {
                capacity[s] = segLengths[s] - margin;
                if (capacity[s] < distTol)
                    capacity[s] = segLengths[s] * ShortSegmentFallbackFraction;
            }

            double[] trim = new double[nVerts];
            bool[] growing = new bool[nVerts];
            int growingCount = 0;

            for (int i = 0; i < nVerts; i++)
            {
                growing[i] = rate[i] > 0;
                if (growing[i])
                    growingCount++;
            }

            double level = 0.0;

            // Each pass stops at least one corner, so it cannot need more passes than there are corners.
            for (int pass = 0; pass < nVerts && growingCount > 0; pass++)
            {
                // The radius at which the next corner stops: the one that was asked for, or the point at
                // which some segment runs out of room for the corners still growing on it.
                double next = radius;
                for (int s = 0; s < segCount; s++)
                {
                    int vEnd = (s + 1) % nVerts;

                    double growingRate = (growing[s] ? rate[s] : 0) + (growing[vEnd] ? rate[vEnd] : 0);
                    if (growingRate <= 0)
                        continue;

                    double headroom = capacity[s] - (growing[s] ? 0 : trim[s]) - (growing[vEnd] ? 0 : trim[vEnd]);
                    next = Math.Min(next, headroom / growingRate);
                }

                level = Math.Max(level, next);

                for (int i = 0; i < nVerts; i++)
                {
                    if (growing[i])
                        trim[i] = level * rate[i];
                }

                // Everything stops once the requested radius is reached; short of that, only the pair on
                // each segment that has just filled up.
                if (level >= radius)
                    break;

                for (int s = 0; s < segCount; s++)
                {
                    int vEnd = (s + 1) % nVerts;
                    if (trim[s] + trim[vEnd] < capacity[s] - distTol)
                        continue;

                    if (growing[s])
                    {
                        growing[s] = false;
                        growingCount--;
                    }

                    if (growing[vEnd])
                    {
                        growing[vEnd] = false;
                        growingCount--;
                    }
                }
            }

            for (int i = 0; i < nVerts; i++)
            {
                if (trim[i] <= distTol)
                    trim[i] = 0;
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

                arcs[i] = CornerArc(vertices[(i - 1 + nVerts) % nVerts], vertices[i], vertices[(i + 1) % nVerts], trim[i], distTol);
            }

            return arcs;
        }

        /***************************************************/

        // The arc rounding a single corner, tangent to both adjacent segments at the trim distance from
        // the corner, or null where no valid arc exists.
        private static Arc CornerArc(Point prev, Point corner, Point next, double trim, double distTol)
        {
            Vector v1 = (prev - corner).Normalise();
            Vector v2 = (next - corner).Normalise();

            double theta = v1.Angle(v2);

            // The arc sweeps pi - theta. Create.ArcByCentre guards a sweep near pi but not one near zero,
            // and CartesianCoordinateSystem throws rather than returning null on parallel vectors, so
            // without this check a doubling-back joint becomes an unhandled exception. The joint test in
            // FilletTrimLengths only rules that out while the caller leaves angleTol at its default.
            if (Math.PI - theta < Tolerance.Angle)
                return null;

            double arcRadius = trim * Math.Tan(theta / 2.0);
            if (arcRadius <= distTol)
                return null;

            // Non-zero because the sweep check above holds theta away from pi.
            Vector bisector = (v1 + v2).Normalise();
            Point centre = corner + bisector * (arcRadius / Math.Sin(theta / 2.0));

            return BH.Engine.Geometry.Create.ArcByCentre(centre, corner + v1 * trim, corner + v2 * trim, distTol);
        }

        /***************************************************/

        // Warns where the segments available could not carry the radius that was asked for, so that a
        // reduced fillet does not have to be spotted by eye.
        private static void WarnOnReducedRadius(Arc[] arcs, double radius, double distTol)
        {
            int reduced = 0;
            double smallest = radius;

            foreach (Arc arc in arcs)
            {
                if (arc == null || arc.Radius >= radius - distTol)
                    continue;

                reduced++;
                smallest = Math.Min(smallest, arc.Radius);
            }

            if (reduced > 0)
                Base.Compute.RecordWarning("The requested fillet radius of " + radius + " did not fit at " + reduced + " corner(s), where the adjacent segments were too short to carry it. The smallest radius achieved was " + smallest + ".");
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

                output.Add(BH.Engine.Geometry.Create.Line(pA + dir * tStart, pB - dir * tEnd));

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

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        // Fraction of a segment's own length that its two trims may occupy, on a segment too short for
        // the distTol margin to leave any room at all. Deliberately not an input: it has no physical
        // meaning a caller could set it from, and only ever applies to a degenerate segment.
        private const double ShortSegmentFallbackFraction = 0.99;

        /***************************************************/
    }
}
