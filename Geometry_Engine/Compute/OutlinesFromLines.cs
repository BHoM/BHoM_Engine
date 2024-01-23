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

using BH.oM.Geometry;
using BH.oM.Base.Attributes;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BH.oM.Data.Collections;
using System;
using BH.Engine.Data;
using BH.oM.Geometry.CoordinateSystem;
using System.Security.Cryptography.Xml;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
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

            lines = lines.BooleanUnion(distanceTolerance, true);
            List<Point> intersectingPoints = Query.LineIntersections(lines).CullDuplicates(distanceTolerance);
            lines = lines.SelectMany(x => x.SplitAtPoints(intersectingPoints)).ToList();

            // lines = lines.Select(x => x.RoundCoordinates(decimalPlaces)).ToList();
            //TODO: snap to grids? or not needed actually?
            //NEEDED! to make clustering etc. work

            //lines = lines.ToList();

            //List<Point> uniqueMids = new List<Point>();
            //for (int i = lines.Count - 1; i >= 0; i--)
            //{
            //    Point mid = (lines[i].Start + lines[i].End) / 2;
            //    if (uniqueMids.Contains(mid))
            //        lines.RemoveAt(i);
            //    else
            //        uniqueMids.Add(mid);
            //}

            double sqTol = distanceTolerance * distanceTolerance;
            Func<Line, Line, bool> distanceFunction = (a, b) =>
            a.Start.SquareDistance(b.Start) < sqTol ||
            a.Start.SquareDistance(b.End) < sqTol ||
            a.End.SquareDistance(b.Start) < sqTol ||
            a.End.SquareDistance(b.End) < sqTol;
            var clustered = lines.ClusterDBSCAN(distanceFunction);

            List<Polyline> result = new List<Polyline>();
            for (int i = 0; i < clustered.Count; i++)
            {
                var cluster = clustered[i];
                //TODO: move out to a dedicated method?
                //TODO: warning if any nodes removed?

                Dictionary<Point, int> nodesByValence = new Dictionary<Point, int>();
                foreach (Line l in cluster)
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
                            int index = cluster.FindIndex(x => x.Start == key || x.End == key);
                            Line l = cluster[index];
                            cluster.RemoveAt(index);

                            if (l.Start == key)
                                nodesByValence[l.End]--;
                            else
                                nodesByValence[l.Start]--;
                        }    
                    }

                    if (valenceOne.Count == 0)
                        break;

                    foreach(Point p in valenceOne)
                    {
                        nodesByValence.Remove(p);
                    }
                }                

                if (!nodesByValence.Keys.ToList().IsCoplanar(distanceTolerance))
                {
                    // warning
                    continue;
                }

                Vector dir1 = cluster[0].Direction();
                Vector dir2 = cluster.FirstOrDefault(x => x.Direction().IsParallel(dir1) == 0)?.Direction();
                if (dir2 == null)
                {
                    // warning
                    continue;
                }

                Vector normal = dir1.CrossProduct(dir2);
                Cartesian cs = Create.CartesianCoordinateSystem(cluster[0].Start, dir1, dir1.CrossProduct(normal));

                TransformMatrix toGlobal = cs.OrientationMatrix(new Cartesian());

                var transformed = cluster.Select(x => x.Transform(toGlobal)).ToList();


                Dictionary<Line, (Point, Point)> endpoints = transformed.ToDictionary(x => x, x => (x.Start, x.End));
                Dictionary<Line, Vector> dirs = transformed.ToDictionary(x => x, x => x.Direction());
                Dictionary<Point, List<Line>> graph = new Dictionary<Point, List<Line>>();
                //List<Point> nodes = new List<Point>();
                foreach (Line line in transformed)
                {
                    //if (!nodes.Contains(line.Start))
                    //    nodes.Add(line.Start);

                    //if (!nodes.Contains(line.End))
                    //    nodes.Add(line.End);

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
                        //TODO: can't skip here to avoid changing collection lengths!
                        //if (l == currentEdge)
                        //    continue;

                        Vector dir = l.Direction();
                        if (l.Start != currentNode)
                            dir = dir.Reverse();

                        nodeDirs.Add(dir);
                    }

                    Vector newDir = null;
                    Line newEdge = null;

                    // To avoid computing heavy signed angle, first check only dirs to the left from the current dir
                    // If not found, then look up the ones to the right
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

                outline = outline.Select(x => cluster[transformed.IndexOf(x)]).ToList();

                //TODO: WHY DOES OUTLINE CONTAIN LINES FROM OUTSIDE OF CLUSTER??????
                //var cluster2 = cluster.Select(x=>x.Transform)
                //List<Line> inner = cluster.Where(x => outline.All(y => !x.IsEqual(y))).ToList();
                List<Line> inner = cluster.Except(outline).ToList();

                //return null;
                //TODO: hardcoded decimal!
                result.AddRange(LinesToPolygons(outline, inner, distanceTolerance));
            }

            return result;
        }
    }
}
