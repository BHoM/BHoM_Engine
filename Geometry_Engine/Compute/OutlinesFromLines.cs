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
using BH.oM.Geometry;
using BH.oM.Geometry.CoordinateSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /****              Public Methods               ****/
        /***************************************************/

        //[Description("Split an outer region by the cutting lines into a collection of closed contained regions within the outer region.")]
        //[Input("outerRegion", "An outer region that will contain the closed regions generated.")]
        //[Input("cuttingLines", "The lines to cut the outer region by.")]
        //[Input("distanceTolerance", "Tolerance to use for distance measurment operations, default to BH.oM.Geometry.Tolerance.Distance.")]
        //[Input("decimalPlaces", "All coordinates of the geometry will be rounded to the number of decimal places specified. Default 6.")]
        //[Output("regions", "Closed polygon regions contained within the outer region cut by the cutting lines.")]
        public static List<Polyline> OutlinesFromLines(this List<Line> lines, double distanceTolerance = Tolerance.Distance)
        {
            if (lines == null)
                return null;

            lines = lines.CleanUpAndSplitWithEachOther(distanceTolerance);
            List<List<Line>> clustered = lines.Cluster(distanceTolerance);

            List<Polyline> result = new List<Polyline>();
            for (int i = 0; i < clustered.Count; i++)
            {
                var cluster = clustered[i];
                cluster.RemoveOutliers(distanceTolerance);
                if (cluster.Count == 0)
                    continue;

                if (!cluster.IsCoplanar(distanceTolerance))
                {
                    BH.Engine.Base.Compute.RecordWarning("Some of the lines have been ignored in the process of creating outlines because they were not coplanar.");
                    continue;
                }

                List<Line> outline = cluster.ClusterOutline(distanceTolerance);
                List<Line> inner = cluster.Except(outline).ToList();

                result.AddRange(LinesToPolygons(outline, inner, distanceTolerance));
            }

            return result;
        }


        /***************************************************/
        /****              Private Methods              ****/
        /***************************************************/

        private static List<Line> CleanUpAndSplitWithEachOther(this List<Line> lines, double distanceTolerance)
        {
            lines = lines.BooleanUnion(distanceTolerance, true);
            List<Point> intersectingPoints = Query.LineIntersections(lines).CullDuplicates(distanceTolerance);
            return lines.SelectMany(x => x.SplitAtPoints(intersectingPoints)).ToList();
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

            //TODO: more efficient if made a recursive function starting from valence one and dfs?
            while (true)
            {
                List<Point> valenceOne = new List<Point>();
                foreach (var key in nodesByValence.Keys.ToList())
                {
                    if (nodesByValence[key] == 0)
                        valenceOne.Add(key);
                    else if (nodesByValence[key] == 1)
                    {
                        valenceOne.Add(key);
                        int index = lines.FindIndex(x => x.Start == key || x.End == key);
                        Line l = lines[index];
                        lines.RemoveAt(index);

                        if (l.Start == key)
                            nodesByValence[l.End]--;
                        else
                            nodesByValence[l.Start]--;
                    }
                }

                if (valenceOne.Count == 0)
                    break;

                foreach (Point p in valenceOne)
                {
                    nodesByValence.Remove(p);
                }
            }

            if (lines.Count != initialCount)
                BH.Engine.Base.Compute.RecordNote("Lines without a valid node at end have been ignored in the process of creating outlines.");
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

        private static List<Line> ClusterOutline(this List<Line> lines, double distanceTolerance)
        {
            TransformMatrix toGlobal = lines.TransformToGlobalXY();
            if (toGlobal == null)
            {
                BH.Engine.Base.Compute.RecordWarning("Some of the lines have been ignored in the process of creating outlines because they were all collinear.");
                return null;
            }

            List<Line> transformed = lines.Select(x => x.Transform(toGlobal)).ToList();
            Dictionary<Line, (Point, Point)> endpoints = transformed.ToDictionary(x => x, x => (x.Start, x.End));
            Dictionary<Line, Vector> dirs = transformed.ToDictionary(x => x, x => x.Direction());
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

            Point leftmost = graph.Keys.First();
            foreach (Point node in graph.Keys)
            {
                if (node.X < leftmost.X)
                    leftmost = node;
            }

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

            while (true)
            {
                if (currentNode == currentEdge.Start)
                    currentNode = currentEdge.End;
                else
                    currentNode = currentEdge.Start;

                if (currentNode == leftmost)
                    break;

                Vector headingLeft = new Vector { X = -currentDir.Y, Y = currentDir.X };

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

            return outline.Select(x => lines[transformed.IndexOf(x)]).ToList();
        }

        /***************************************************/
    }
}
