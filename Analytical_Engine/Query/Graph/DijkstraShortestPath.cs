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
using BH.oM.Analytical;
using BH.oM.Analytical.Elements;
using BH.oM.Analytical.Fragments;
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

        [Description("Returns the Dijkstra shortest path for a graph.")]   
        [Input("graph", "The Graph to query for the shortest path.")]
        [Input("start", "The IBHoMObject entity used for the start of the path.")]
        [Input("end", "The IBHoMObject entity used for the end of the path.")]
        [Output("shortest path result", "The ShortestPathResult.")]

        public static ShortestPathResult DijkstraShortestPath(Graph graph, IBHoMObject start, IBHoMObject end)
        {
            ShortestPathResult result = DijkstraShortestPath(graph, start.BHoM_Guid, end.BHoM_Guid);
            return result;
        }

        /***************************************************/

        [Description("Returns the Dijkstra shortest path for a graph.")]
        [Input("graph", "The Graph to query for the shortest path.")]
        [Input("start", "The Guid entity used for the start of the path.")]
        [Input("end", "The Guid entity used for the end of the path.")]
        [Output("shortest path result", "The ShortestPathResult.")]

        public static ShortestPathResult DijkstraShortestPath(Graph graph, Guid start, Guid end)
        {
            SetFragments(graph);
            
            DijkstraSearch(graph, start, end);

            List<Guid> shortestPath = new List<Guid>();
            shortestPath.Add(end);

            double length = 0;
            double cost = 0;

            DijkstraResult(shortestPath, end,ref length, ref cost);
            shortestPath.Reverse();
            List<IBHoMObject> objPath = new List<IBHoMObject>();
            shortestPath.ForEach(g => objPath.Add(graph.Entities[g]));

            List<IBHoMObject> nodesVisited = m_Fragments.Where(kvp => kvp.Value.Visited).Select(kvp => graph.Entities[kvp.Key]).ToList();
            ShortestPathResult result = new ShortestPathResult(graph.BHoM_Guid, "DijkstraShortestPath", -1, objPath, length, cost, nodesVisited);
            return result;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static void SetFragments(Graph graph)
        {
            m_Fragments = new Dictionary<Guid, RoutingFragment>();
            foreach (Guid n in graph.Entities.Keys.ToList())
            {
                m_Fragments[n] = new RoutingFragment();
            }
        }
        /***************************************************/
        private static void DijkstraSearch(Graph graph, Guid start, Guid end)
        {
            m_Fragments[start].MinCostToSource = 0;
            var prioQueue = new List<Guid>();
            prioQueue.Add(start);
            do
            {
                prioQueue = prioQueue.OrderBy(x => m_Fragments[x].MinCostToSource).ToList();
                Guid node = prioQueue.First();
                prioQueue.Remove(node);
                List<IRelation> relations = graph.Relations.FindAll(link => link.Source.Equals(node));
                
                //use weight to define cost to end
                foreach (IRelation r in relations)
                    m_Fragments[r.Target].Cost = r.Weight;

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
        private static void DijkstraResult(List<Guid> list, Guid node, ref double length, ref double cost)
        {
            if (m_Fragments[node].NearestToSource == Guid.Empty)
                return;
            Guid n = m_Fragments[node].NearestToSource;
            list.Add(n);
            //assuming Dijkstra use is non-spatial
            length += 1;

            if(m_Fragments[n].Cost.HasValue)
                cost += m_Fragments[n].Cost.Value;

            DijkstraResult(list, n,ref length, ref cost);
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/
        private static Dictionary<Guid, RoutingFragment> m_Fragments = new Dictionary<Guid, RoutingFragment>();
    }
}
