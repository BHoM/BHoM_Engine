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

        [Description("Fillets a Polyline by inserting an arc of exactly the requested radius at each internal vertex.\n" +
                     "A corner that cannot achieve that radius is left sharp and named in a warning; the end points of an open Polyline stay sharp.")]
        [Input("polyline", "Polyline to fillet.")]
        [Input("radius", "Fillet radius, which must be greater than zero. A corner either achieves it exactly or is left sharp.", typeof(Length))]
        [Input("distTol", "Distance tolerance used for checking point coincidence and segment lengths equal to zero.", typeof(Length))]
        [Input("angleTol", "Angle tolerance for treating a joint as running straight through or doubling back on itself.", typeof(Angle))]
        [Output("polyCurve", "The filleted curve, as trimmed Lines joined by Arc fillets, or null where there is nothing to fillet.")]
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

            List<int> vertexIndices = FilletVertices(pts, closed, distTol);
            if (vertexIndices.Count < 3)
            {
                Base.Compute.RecordError("The Polyline has fewer than three distinct vertices, so it has no corner to round.");
                return null;
            }

            List<Point> vertices = vertexIndices.Select(i => pts[i]).ToList();

            // A closed polyline carries a segment from its last vertex back to its first; an open one does not.
            int segCount = closed ? vertices.Count : vertices.Count - 1;

            List<int> noRoom;
            List<int> degenerate;
            double[] trim = FilletTrimLengths(vertices, vertexIndices, segCount, closed, radius, distTol, angleTol, out noRoom, out degenerate);
            Arc[] arcs = FilletArcs(vertices, vertexIndices, trim, radius, distTol, degenerate);

            WarnOnSkippedCorners(radius, noRoom, degenerate);

            return FilletedCurve(vertices, trim, arcs, segCount, closed, distTol);
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        // The polyline's vertices as control point indices, with coincident points collapsed so that every
        // vertex is further than distTol from its neighbours. Indices, so a warning can name the control point.
        private static List<int> FilletVertices(IList<Point> pts, bool closed, double distTol)
        {
            List<int> vertices = new List<int>();

            for (int i = 0; i < pts.Count; i++)
            {
                // Measured against the last vertex KEPT, or a there-and-back jitter retains two close vertices.
                if (vertices.Count == 0 || pts[i].Distance(pts[vertices[vertices.Count - 1]]) > distTol)
                    vertices.Add(i);
            }

            // The loop above never compares against the first vertex, so a closed polyline can still end on one
            // that has collapsed onto it.
            while (closed && vertices.Count > 1 && pts[vertices[0]].Distance(pts[vertices[vertices.Count - 1]]) <= distTol)
                vertices.RemoveAt(vertices.Count - 1);

            return vertices;
        }

        /***************************************************/

        // How far back each corner is cut, and zero where it is left sharp. Every trim buys the full radius, so
        // this decides which corners are rounded: those left sharp are reported through noRoom and degenerate.
        private static double[] FilletTrimLengths(List<Point> vertices, List<int> vertexIndices, int segCount, bool closed,
            double radius, double distTol, double angleTol, out List<int> noRoom, out List<int> degenerate)
        {
            int nVerts = vertices.Count;

            noRoom = new List<int>();
            degenerate = new List<int>();

            double[] segLengths = new double[segCount];
            for (int s = 0; s < segCount; s++)
            {
                // The modulo wraps the closing segment of a closed polyline back round to the first vertex.
                segLengths[s] = vertices[s].Distance(vertices[(s + 1) % nVerts]);
            }

            // What the radius costs at each corner, and zero where the vertex is no candidate for filleting.
            double[] required = new double[nVerts];

            for (int i = 0; i < nVerts; i++)
            {
                if (!closed && (i == 0 || i == nVerts - 1))
                    continue;

                Point corner = vertices[i];
                Vector v1 = vertices[(i - 1 + nVerts) % nVerts] - corner;
                Vector v2 = vertices[(i + 1) % nVerts] - corner;

                if (v1.Length() < distTol || v2.Length() < distTol)
                {
                    degenerate.Add(vertexIndices[i]);
                    continue;
                }

                double theta = v1.Angle(v2);

                // There is nothing to round on a joint that runs straight through, or that doubles back on itself.
                if (theta < angleTol || Math.Abs(Math.PI - theta) < angleTol)
                {
                    degenerate.Add(vertexIndices[i]);
                    continue;
                }

                double cornerTrim = radius / Math.Tan(theta / 2.0);

                // A trim this small cannot be told from no trim at all.
                if (cornerTrim <= distTol)
                {
                    degenerate.Add(vertexIndices[i]);
                    continue;
                }

                required[i] = cornerTrim;
            }

            // A corner is charged what its neighbours have asked for whether or not those neighbours end up
            // filleted, so that the outcome does not depend on which corner happened to be tested first.
            double[] trim = new double[nVerts];

            for (int i = 0; i < nVerts; i++)
            {
                if (required[i] <= 0)
                    continue;

                // Segment i - 1 arrives at this vertex and segment i leaves it.
                if (FilletSegmentFits(required, segLengths, nVerts, (i - 1 + nVerts) % nVerts, distTol)
                    && FilletSegmentFits(required, segLengths, nVerts, i, distTol))
                    trim[i] = required[i];
                else
                    noRoom.Add(vertexIndices[i]);
            }

            return trim;
        }

        /***************************************************/

        // Whether a segment can carry the trims both of its end corners have asked for and keep a straight
        // length between them; the margin is what stops two fillets meeting in a line of no length.
        private static bool FilletSegmentFits(double[] required, double[] segLengths, int nVerts, int seg, double distTol)
        {
            return required[seg] + required[(seg + 1) % nVerts] <= segLengths[seg] - distTol * 2.0;
        }

        /***************************************************/

        // The fillet arc at each vertex, or null where the vertex is not filleted or its arc could not be built.
        // Resolving every arc before assembly keeps the result contiguous, a null arc being read as no trim.
        private static Arc[] FilletArcs(List<Point> vertices, List<int> vertexIndices, double[] trim, double radius, double distTol, List<int> degenerate)
        {
            int nVerts = vertices.Count;
            Arc[] arcs = new Arc[nVerts];

            for (int i = 0; i < nVerts; i++)
            {
                if (trim[i] <= 0)
                    continue;

                arcs[i] = CornerArc(vertices[(i - 1 + nVerts) % nVerts], vertices[i], vertices[(i + 1) % nVerts], trim[i], radius, distTol);

                // An arc that could not be built leaves the corner sharp, so it belongs with the degenerate ones.
                if (arcs[i] == null)
                    degenerate.Add(vertexIndices[i]);
            }

            return arcs;
        }

        /***************************************************/

        // The arc rounding a single corner at the requested radius, tangent to both adjacent segments at the
        // trim distance from the corner, or null where no valid arc exists.
        private static Arc CornerArc(Point prev, Point corner, Point next, double trim, double radius, double distTol)
        {
            Vector v1 = (prev - corner).Normalise();
            Vector v2 = (next - corner).Normalise();

            double theta = v1.Angle(v2);

            // The arc sweeps pi - theta. Create.ArcByCentre guards a sweep near pi but not one near zero, and
            // CartesianCoordinateSystem throws on parallel vectors, so a doubling-back joint is caught here.
            if (Math.PI - theta < Tolerance.Angle)
                return null;

            // Offset by the radius asked for rather than by one recovered back out of the trim. Non-zero
            // bisector, because the sweep check above holds theta away from pi.
            Vector bisector = (v1 + v2).Normalise();
            Point centre = corner + bisector * (radius / Math.Sin(theta / 2.0));

            return BH.Engine.Geometry.Create.ArcByCentre(centre, corner + v1 * trim, corner + v2 * trim, distTol);
        }

        /***************************************************/

        // Warns for the corners left sharp, by control point number. The two causes are kept apart because too
        // short a segment is answered by a smaller radius and a joint with no corner is not.
        private static void WarnOnSkippedCorners(double radius, List<int> noRoom, List<int> degenerate)
        {
            if (noRoom.Count > 0)
                Base.Compute.RecordWarning("The fillet radius of " + radius + " could not be achieved at control point(s) " + FilletVertexNumbers(noRoom) + ", where an adjacent segment is too short to carry it, so those corners have been left sharp.");

            if (degenerate.Count > 0)
                Base.Compute.RecordWarning("No arc could be inserted at control point(s) " + FilletVertexNumbers(degenerate) + " - the joint runs straight through, doubles back on itself or is otherwise degenerate, so those corners have been left sharp.");
        }

        /***************************************************/

        // Vertices as a readable list, ascending because a vertex can join one of these lists at either the
        // trim or the arc stage, and zero based to match the Polyline's ControlPoints.
        private static string FilletVertexNumbers(List<int> vertices)
        {
            return string.Join(", ", vertices.OrderBy(i => i));
        }

        /***************************************************/

        // The trimmed Lines and corner Arcs as a single PolyCurve, with a closing Line where a closed polyline's
        // last curve does not already meet its first.
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

                // A corner is only cut back where its arc was built, so a line always runs to something that
                // closes it.
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
    }
}
