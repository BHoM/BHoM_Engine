/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2023, the respective contributors. All rights reserved.
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

using BH.Engine.Data;
using BH.oM.Base;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        [Description("Combines an unordered set of lines and turns them into closed cells, e.g. mesh edges will be combined into a set of outlines each representing one mesh cell.")]
        [Input("lines", "Unordered set of lines to turn into closed cells.")]
        [Input("distanceTolerance", "Tolerance to use for geometrical processing.")]
        [Output("outlines", "Closed outines created from the input lines.")]
        public static List<Polyline> OutlinesFromLines(this List<Line> lines, double distanceTolerance = Tolerance.Distance)
        {
            lines = lines?.CleanUpAndSplitWithEachOther(distanceTolerance);
            if (lines == null)
            {
                BH.Engine.Base.Compute.RecordError("Outlines could not be created because the input set is not valid.");
                return null;
            }

            List<List<Line>> clustered = lines.Cluster(distanceTolerance);

            List<Polyline> result = new List<Polyline>();
            for (int i = 0; i < clustered.Count; i++)
            {
                var cluster = clustered[i];
                cluster.RemoveOutliers(distanceTolerance);
                if (cluster.Count == 0)
                    continue;

                Plane fitPlane = cluster.SelectMany(x => x.ControlPoints()).ToList().FitPlane();
                if (fitPlane == null)
                {
                    BH.Engine.Base.Compute.RecordWarning("Some of the lines have been ignored in the process of creating outlines because they were not coplanar.");
                    continue;
                }

                cluster = cluster.Select(x => x.Project(fitPlane)).ToList();

                Output<Polyline, List<Line>> preprocessedLines = cluster.OuterAndInnerLines(distanceTolerance);
                result.AddRange(OutlinesFromPreprocessedLines(preprocessedLines.Item1, preprocessedLines.Item2, distanceTolerance));
            }

            return result;
        }


        /***************************************************/
        /****              Private Methods              ****/
        /***************************************************/

        private static List<Line> CleanUpAndSplitWithEachOther(this List<Line> lines, double distanceTolerance)
        {
            double sqTol = distanceTolerance * distanceTolerance;
            lines = lines.BooleanUnion(distanceTolerance, true);
            List<Point> intersectingPoints = Query.LineIntersections(lines).CullDuplicates(distanceTolerance);
            return lines.SelectMany(x => x.SplitAtPoints(intersectingPoints)).Where(x => x.SquareLength() > sqTol).ToList().CullDuplicateLines(distanceTolerance);
        }

        /***************************************************/

        private static List<List<Line>> Cluster(this List<Line> lines, double distanceTolerance)
        {
            double sqTol = distanceTolerance * distanceTolerance;
            Func<Line, Line, bool> distanceFunction = (a, b) =>
            a.Start.SquareDistance(b.Start) < sqTol ||
            a.Start.SquareDistance(b.End) < sqTol ||
            a.End.SquareDistance(b.Start) < sqTol ||
            a.End.SquareDistance(b.End) < sqTol;
            return lines.ClusterDBSCAN(distanceFunction);
        }

        /***************************************************/

        private static void RemoveOutliers(this List<Line> lines, double distanceTolerance)
        {
            int initialCount = lines.Count;

            Dictionary<Point, int> nodesByValence = new Dictionary<Point, int>();
            foreach (Line l in lines)
            {
                if (!nodesByValence.ContainsKey(l.Start))
                    nodesByValence.Add(l.Start, 0);

                nodesByValence[l.Start]++;

                if (!nodesByValence.ContainsKey(l.End))
                    nodesByValence.Add(l.End, 0);

                nodesByValence[l.End]++;
            }

            foreach (Point point in nodesByValence.Keys.ToList())
            {
                if (nodesByValence.ContainsKey(point))
                    point.CullValenceOne(nodesByValence, lines, distanceTolerance);
            }

            if (lines.Count != initialCount)
                BH.Engine.Base.Compute.RecordNote("Lines without a valid node at end have been ignored in the process of creating outlines.");
        }

        /***************************************************/

        private static void CullValenceOne(this Point point, Dictionary<Point, int> nodesByValence, List<Line> lines, double distanceTolerance)
        {
            int nodeValence = nodesByValence[point];
            if (nodeValence == 0)
                nodesByValence.Remove(point);
            if (nodeValence == 1)
            {
                int index = lines.FindIndex(x => x.Start == point || x.End == point);
                Line l = lines[index];
                lines.RemoveAt(index);
                nodesByValence.Remove(point);

                if (l.Start == point)
                {
                    nodesByValence[l.End]--;
                    l.End.CullValenceOne(nodesByValence, lines, distanceTolerance);
                }
                else
                {
                    nodesByValence[l.Start]--;
                    l.Start.CullValenceOne(nodesByValence, lines, distanceTolerance);
                }
            }
        }

        /***************************************************/

        private static TransformMatrix TransformToGlobalXY(this List<Line> lines)
        {
            Vector dir1 = lines[0].Direction();
            Vector dir2 = lines.FirstOrDefault(x => x.Direction().IsParallel(dir1) == 0)?.Direction();
            if (dir2 == null)
                return null;

            Vector normal = dir1.CrossProduct(dir2);
            Cartesian local = Create.CartesianCoordinateSystem(lines[0].Start, dir1, dir1.CrossProduct(normal));
            return local.OrientationMatrix(new Cartesian());
        }

        /***************************************************/

        private static Output<Polyline, List<Line>> OuterAndInnerLines(this List<Line> lines, double distanceTolerance)
        {
            // Transform the lines to global XY plane
            TransformMatrix toGlobal = lines.TransformToGlobalXY();
            if (toGlobal == null)
            {
                BH.Engine.Base.Compute.RecordWarning("Some of the lines have been ignored in the process of creating outlines because they were all collinear.");
                return null;
            }

            List<Line> transformed = lines.Select(x => x.Transform(toGlobal)).ToList();

            // Turn the lines into a graph represented as a dictionary of nodes and adjoining edges
            Dictionary<Point, List<Line>> graph = new Dictionary<Point, List<Line>>();
            foreach (Line line in transformed)
            {
                if (!graph.ContainsKey(line.Start))
                    graph.Add(line.Start, new List<Line>());

                if (!graph.ContainsKey(line.End))
                    graph.Add(line.End, new List<Line>());

                graph[line.Start].Add(line);
                graph[line.End].Add(line);
            }

            // Start at leftmost node
            Point leftmost = graph.Keys.First();
            foreach (Point node in graph.Keys)
            {
                if (node.X < leftmost.X)
                    leftmost = node;
            }

            // Find edge starting at start node with leftmost dir
            Line currentEdge = null;
            Vector currentDir = null;
            foreach (Line l in graph[leftmost])
            {
                var dir = l.Direction();
                if (l.Start != leftmost)
                    dir = dir.Reverse();

                if (currentDir == null || currentDir.Y < dir.Y)
                {
                    currentEdge = l;
                    currentDir = dir;
                }
            }

            List<Line> outline = new List<Line> { currentEdge };
            Point currentNode = leftmost;

            // Look for subsequent edges with leftmost dirs compared to the dir of current edge
            while (true)
            {
                if (currentNode == currentEdge.Start)
                    currentNode = currentEdge.End;
                else
                    currentNode = currentEdge.Start;

                if (currentNode == leftmost)
                    break;

                List<Vector> nodeDirs = new List<Vector>();
                foreach (Line l in graph[currentNode])
                {
                    Vector dir = l.Direction();
                    if (l.Start != currentNode)
                        dir = dir.Reverse();

                    nodeDirs.Add(dir);
                }

                Vector newDir = null;
                Line newEdge = null;

                // To avoid computing heavy signed angle, first check only dirs to the left from the current dir
                Vector headingLeft = new Vector { X = -currentDir.Y, Y = currentDir.X };
                for (int j = 0; j < nodeDirs.Count; j++)
                {
                    if (graph[currentNode][j] == currentEdge)
                        continue;

                    if (headingLeft.DotProduct(nodeDirs[j]) < 0)
                        continue;

                    if (newDir == null || nodeDirs[j].DotProduct(currentDir) < newDir.DotProduct(currentDir))
                    {
                        newDir = nodeDirs[j];
                        newEdge = graph[currentNode][j];
                    }
                }

                // If not found (and only if), then look up the ones to the right
                if (newEdge == null)
                {
                    for (int j = 0; j < nodeDirs.Count; j++)
                    {
                        if (graph[currentNode][j] == currentEdge)
                            continue;

                        if (headingLeft.DotProduct(nodeDirs[j]) >= 0)
                            continue;

                        if (newDir == null || nodeDirs[j].DotProduct(currentDir) > newDir.DotProduct(currentDir))
                        {
                            newDir = nodeDirs[j];
                            newEdge = graph[currentNode][j];
                        }
                    }
                }

                currentEdge = newEdge;
                currentDir = newDir;

                outline.Add(currentEdge);
            }

            List<Line> outerSegments = outline.Select(x => lines[transformed.IndexOf(x)]).ToList();
            Polyline outer = outerSegments.Join(distanceTolerance).First();
            List<Line> inner = lines.Except(outerSegments).ToList();

            return new Output<Polyline, List<Line>> { Item1 = outer, Item2 = inner };
        }

        /***************************************************/

        private static List<Polyline> OutlinesFromPreprocessedLines(this Polyline outer, List<Line> inner, double distanceTolerance)
        {
            double sqTol = distanceTolerance * distanceTolerance;

            // Transform to global XY plane
            TransformMatrix toGlobal = outer.SubParts().TransformToGlobalXY();
            if (toGlobal == null)
            {
                BH.Engine.Base.Compute.RecordWarning("Some of the lines have been ignored in the process of creating outlines because they were all collinear.");
                return null;
            }

            // If outline normal is negative, flip it to make sure it is clockwise
            Polyline transformedOuter = outer.Transform(toGlobal);
            if (transformedOuter.Normal().Z < 0)
                outer = outer.Flip();

            // Combine outer edges and inner ones in both directions
            List<Line> all = outer.SubParts().Union(inner).Union(inner.Select(x => x.Flip())).ToList();
            
            // Transform the lines to global XY plane, find their dirs
            List<Line> transformed = all.Select(x => x.Transform(toGlobal)).ToList();            
            Dictionary<Line, Vector> dirs = transformed.ToDictionary(x => x, x => x.Direction());
            
            // Turn the lines into a graph represented as a dictionary of nodes and adjoining edges
            Dictionary<Point, List<Line>> graph = new Dictionary<Point, List<Line>>();
            foreach (Line line in transformed)
            {
                if (!graph.ContainsKey(line.Start))
                    graph.Add(line.Start, new List<Line>());

                graph[line.Start].Add(line);
            }

            // Iteratively pick a random graph edge and go leftwards until a closed outline is found
            // Repeat until the graph is emptied
            List<List<Line>> result = new List<List<Line>>();
            while (graph.Count != 0)
            {
                // Start from arbitrary node and edge
                Point startNode = graph.Keys.First();
                Line currentEdge = graph[startNode].First();
                graph[startNode].Remove(currentEdge);
                if (graph[startNode].Count == 0)
                    graph.Remove(startNode);

                Vector currentDir = dirs[currentEdge];
                Point currentNode = currentEdge.End;

                // Keep on turning left until hitting the start node
                List<Line> outline = new List<Line> { currentEdge };
                while (currentNode != startNode)
                {
                    List<Vector> nodeDirs = graph[currentNode].Select(x => dirs[x]).ToList();
                    Vector newDir = null;
                    Line newEdge = null;

                    // To avoid computing heavy signed angle, first check only dirs to the left from the current dir
                    Vector headingLeft = new Vector { X = -currentDir.Y, Y = currentDir.X };
                    for (int j = 0; j < nodeDirs.Count; j++)
                    {
                        // Avoid the edge itself as well as the same one in opposite direction
                        if (graph[currentNode][j] == currentEdge || 1 + currentDir.DotProduct(nodeDirs[j]) < sqTol)
                            continue;

                        if (headingLeft.DotProduct(nodeDirs[j]) < 0)
                            continue;

                        if (newDir == null || nodeDirs[j].DotProduct(currentDir) < newDir.DotProduct(currentDir))
                        {
                            newDir = nodeDirs[j];
                            newEdge = graph[currentNode][j];
                        }
                    }

                    // If not found (and only if), then look up the ones to the right
                    if (newEdge == null)
                    {
                        for (int j = 0; j < nodeDirs.Count; j++)
                        {
                        // Avoid the edge itself as well as the same one in opposite direction
                            if (graph[currentNode][j] == currentEdge || 1 + currentDir.DotProduct(nodeDirs[j]) < sqTol)
                                continue;

                            if (headingLeft.DotProduct(nodeDirs[j]) >= 0)
                                continue;

                            if (newDir == null || nodeDirs[j].DotProduct(currentDir) > newDir.DotProduct(currentDir))
                            {
                                newDir = nodeDirs[j];
                                newEdge = graph[currentNode][j];
                            }
                        }
                    }

                    if (newEdge == null)
                    {
                        BH.Engine.Base.Compute.RecordError("Outlines could not be created due to an internal error.");
                        return null;
                    }

                    graph[currentNode].Remove(newEdge);
                    if (graph[currentNode].Count == 0)
                        graph.Remove(currentNode);

                    currentEdge = newEdge;
                    currentDir = newDir;
                    currentNode = newEdge.End;

                    outline.Add(currentEdge);
                }

                result.Add(outline.Select(x => all[transformed.IndexOf(x)]).ToList());
            }

            return result.Select(x => x.Join().First().Simplify()).ToList();
        }

        /***************************************************/
    }
}
