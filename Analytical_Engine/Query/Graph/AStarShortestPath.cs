/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2020, the respective contributors. All rights reserved.
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

using BH.Engine.Geometry;
using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        public static ShortestPathResult AStarShortestPath(Graph graph, IBHoMObject start, IBHoMObject end)
        {
            ShortestPathResult result = AStarShortestPath(graph, start.BHoM_Guid, end.BHoM_Guid);
            
            return result;
        }
        /***************************************************/
        public static ShortestPathResult AStarShortestPath(Graph graph, Guid start, Guid end)
        {
            MakeNodesSpatial(graph);
            if (m_SpatialNodes.Count!= graph.Entities.Count)
            {
                Reflection.Compute.RecordWarning("One or more of the provided graph entities do not implement BH.oM.Analytical.INode.\n" +
                    "All vertices must implement INode to provide required spatial location to use AStar shortest path.\n" +
                    "Shortest path is computed using Dijkstra shortest path instead.");

                return DijkstraShortestPath(graph, start, end);
            }
                
            SetFragments(graph);
            INode endNode = m_SpatialNodes[end];
            foreach (Guid node in graph.Entities.Keys.ToList())
            {
                INode nodeA = m_SpatialNodes[node];
                m_Fragments[node].StraightLineDistanceToTarget = nodeA.Position.Distance(endNode.Position);
            }
                
            AStarSearch(graph, start, end);

            List<Guid> shortestPath = new List<Guid>();
            shortestPath.Add(end);

            double length = 0;
            double cost = 0;
            AStarResult(shortestPath, end,ref length, ref cost);
            shortestPath.Reverse();

            List<IBHoMObject> objPath = new List<IBHoMObject>();
            shortestPath.ForEach(g => objPath.Add(graph.Entities[g]));

            List<IBHoMObject> nodesVisited = m_Fragments.Where(kvp => kvp.Value.Visited).Select(kvp => graph.Entities[kvp.Key]).ToList();
            ShortestPathResult result = new ShortestPathResult(graph.BHoM_Guid, "AStarShortestPath", -1, objPath, length, cost, nodesVisited);
            return result;
        }
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static void AStarSearch(Graph graph, Guid start, Guid end)
        {
            m_Fragments[start].MinCostToSource = 0;
            List <Guid> prioQueue = new List<Guid>();
            prioQueue.Add(start);
            do
            {
                prioQueue = prioQueue.OrderBy(x => m_Fragments[x].MinCostToSource + m_Fragments[x].StraightLineDistanceToTarget).ToList();
                Guid node = prioQueue.First();
                prioQueue.Remove(node);
                List<IRelation> relations = graph.Relations.FindAll(link => link.Source.Equals(node));
                
                //use weight AND distance to define cost to end
                foreach (IRelation r in relations)
                    m_Fragments[r.Target].Cost = m_SpatialNodes[r.Target].Position.Distance(m_SpatialNodes[node].Position) * r.Weight;

                List<Guid> connections = relations.Select(link => link.Target).ToList();

                foreach (Guid childNode in connections.OrderBy(x => m_Fragments[x].Cost))
                {

                    if (m_Fragments[childNode].Visited)
                        continue;
                    //if min cost to start is null or cost of this node is less than child node cost
                    if (!m_Fragments[childNode].MinCostToSource.HasValue ||
                        m_Fragments[node].MinCostToSource + m_Fragments[childNode].Cost < m_Fragments[childNode].MinCostToSource)
                    {
                        //set cost
                        m_Fragments[childNode].MinCostToSource = m_Fragments[node].MinCostToSource + m_Fragments[childNode].Cost;
                        //set nearest node to start
                        m_Fragments[childNode].NearestToSource = node;
                        //add to queue 
                        if (!prioQueue.Contains(childNode))
                            prioQueue.Add(childNode);
                    }
                }
                m_Fragments[node].Visited = true;
                if (node.Equals(end))
                    return;
            } while (prioQueue.Any());
        }
        /***************************************************/
        private static void MakeNodesSpatial(Graph graph)
        {
            m_SpatialNodes = new Dictionary<Guid, INode>();
            foreach (KeyValuePair<Guid, IBHoMObject> kvp in graph.Entities)
            {
                if (kvp.Value is INode)
                    m_SpatialNodes.Add(kvp.Key, (INode)kvp.Value);
            }
        }
        /***************************************************/
        private static void AStarResult(List<Guid> list, Guid node, ref double length, ref double cost)
        {
            if (m_Fragments[node].NearestToSource == Guid.Empty)
                return;
            Guid n = m_Fragments[node].NearestToSource;
            list.Add(n);
            length += m_SpatialNodes[node].Position.Distance(m_SpatialNodes[n].Position);

            if (m_Fragments[n].Cost.HasValue)
                cost += m_Fragments[n].Cost.Value;
            AStarResult(list, n, ref length, ref cost);
        }
        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/
        private static Dictionary<Guid, INode> m_SpatialNodes = new Dictionary<Guid, INode>();

    }
}
