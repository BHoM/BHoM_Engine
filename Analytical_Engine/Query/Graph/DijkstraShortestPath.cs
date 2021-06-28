/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
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
        [PreviousVersion("4.3", "BH.Engine.Analytical.Query.DijkstraShortestPath(BH.oM.Analytical.Elements.Graph, BH.oM.Base.IBHoMObject, BH.oM.Base.IBHoMObject>")]
        public static ShortestPathResult<T> DijkstraShortestPath<T>(this Graph<T> graph, T start, T end)
            where T : IBHoMObject
        {
            if (graph == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the Dijkstra shortest path from a null graph.");
                return null;
            }

            if (start == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the Dijkstra shortest path between two points when the start is null.");
                return null;
            }

            if (end == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the Dijkstra shortest path between two points when the end is null.");
                return null;
            }

            return DijkstraShortestPath(graph, start.BHoM_Guid, end.BHoM_Guid);
        }

        /***************************************************/

        [Description("Returns the Dijkstra shortest path for a graph.")]
        [Input("graph", "The Graph to query for the shortest path.")]
        [Input("start", "The Guid entity used for the start of the path.")]
        [Input("end", "The Guid entity used for the end of the path.")]
        [Output("shortest path result", "The ShortestPathResult.")]
        [PreviousVersion("4.3", "BH.Engine.Analytical.Query.DijkstraShortestPath(BH.oM.Analytical.Elements.Graph, System.Guid, System.Guid>")]
        public static ShortestPathResult<T> DijkstraShortestPath<T>(this Graph<T> graph, Guid start, Guid end)
            where T : IBHoMObject
        {
            if (graph == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the Dijkstra shortest path from a null graph.");
                return null;
            }

            SetFragments(graph);
            
            DijkstraSearch(graph, start, end);

            List<Guid> shortestPath = new List<Guid>();
            shortestPath.Add(end);

            double length = 0;
            double cost = 0;
            List<IRelation<T>> relations = new List<IRelation<T>>();

            graph.DijkstraResult(shortestPath, end,ref length, ref cost, ref relations);
            shortestPath.Reverse();
            List<T> objPath = new List<T>();
            shortestPath.ForEach(g => objPath.Add(graph.Entities[g]));

            List<T> entitiesVisited = m_Fragments.Where(kvp => kvp.Value.Visited).Select(kvp => graph.Entities[kvp.Key]).ToList();
            ShortestPathResult<T> result = new ShortestPathResult<T>(graph.BHoM_Guid, "DijkstraShortestPath", -1, objPath, length, cost, entitiesVisited, relations);
            return result;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static void SetFragments<T>(Graph<T> graph)
            where T : IBHoMObject
        {
            m_Fragments = new Dictionary<Guid, RoutingFragment>();
            foreach (Guid n in graph.Entities.Keys.ToList())
            {
                m_Fragments[n] = new RoutingFragment();
            }
        }

        /***************************************************/
        private static void DijkstraSearch<T>(Graph<T> graph, Guid start, Guid end)
            where T : IBHoMObject
        {
            m_Fragments[start].MinCostToSource = 0;
            var prioQueue = new List<Guid>();
            prioQueue.Add(start);
            do
            {
                prioQueue = prioQueue.OrderBy(x => m_Fragments[x].MinCostToSource).ToList();
                Guid currentEntity = prioQueue.First();
                prioQueue.Remove(currentEntity);
                List<IRelation<T>> relations = graph.Relations.FindAll(link => link.Source.Equals(currentEntity));
                
                //use weight to define cost to end
                foreach (IRelation<T> r in relations)
                    m_Fragments[r.Target].Cost = r.Weight;

                List<Guid> connections = relations.Select(link => link.Target).ToList();

                foreach (Guid childEntity in connections.OrderBy(x => m_Fragments[x].Cost))
                {
                    
                    if (m_Fragments[childEntity].Visited)
                        continue;
                    //if min cost to start is null or cost of this entity is less than child entity cost
                    if (!m_Fragments[childEntity].MinCostToSource.HasValue ||
                        m_Fragments[currentEntity].MinCostToSource + m_Fragments[childEntity].Cost < m_Fragments[childEntity].MinCostToSource)
                    {
                        //set cost
                        m_Fragments[childEntity].MinCostToSource = m_Fragments[currentEntity].MinCostToSource + m_Fragments[childEntity].Cost;
                        //set nearest currentEntity to start
                        m_Fragments[childEntity].NearestToSource = currentEntity;
                        //add to queue 
                        if (!prioQueue.Contains(childEntity))
                            prioQueue.Add(childEntity);
                    }
                }
                m_Fragments[currentEntity].Visited = true;
                if (currentEntity.Equals(end))
                    return;
            } while (prioQueue.Any());
        }

        /***************************************************/
        private static void DijkstraResult<T>(this Graph<T> graph, List<Guid> list, Guid currentEntity, ref double length, ref double cost, ref List<IRelation<T>> relations)
            where T : IBHoMObject
        {
            if (m_Fragments[currentEntity].NearestToSource == Guid.Empty)
                return;
            Guid n = m_Fragments[currentEntity].NearestToSource;
            list.Add(n);
            
            //assuming Dijkstra use is non-spatial increment by 1
            length += 1;

            //relations linking entities working backwards from end
            List<IRelation<T>> links = graph.Relation(graph.Entities[n], graph.Entities[currentEntity]).ToList();
            
            //hang on to all if multiple exist
            relations.AddRange(links);

            if (m_Fragments[n].Cost.HasValue)
                cost += m_Fragments[n].Cost.Value;

            graph.DijkstraResult(list, n, ref length, ref cost, ref relations);
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<Guid, RoutingFragment> m_Fragments = new Dictionary<Guid, RoutingFragment>();

    }
}

