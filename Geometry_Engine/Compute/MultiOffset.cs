/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

using BH.Engine.Base;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Quantities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Generate multiple offset curves corresponding to the provided values. Method can return more than a single curve for a particular distance for self-intersection curves.")]
        [Input("curve", "Curve to offset.")]
        [Input("offsets", "List of offset distances that the curve will be offset by. Please note that the order of the input offset distances might differ from the returned curves!", typeof(Length))]
        [Input("normal", "Optional Normal of the curve. Method atempts to automatically compute a normal if nothing is provided.")]
        [Input("options", "Various options controling the behaviour of the method in terms of hanlding of intersections and short segments.")]
        [Input("onlyLargestPerStep", "If true, only the largest curve per step is returned for self-intersection curves. If false, all curves are returned.")]
        [Input("distTol", "Distance tolerance used for checking segment lengths equal to zero, point coincidence etc.")]
        [Input("angleTol", "Angle tolerance used for checking if segments are parallel.")]
        [Output("offsetCurves", "List of offset curves. Please note that the order might not correspond to the order of the input offset values.")]
        public static List<Polyline> MultiOffset(this Polyline curve, List<double> offsets, Vector normal = null, OffsetOptions options = null, bool onlyLargestPerStep = false, double distTol = Tolerance.Distance, double angleTol = Tolerance.Angle)
        {
            if (curve == null || curve.Length() < distTol)
                return null;

            if (offsets == null || offsets.Count == 0)
                return new List<Polyline>();

            if (offsets.Count == 1 && offsets[0] == 0)
                return new List<Polyline> { curve };

            if (!curve.IsPlanar(distTol))
            {
                BH.Engine.Base.Compute.RecordError("Offset works only on planar curves");
                return null;
            }

            options = options ?? new OffsetOptions();   //Initialise to default values if nothing is provided

            if (options.HandleGeneralCreatedSelfIntersections)
            {
                if (!options.HandleCreatedLocalSelfIntersections)
                {
                    options = options.ShallowClone();
                    options.HandleCreatedLocalSelfIntersections = true;
                    Base.Compute.RecordNote($"{nameof(options.HandleCreatedLocalSelfIntersections)} has been set to true due to {nameof(options.HandleGeneralCreatedSelfIntersections)} being true.");
                }
            }

            bool isClosed = curve.IsClosed(distTol);

            List<Point> cPts = new List<Point>(curve.ControlPoints);

            if (normal == null)
                if (!isClosed)
                {
                    int i = 2;
                    Vector v1 = cPts[1] - cPts[0];
                    do
                    {
                        normal = v1.CrossProduct(cPts[i] - cPts[0]);
                        i++;
                        if (i >= cPts.Count)
                        {
                            Base.Compute.RecordError("Normal vector is required to be provided for a linear curve.");
                            return null;
                        }
                    } while (normal.Length() < distTol);
                }
                else
                    normal = curve.Normal();
            else
                normal = normal.Normalise();

            //Construct list of segment data for each line piece. Segment data contains
            // - Length of the segments
            // - Tangent vector of the segment
            // - Orthogonal vector of the segment
            // - Length change of the segment for a offset with a magnitude equal to 1
            List<OffsetSegment> segments = new List<OffsetSegment>();
            for (int i = 0; i < cPts.Count - 1; i++)
            {
                Vector tan = (cPts[i + 1] - cPts[i]);
                double length = tan.Length();
                if (options.RemoveShortSegments && length < distTol) //Check for short segments and remove
                {
                    cPts.RemoveAt(i + 1);
                    i--;
                }
                else
                {
                    if (options.RemoveShortSegments || length > 0)
                        tan = tan * (1 / length);

                    //Set length, tangent and orthogonal vector. Length change for the segment is computed in a later step
                    segments.Add(new OffsetSegment() { Length = length, Tangent = tan, Orthogonal = tan.CrossProduct(normal).Normalise(), ComputeLengthChange = options.HandleCreatedLocalSelfIntersections });
                }
            }

            if (isClosed)
                cPts.RemoveAt(cPts.Count - 1);

            //Construct list of vertex data for each control point where the vertex data contains information about
            // - Position of the vertex
            // - Offset direction for the vertex as a vector with a vector scaled to an offset of magnitude equal to 1
            // - Length change for adjecent segments for an offset with a magnitude equal to 1
            // - Maxmimum allowed offset until the vertex hits a segment
            // - Segment hit after maximum offset
            List<OffsetVertex> vertices = cPts.Select(x => new OffsetVertex() { Position = x, ComputeTranslation = true, ComputeIntersection = options.HandleGeneralCreatedSelfIntersections }).ToList();

            List<Polyline> pLines = new List<Polyline>();
            if (offsets.Any(x => x == 0))
                pLines.Add(curve);

            List<double> positive = offsets.Where(x => x > 0).OrderBy(x => x).ToList();
            List<double> negative = offsets.Where(x => x < 0).Select(x => -x).OrderBy(x => x).ToList();

            //Set lists to be additional offset after current step
            for (int i = positive.Count - 1; i > 0; i--)
            {
                positive[i] = positive[i] - positive[i - 1];
            }

            for (int i = negative.Count - 1; i > 0; i--)
            {
                negative[i] = negative[i] - negative[i - 1];
            }

            List<OffsetVertex> negV = vertices.Select(v => v.Clone()).ToList();
            List<OffsetSegment> negSeg = segments.Select(s => s.Clone(true)).ToList();


            List<List<OffsetVertex>> offsetVertices = Iterate(segments, vertices, positive, normal, isClosed, options, onlyLargestPerStep, distTol, angleTol, true);
            offsetVertices.AddRange(Iterate(negSeg, negV, negative, -normal, isClosed, options, onlyLargestPerStep, distTol, angleTol, true));
            foreach (var offsetVert in offsetVertices)
            {
                pLines.Add(new Polyline { ControlPoints = offsetVert.Select(x => x.Position).ToList() });
            }

            return pLines;
        }

        /***************************************************/
        /***  Private Methods                            ***/
        /***************************************************/

        private static List<List<OffsetVertex>> Iterate(List<OffsetSegment> segments, List<OffsetVertex> vertices, List<double> offsets, Vector normal, bool isClosed, OffsetOptions options, bool onlyLargestPerStep, double distTol, double angleTol, bool firstIteration)
        {
            if (offsets.Count == 0)
                return new List<List<OffsetVertex>>();
            //Tolerance used for checking if two adjecent segments are anti-parallel and hence creating a acute corner
            double sinTol = Math.Pow(Math.Sin(angleTol / 2), 2);

            List<List<OffsetVertex>> finalVertices = new List<List<OffsetVertex>>();
            double totalOffset = offsets.Sum();
            while (offsets.Count > 0)
            {
                double offset = offsets[0];
                offsets.RemoveAt(0);

                //Loop and continue offseting until all offsets done.
                //Done to handle local apparent self intersections due to segments getting a length < 0 (being flipped) for concave offsets
                int bailout = 10000;
                int counter = 0;
                while (offset > 0 && counter < bailout)
                {
                    counter++;
                    //Compute corner offset vectors. First whileloop iteration computes for all corners
                    //Subsequent iteration only updates corners that have seen a change
                    bool recheckTranlsations = false;
                    int recomputeCount = 0;
                    do
                    {
                        recheckTranlsations = false;
                        recomputeCount++;
                        for (int i = 0; i < vertices.Count; i++)
                        {
                            OffsetVertex v = vertices[i];
                            if (!v.ComputeTranslation)
                                continue;

                            v.ComputeTranslation = false;
                            if (!isClosed && i == 0)    //Start point vector for open curves will simply be the orthogonal vector of the first segment
                            {
                                v.Translation = segments[i].Orthogonal;
                                segments[i].ComputeLengthChange = options.HandleCreatedLocalSelfIntersections;
                            }
                            else if (!isClosed && i == vertices.Count - 1)  //Similar to start
                            {
                                v.Translation = segments[i - 1].Orthogonal;
                                segments[i - 1].ComputeLengthChange = options.HandleCreatedLocalSelfIntersections;
                            }
                            else
                            {
                                int prev = i == 0 ? segments.Count - 1 : i - 1;

                                var seg1 = segments[prev];
                                var seg2 = segments[i];

                                Vector dir = (seg1.Orthogonal + seg2.Orthogonal) / 2; //Offset direction computed as sum of the previous and next orthogonal vectors

                                //Below factor gives the correct magnitude for corner scaling based on equal angle triangles.
                                //ortho projected onto the direction gives a right angled triangle with the same angles as the triangle
                                //formed by offset vetor with correct length and dir.
                                double length = dir.SquareLength();

                                if (options.HandleAdjacentParallelSegments && length < sinTol)    //Equivalent to check that angle between two adjecent segements is less than angle tolerance
                                {
                                    if (!HandleParalellAdjecentPolylinearSegments(segments, vertices, normal, isClosed, firstIteration, distTol, i, prev, angleTol))
                                    {
                                        return finalVertices;
                                    }
                                    recheckTranlsations = true; //Set flag to re-iteratate to check translation vectors again
                                }
                                else
                                {
                                    Vector trans = dir * (1 / length);
                                    double vertLengthChange = seg1.Tangent.DotProduct(trans);
                                    v.Translation = trans;
                                    v.AdjacentSegmentLengthChange = vertLengthChange;
                                    seg1.ComputeLengthChange = options.HandleCreatedLocalSelfIntersections; //Set adjecent segments to be recomputed in terms of length change
                                    seg2.ComputeLengthChange = options.HandleCreatedLocalSelfIntersections;
                                    v.ComputeIntersection = options.HandleGeneralCreatedSelfIntersections;
                                }
                            }

                        }
                    } while (recheckTranlsations);


                    //Find maximum offset possible
                    double maxOffset = offset;

                    if (options.HandleCreatedLocalSelfIntersections || options.HandleGeneralCreatedSelfIntersections)
                    {
                        //Compute how much the length of each segment is affected by a unit offset (length 1)
                        //First whileloop iteration runs though all segemnts, later loop iterations only recomputes for segments that have seen a change
                        for (int i = 0; i < segments.Count; i++)
                        {
                            if (!segments[i].ComputeLengthChange)
                                continue;

                            double change;  //Change length change given a unit offset (length 1)
                            if (!isClosed && i == 0)
                                change = vertices[i + 1].AdjacentSegmentLengthChange; //For open curves, the first vertex has no impact
                            else if (!isClosed && i == segments.Count - 1)
                                change = vertices[i].AdjacentSegmentLengthChange;     //For open curves the last vertex has no impact
                            else
                            {
                                change = vertices[i].AdjacentSegmentLengthChange + vertices[(i + 1) % vertices.Count].AdjacentSegmentLengthChange;  //For closed curves, as well as non-start points, both vertices impact
                            }
                            segments[i].SegmentLengthChange = change;   //Set length change 
                            segments[i].ComputeLengthChange = false;    //Set computation flag
                        }

                        //Compute the maximum offset allowed for this main iteration.
                        //The maximum offset is set to be the smaller of the remaining offset value and the minimum offset value required to make any segment get a length of 0
                        for (int i = 0; i < segments.Count; i++)
                        {
                            double offsetScale = segments[i].Length / segments[i].SegmentLengthChange; //offset required for element length to become 0 length
                            if (offsetScale < 0)    //Positive value means segments are getting longer, hence no limit
                                maxOffset = Math.Min(maxOffset, -offsetScale);  //Set max offset to maxmimum of offset value and 
                        }
                    }

                    if (options.HandleGeneralCreatedSelfIntersections)
                        maxOffset = Math.Min(maxOffset, ComputeMaxOffsetUntilIntersection(segments, vertices, totalOffset, isClosed, distTol));

                    bool firstIterForceOffset = false;
                    if (firstIteration && maxOffset == 0 && recomputeCount > 1)
                    {
                        firstIterForceOffset = true;
                        maxOffset = distTol * 1.5;
                    }
                    //Offset the points
                    for (int i = 0; i < vertices.Count; i++)
                    {
                        var v = vertices[i];
                        v.Position = v.Position + v.Translation * maxOffset;
                    }

                    //Reduce the remaining offset by amount used up in this iteration
                    offset -= maxOffset;

                    //If remaining offset, make adjustments (remove 0 length segments etc) for next iteration
                    if (firstIterForceOffset)
                    {
                        if (options.HandleCreatedLocalSelfIntersections)
                        {
                            foreach (var segment in segments)
                            {
                                segment.Length += segment.SegmentLengthChange * maxOffset;
                            }
                        }
                        if (options.HandleGeneralCreatedSelfIntersections)
                        {
                            foreach (OffsetVertex vertex in vertices)
                            {
                                if (vertex.AnySegmentInRangeForIntersection)
                                    vertex.ComputeIntersection = true;
                            }
                        }
                    }
                    else if (offset > 0)
                    {
                        if (!isClosed && vertices[0].Position.SquareDistance(vertices[vertices.Count - 1].Position) < distTol * distTol)
                        {
                            isClosed = true;
                            vertices.RemoveAt(vertices.Count - 1);
                            vertices[0].ComputeTranslation = true;
                            vertices[0].ComputeIntersection = true;

                            if (options.HandleCreatedLocalSelfIntersections)
                            {
                                foreach (var segment in segments)
                                {
                                    segment.Length += segment.SegmentLengthChange * maxOffset;
                                }
                            }
                            if (options.HandleGeneralCreatedSelfIntersections)
                            {
                                foreach (OffsetVertex vertex in vertices)
                                {
                                    if (vertex.SegmentIntersected != -1)
                                        vertex.OffsetUntilIntersection -= maxOffset;
                                }
                            }
                        }
                        else
                        {
                            List<int> toBeRemoved = new List<int>();
                            if (options.HandleCreatedLocalSelfIntersections)
                            {
                                for (int i = 0; i < segments.Count; i++)
                                {
                                    var seg = segments[i];
                                    double l = seg.Length + seg.SegmentLengthChange * maxOffset;   //new length of segment
                                    if (l < distTol)    //Segment vansihes -> to be removed
                                    {
                                        //If smaller than tolerance, add to be removed from segment and vertex lists
                                        toBeRemoved.Add(i);

                                        //Set vertices and segments that are to be recomputed in terms of translation vectors
                                        vertices[(i + 1) % vertices.Count].ComputeTranslation = true;
                                    }
                                    else
                                        segments[i].Length = l;
                                }

                                foreach (int i in toBeRemoved.OrderByDescending(x => x))
                                {
                                    vertices.RemoveAt(i);
                                    segments.RemoveAt(i);
                                }

                            }
                            if (options.HandleGeneralCreatedSelfIntersections)
                            {
                                if (toBeRemoved.Count == 0)
                                {
                                    offsets.Insert(0, offset);
                                    finalVertices.AddRange(CheckIntersectionAndCullSegments(segments, vertices, offsets, maxOffset, normal, isClosed, options, onlyLargestPerStep, distTol, angleTol));
                                    return finalVertices;
                                }
                                else
                                {
                                    foreach (OffsetVertex vertex in vertices)
                                    {
                                        if (vertex.AnySegmentInRangeForIntersection)
                                            vertex.ComputeIntersection = true;

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (options.HandleCreatedLocalSelfIntersections)
                        {
                            foreach (var segment in segments)
                            {
                                segment.Length += segment.SegmentLengthChange * maxOffset;
                            }
                        }
                        if (options.HandleGeneralCreatedSelfIntersections)
                        {
                            foreach (OffsetVertex vertex in vertices)
                            {
                                if (vertex.SegmentIntersected != -1)
                                    vertex.OffsetUntilIntersection -= maxOffset;
                            }
                        }
                    }


                    if (vertices.Count < 2 || (isClosed ? segments.Count < 2 : segments.Count < 1)) //Nothing left - exit
                        return finalVertices;

                    firstIteration = false;
                }
                if (vertices.Count >= 2)
                {
                    List<OffsetVertex> finalpLine = vertices.Select(x => x.Clone()).ToList();
                    if (isClosed)
                        finalpLine.Add(finalpLine[0]);

                    finalVertices.Add(finalpLine);
                }
                else
                    return finalVertices;
            }

            return finalVertices;
        }

        /***************************************************/

        private static OffsetVertex Clone(this OffsetVertex v)
        {
            return new OffsetVertex
            {
                Position = v.Position,
                AdjacentSegmentLengthChange = v.AdjacentSegmentLengthChange,
                AnySegmentInRangeForIntersection = v.AnySegmentInRangeForIntersection,
                ComputeIntersection = v.ComputeIntersection,
                ComputeTranslation = v.ComputeTranslation,
                OffsetUntilIntersection = v.OffsetUntilIntersection,
                SegmentIntersected = v.SegmentIntersected,
                Translation = v.Translation,
            };
        }

        /***************************************************/

        private static OffsetSegment Clone(this OffsetSegment s, bool flipOrtho)
        {
            return new OffsetSegment
            {
                Length = s.Length,
                Tangent = s.Tangent,
                Orthogonal = flipOrtho ? - s.Orthogonal : s.Orthogonal,
                ComputeLengthChange = s.ComputeLengthChange,
                RecomputeLength = s.RecomputeLength,
                SegmentLengthChange = s.SegmentLengthChange
            };
        }

        /***************************************************/

        [Description("Support method for Polyline offsets for the case of two adjecent polyline segments creating a acute angle within provided tolerance. This case is handled either by vertex and segment removal, or introduction of additional vertices.")]
        private static double ComputeMaxOffsetUntilIntersection(List<OffsetSegment> segments, List<OffsetVertex> vertices, double offset, bool isClosed, double distTol)
        {
            double sqTol = distTol * distTol;
            double overallMaxOffset = offset;
            for (int i = 0; i < vertices.Count; i++)
            {
                OffsetVertex vertex = vertices[i];
                if (!vertex.ComputeIntersection)    //Intersection check does not need to be recomputed
                    continue;

                vertex.ComputeIntersection = false;
                vertex.AnySegmentInRangeForIntersection = false;    //Set flag to false to start

                Vector trans = vertex.Translation;
                double maxOffset = offset;
                int segmentIntersected = -1;

                List<Tuple<int, double>> segmentPerpDistance = new List<Tuple<int, double>>();

                for (int j = 0; j < segments.Count; j++)
                {
                    //Skip segments adjecent
                    if (i == j)
                        continue;

                    if (i == 0 && isClosed)
                    {
                        if (j == segments.Count - 1)
                            continue;
                    }
                    else if (j == i - 1)
                        continue;

                    OffsetSegment segment = segments[j];
                    Vector segOrtho = segment.Orthogonal;
                    var startVertex = vertices[j];
                    Point segStart = startVertex.Position;

                    double dot1 = segOrtho.DotProduct(segStart - vertex.Position);
                    double dot2 = segOrtho.DotProduct(trans);
                    double reqOffset = dot1 / (dot2 - 1); //Required offset for the vertex to end up on the segment-plane after it has been offset

                    if (reqOffset > -distTol && reqOffset < offset)  //Offset in the correct direction and samller than previous max.
                    {
                        if (reqOffset < 0)
                            reqOffset = 0;
                        vertex.AnySegmentInRangeForIntersection = true;
                        segmentPerpDistance.Add(new Tuple<int, double>(j, reqOffset));
                    }
                }

                segmentPerpDistance = segmentPerpDistance.OrderBy(x => x.Item2).ToList();

                foreach (var segmentPerpDist in segmentPerpDistance)
                {
                    double reqOffset = segmentPerpDist.Item2;
                    int j = segmentPerpDist.Item1;

                    //Check if the point ends up on the finite segment
                    Point vOff = vertex.Position + reqOffset * trans;  //Point translated with offset
                    var startVertex = vertices[j];
                    Point stOff = startVertex.Position + reqOffset * startVertex.Translation;
                    var enVertex = vertices[(j + 1) % vertices.Count];
                    Point enOff = enVertex.Position + reqOffset * enVertex.Translation;


                    double stSqDist = vOff.SquareDistance(stOff);

                    bool inRange = stSqDist < sqTol;    //On start point within tolerance
                    if (!inRange)
                    {
                        double enSqDist = vOff.SquareDistance(enOff);
                        inRange = enSqDist < sqTol; //On end point within tolerance
                        if (!inRange)
                        {
                            double sqLength = stOff.SquareDistance(enOff);
                            inRange = stSqDist < sqLength && enSqDist < sqLength;   //On line
                        }
                    }

                    if (inRange)   //Point ends on line
                    {
                        maxOffset = reqOffset;
                        segmentIntersected = j;
                        break;
                    }
                }

                if (segmentIntersected != -1)
                {
                    vertices[i].OffsetUntilIntersection = maxOffset;
                    vertices[i].SegmentIntersected = segmentIntersected;
                }
                else if (vertex.SegmentIntersected != -1)
                {
                    vertices[i].OffsetUntilIntersection = double.MaxValue;
                    vertices[i].SegmentIntersected = -1;
                }
            }

            foreach (var vertex in vertices)
            {
                if (vertex.SegmentIntersected != -1)
                    overallMaxOffset = Math.Min(vertex.OffsetUntilIntersection, overallMaxOffset);
            }
            return overallMaxOffset;
        }

        /***************************************************/

        private static List<List<OffsetVertex>> CheckIntersectionAndCullSegments(List<OffsetSegment> segments, List<OffsetVertex> vertices, List<double> offsets, double currentOfsetStep, Vector normal, bool isClosed, OffsetOptions options, bool onlyLargestPerStep, double distTol, double angleTol)
        {

            List<Tuple<int, int>> vertSegInter = new List<Tuple<int, int>>();
            for (int i = 0; i < vertices.Count; i++)
            {
                var v = vertices[i];
                if (v.SegmentIntersected != -1)
                {
                    double remOffset = vertices[i].OffsetUntilIntersection - currentOfsetStep;
                    if (remOffset < distTol)    //Intersection between vertex and another segment
                    {
                        vertSegInter.Add(new Tuple<int, int>(i, v.SegmentIntersected));
                        v.ComputeTranslation = true;
                        v.ComputeIntersection = true;
                        segments[v.SegmentIntersected].RecomputeLength = true;
                    }
                    else
                        vertices[i].OffsetUntilIntersection = remOffset;
                }
            }

            if (vertSegInter.Count > 0)
            {
                List<List<int>> vertexLoops;
                List<List<int>> segmentLoops;
                GenerateLoops(vertSegInter, vertices.Count, out vertexLoops, out segmentLoops);

                if (!isClosed)
                {
                    List<int> firstLoop = segmentLoops.First();
                    firstLoop.RemoveAt(firstLoop.Count - 1);
                }

                if (onlyLargestPerStep)
                {
                    List<int> largestVertexLoop = new List<int>();
                    List<int> largestSegmentLoop = new List<int>();
                    if (isClosed)
                    {
                        double maxArea = double.MinValue;

                        for (int i = 0; i < vertexLoops.Count; i++)
                        {
                            double area = LoopArea(vertexLoops[i], vertices, normal);
                            if (area > maxArea)
                            {
                                maxArea = area;
                                largestVertexLoop = vertexLoops[i];
                                largestSegmentLoop = segmentLoops[i];
                            }
                        }
                    }
                    else
                    {
                        largestVertexLoop = vertexLoops[0];
                        largestSegmentLoop = segmentLoops[0];
                    }

                    List<OffsetVertex> largeVertices;
                    List<OffsetSegment> largeSegements;
                    ItemsFromLoops(vertices, segments, largestVertexLoop, largestSegmentLoop, false, out largeVertices, out largeSegements);
                    return Iterate(largeSegements, largeVertices, offsets, normal, isClosed, options, onlyLargestPerStep, distTol, angleTol, false);
                }
                else
                {
                    List<List<OffsetVertex>> finalVertices = new List<List<OffsetVertex>>();
                    for (int i = 0; i < vertexLoops.Count; i++)
                    {
                        if (vertexLoops[i].Count <= 2)  //Skip loops with 2 or less vertices, except for open curves if it is the first segment
                        {
                            if (isClosed)
                                continue;
                            else if (i != 0)
                                continue;
                        }
                        List<OffsetVertex> loopVs;
                        List<OffsetSegment> loopSegs;
                        ItemsFromLoops(vertices, segments, vertexLoops[i], segmentLoops[i], true, out loopVs, out loopSegs);
                        finalVertices.AddRange(Iterate(loopSegs, loopVs, offsets.ToList(), normal, isClosed || i > 0, options, onlyLargestPerStep, distTol, angleTol, false));
                    }
                    return finalVertices;
                }
            }
            else
                return Iterate(segments, vertices, offsets, normal, isClosed, options, onlyLargestPerStep, distTol, angleTol, false);
            
        }

        /***************************************************/

        private static void ItemsFromLoops(List<OffsetVertex> vertices, List<OffsetSegment> segments, List<int> vertexLoop, List<int> segmentLoop, bool clone, out List<OffsetVertex> loopVs, out List<OffsetSegment> loopSegs)
        {
            if (clone)
            {
                loopVs = vertexLoop.Select(i => vertices[i].Clone()).ToList();
                loopSegs = segmentLoop.Select(i => segments[i].Clone(false)).ToList();
            }
            else
            {
                loopVs = vertexLoop.Select(i => vertices[i]).ToList();
                loopSegs = segmentLoop.Select(i => segments[i]).ToList();
            }

            if (segmentLoop[0] < vertexLoop[0])
            {
                var temp = loopSegs[0];
                loopSegs.RemoveAt(0);
                loopSegs.Add(temp);
            }

            for (int i = 0; i < loopSegs.Count; i++)
            {
                if (loopSegs[i].RecomputeLength)
                    loopSegs[i].Length = loopVs[i].Position.Distance(loopVs[(i + 1) % loopVs.Count].Position);
            }

            foreach (var vertex in loopVs)
            {
                if (vertex.AnySegmentInRangeForIntersection)
                    vertex.ComputeIntersection = true;
            }
        }

        /***************************************************/

        private static void GenerateLoops(List<Tuple<int, int>> vertSegInter, int count, out List<List<int>> verticeLoops, out List<List<int>> segmentLoops)
        {
            List<Tuple<int, int>> vertSegInterUsed = new List<Tuple<int, int>>();

            Dictionary<int, List<int>> vertLoopDict = new Dictionary<int, List<int>>();
            Dictionary<int, List<int>> segLoopDict = new Dictionary<int, List<int>>();

            int k = 0;
            vertLoopDict[k] = new List<int>();
            segLoopDict[k] = new List<int>();
            List<int> segIntersected = new List<int>();
            for (int i = 0; i < count; i++)
            {
                bool intersected = false;

                if (vertSegInterUsed.Any())
                {

                    if (vertSegInterUsed.Last().Item1 == i)
                    {
                        vertLoopDict[k].Add(i);
                        k -= IncKey(vertSegInterUsed.Last().Item1, count);
                        vertLoopDict[k].Add(i);
                        segLoopDict[k].Add(i);
                        vertSegInterUsed.RemoveAt(vertSegInterUsed.Count - 1);
                        intersected = true;
                    }

                    if (vertSegInterUsed.Any() && vertSegInterUsed.Last().Item2 == i)
                    {
                        vertLoopDict[k].Add(i);
                        segLoopDict[k].Add(i);
                        while (vertSegInterUsed.Any() && vertSegInterUsed.Last().Item2 == i)
                        {
                            k -= IncKey(vertSegInterUsed.Last().Item1, count);
                            segLoopDict[k].Add(i);
                            vertSegInterUsed.RemoveAt(vertSegInterUsed.Count - 1);
                        }

                        intersected = true;
                    }
                }

                int ind = vertSegInter.FindIndex(x => x.Item1 == i);
                if (ind != -1)
                {
                    vertLoopDict[k].Add(i);
                    k += IncKey(vertSegInter[ind].Item1, count);

                    segIntersected.Add(vertSegInter[ind].Item2);
                    vertLoopDict[k] = new List<int>();
                    segLoopDict[k] = new List<int>();

                    vertLoopDict[k].Add(i);
                    segLoopDict[k].Add(i);
                    vertSegInterUsed.Add(vertSegInter[ind]);
                    vertSegInter.RemoveAt(ind);

                    intersected = true;
                }

                var segInters = vertSegInter.Where(x => x.Item2 == i).OrderByDescending(x => x.Item1);
                if (segInters.Any())
                {
                    if (!segIntersected.Contains(i))
                    {
                        vertLoopDict[k].Add(i);
                        segLoopDict[k].Add(i);
                    }
                    foreach (var segInter in segInters)
                    {
                        k += IncKey(segInter.Item1, count);

                        vertLoopDict[k] = new List<int>();
                        segLoopDict[k] = new List<int>();

                        segLoopDict[k].Add(i);
                        vertSegInterUsed.Add(segInter);
                        vertSegInter.Remove(segInter);
                    }
                    intersected = true;
                }

                if (!intersected)
                {
                    vertLoopDict[k].Add(i);
                    segLoopDict[k].Add(i);
                }
            }

            verticeLoops = vertLoopDict.Values.ToList();
            segmentLoops = segLoopDict.Values.ToList();

        }

        /***************************************************/

        private static int IncKey(int i, int count)
        {
            int key = (i + count) * 3;
            return key;
        }

        /***************************************************/

        private static double LoopArea(List<int> loop, List<OffsetVertex> vertices, Vector normal)
        {
            if (loop.Count < 3)
                return 0;

            List<Point> pts = new List<Point>();
            for (int i = 0; i < loop.Count; i++)
            {
                pts.Add(vertices[loop[i]].Position);
            }

            Point avg = pts.Average();
            pts.Add(vertices[loop[0]].Position);
            double x = 0, y = 0, z = 0;
            for (int i = 0; i < pts.Count; i++)
            {
                int j = (i + 1) % pts.Count;
                Vector prod = Query.CrossProduct(pts[i] - avg, pts[j] - avg);
                x += prod.X;
                y += prod.Y;
                z += prod.Z;
            }

            return Math.Abs((new Vector { X = x, Y = y, Z = z } * normal) * 0.5);
        }

        /***************************************************/

        [Description("Support method for Polyline offsets for the case of two adjecent polyline segments creating a acute angle within provided tolerance. This case is handled either by vertex and segment removal, or introduction of additional vertices.")]
        private static bool HandleParalellAdjecentPolylinearSegments(List<OffsetSegment> segments, List<OffsetVertex> vertices, Vector normal, bool isClosed, bool firstIteration, double distTol, int i, int prev, double angleTol)
        {
            bool outwards = false;
            if (firstIteration) //Only need to check if "Inwards" or "Outwards" for first iteration, as subsequent iterations all can be assumed to be inwards (vanishing)
                outwards = CheckAcuteCornerOutwards(segments, isClosed, i, prev, angleTol, distTol);

            if (outwards)
            {
                //For the case of outwards, an additional vertex and segment is inserted, creating a right angled corner from the acute corner angle
                vertices.Insert(i, new OffsetVertex() { Position = vertices[i].Position, ComputeTranslation = true });
                Vector tanNew = (segments[i].Orthogonal - segments[prev].Orthogonal).Normalise();
                segments.Insert(i, new OffsetSegment() { Length = 0, Tangent = tanNew, Orthogonal = tanNew.CrossProduct(normal).Normalise(), SegmentLengthChange = 0, ComputeLengthChange = true });


                //More segments need to be recomputed due to the insertion and change
                segments[prev].ComputeLengthChange = true;
                segments[i].ComputeLengthChange = true;
                segments[(i + 1) % segments.Count].ComputeLengthChange = true;

                //Same for vertecies requiring re-computation
                vertices[(i + 1) % vertices.Count].ComputeTranslation = true;

            }
            else
            {
                if (vertices[prev].Position.Distance(vertices[(i + 1) % vertices.Count].Position) < distTol)
                {
                    //If the corner removed leads to the previous and next points being adjecent, remove two verticesm as well as the two segments leading up to the removed vertex

                    vertices[(i + 1) % vertices.Count].ComputeTranslation = true;    //Kept vertex needs recomputation of translation vector

                    vertices.RemoveAt(i);
                    segments.RemoveAt(i);
                    if (i < prev)
                        prev--;

                    vertices.RemoveAt(prev);
                    segments.RemoveAt(prev);

                    if (vertices.Count <= 1)
                        return false;   //Vertex list small enough that nothing is left. Method to return empty polyline.

                    if (i >= 2)
                    {
                        segments[i - 2].ComputeLengthChange = true;
                        segments[(i - 1) % segments.Count].ComputeLengthChange = true;
                    }
                    else
                    {
                        segments[0].ComputeLengthChange = true;
                        if (isClosed)
                            segments[segments.Count - 1].ComputeLengthChange = true;
                    }
                }
                else
                {
                    //If not, remove one of the segments and one of the vertices
                    vertices.RemoveAt(i);
                    segments.RemoveAt(i);

                    if (vertices.Count <= 1)
                        return false;   //Vertex list small enough that nothing is left. Method to return empty polyline.

                    if (i < prev)
                        prev--;

                    //And recompute the new direction for the other
                    Vector tan = vertices[i % vertices.Count].Position - vertices[prev].Position;
                    double lengthNew = tan.Length();
                    tan = tan * (1 / lengthNew);
                    segments[prev] = new OffsetSegment() { Length = lengthNew, Tangent = tan, Orthogonal = tan.CrossProduct(normal).Normalise(), SegmentLengthChange = 0, ComputeLengthChange = true };

                    vertices[prev].ComputeTranslation = true;
                    vertices[i % vertices.Count].ComputeTranslation = true;
                    segments[i % segments.Count].ComputeLengthChange = true;

                }
            }

            return true;
        }

        /***************************************************/

        [Description("Method that finds if a particular corner should be lead to an outwards of inwards offset.")]
        private static bool CheckAcuteCornerOutwards(List<OffsetSegment> segments, bool isClosed, int i, int prev, double angleTol, double distTol)
        {

            Vector t1 = segments[prev].Tangent;

            //Checks whether the offset is "outwards" or "inwards" for the current corner
            int i0 = i - 2;

            if (isClosed && i0 < 0)
                i0 = segments.Count + i0;

            Vector t0 = null;   //Tangent vector of the segment coming into the corner before the current vertex being evaluated. 
            List<int> beforeSegs = new List<int>() { prev };

            if (i0 >= 0)
            {
                t0 = segments[i0].Tangent;

                while (t0.IsParallel(t1, angleTol) != 0)    //Ensure not parallel with segments going into the corner. If so, take the one before
                {
                    beforeSegs.Add(i0);
                    i0--;
                    if (i0 < 0)
                    {
                        if (isClosed)
                            i0 = segments.Count - 1;    //Set index to last for closed curves when looping around the list
                        else
                        {
                            t0 = null;  //No suitable vector exists
                            break;
                        }
                    }
                    if (i0 == i)    //Looped all the way around, all segments are parallel
                    {
                        return false;
                    }
                    t0 = segments[i0].Tangent;
                }
            }


            int i3 = i + 1;
            if (isClosed && i3 > segments.Count - 1)
                i3 = i3 % segments.Count;

            Vector t3 = null;   //Vector
            List<int> afterSegs = new List<int>() { i };
            if (i3 <= segments.Count - 1)
            {
                t3 = segments[i3].Tangent;

                while (t3.IsParallel(t1, angleTol) != 0)
                {
                    afterSegs.Add(i3);
                    i3++;
                    if (i3 > segments.Count - 1)
                    {
                        if (isClosed)
                            i3 = 0;
                        else
                        {
                            t3 = null;
                            break;
                        }
                    }
                    if (i3 == prev)    //Looped all the way around, all segments are parallel
                    {
                        return false;
                    }

                    t3 = segments[i3].Tangent;

                }
            }

            if (t0 != null && t3 != null)
            {
                double l1 = beforeSegs.Sum(x => segments[x].Length);// segments[prev].Item1;
                double l2 = afterSegs.Sum(x => segments[x].Length);// segments[i].Item1;
                double lengthDif = l1 - l2;

                //Dot products used for sign, if dotproduct is positive, the vectors are pointing in roungly the same direction
                double t0n1 = t0.DotProduct(segments[prev].Orthogonal);
                double t3n1 = t3.DotProduct(segments[prev].Orthogonal);

                if (Math.Abs(lengthDif) < distTol)
                {
                    if (t0n1 * t3n1 < 0)
                    {
                        //Same side
                        if (t0n1 < 0)
                            return -t0.DotProduct(t1) > t3.DotProduct(t1);
                        else
                            return -t0.DotProduct(t1) < t3.DotProduct(t1);
                    }
                    else
                        return t0n1 < 0;
                }
                else if (lengthDif < 0)
                    return t0n1 < 0;
                else
                    return t3n1 < 0;

            }
            else if (t3 != null)
            {
                //For case of no incoming vector (t0) the corner is outwards if the tangent of the next vector is pointing in the oposite direction to the orthogonal vector coing into the corner
                return t3.DotProduct(segments[prev].Orthogonal) < 0;
            }
            else if (t0 != null)
            {
                //For case of no outgoing vector (t3) the corner is outwards if the tangent of the incoming vector is pointing in the oposite direction to the orthogonal vector coing into the corner
                return t0.DotProduct(segments[prev].Orthogonal) < 0;
            }

            return false;
        }

        /***************************************************/
    }
}

