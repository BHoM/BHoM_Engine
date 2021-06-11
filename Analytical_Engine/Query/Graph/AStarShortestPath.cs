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
using BH.Engine.Spatial;
using BH.oM.Analytical.Elements;
using BH.oM.Analytical.Fragments;
using BH.oM.Base;
using BH.oM.Dimensional;
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

        [Description("Returns the a star shortest path for a Graph. \n" +
            "If the supplied Graph does not contain entities that implement IElement0D or and relations that are SpatialRelations the shortest path is computed using the Dijkstra shortest path.")]
        [Input("graph", "The Graph to query for the shortest path.")]
        [Input("start", "The IBHoMObject entity used for the start of the path.")]
        [Input("end", "The IBHoMObject entity used for the end of the path.")]
        [Output("shortest path result", "The ShortestPathResult.")]
        public static ShortestPathResult<T> AStarShortestPath<T>(this Graph<T> graph, T start, T end)
            where T : IBHoMObject
        {
            if(graph == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the AStar shortest path from a null graph.");
                return null;
            }

            if (start == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the AStar shortest path between two points when the start is null.");
                return null;
            }

            if (end == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the AStar shortest path between two points when the end is null.");
                return null;
            }

            return AStarShortestPath(graph, start.BHoM_Guid, end.BHoM_Guid);
        }

        /***************************************************/

        [Description("Returns the a star shortest path for a Graph. \n" +
            "If the supplied Graph does not contain entities that implement IElement0D or and spatial relations the shortest path is computed using the Dijkstra shortest path.")]
        [Input("graph", "The Graph to query for the shortest path.")]
        [Input("start", "The Guid entity used for the start of the path.")]
        [Input("end", "The Guid entity used for the end of the path.")]
        [Output("shortest path result", "The ShortestPathResult.")]
        public static ShortestPathResult<T> AStarShortestPath<T>(this Graph<T> graph, Guid start, Guid end)
            where T : IBHoMObject
        {
            if (graph == null)
            {
                BH.Engine.Reflection.Compute.RecordError("Cannot query the AStar shortest path from a null graph.");
                return null;
            }

            Graph<T> geometricGraph = graph.IProjectGraph(new GeometricProjection());

            if (geometricGraph.Entities.Count == 0 || geometricGraph.Relations.Count == 0)
            {
                Reflection.Compute.RecordWarning("The graph provided does not contain sufficient spatial entities or relations.\n" +
                    "To use a star shortest path provide a graph where some entities implement IElement0D and spatial relations are defined between them.\n" +
                    "Shortest path is computed using Dijkstra shortest path instead.");

                return DijkstraShortestPath(graph, start, end);
            }
                
            SetFragments(geometricGraph);

            //calculate straight line distance from each entity to the end
            IElement0D endEntity = geometricGraph.Entities[end] as IElement0D;
            foreach (Guid entity in geometricGraph.Entities.Keys.ToList())
            {
                IElement0D element0D = geometricGraph.Entities[entity] as IElement0D;
                m_Fragments[entity].StraightLineDistanceToTarget = element0D.IGeometry().Distance(endEntity.IGeometry());
            }
                
            AStarSearch(geometricGraph, start, ref end);

            List<Guid> shortestPath = new List<Guid>();
            shortestPath.Add(end);

            double length = 0;
            double cost = 0;
            List<ICurve> curves = new List<ICurve>();
            List<IRelation<T>> relations = new List<IRelation<T>>();
            graph.AStarResult(shortestPath, end,ref length, ref cost, ref curves, ref relations);
            shortestPath.Reverse();

            List<T> objPath = new List<T>();
            shortestPath.ForEach(g => objPath.Add(geometricGraph.Entities[g]));

            List<T> entitiesVisited = m_Fragments.Where(kvp => kvp.Value.Visited).Select(kvp => geometricGraph.Entities[kvp.Key]).ToList();
            ShortestPathResult<T> result = new ShortestPathResult<T>(graph.BHoM_Guid, "AStarShortestPath", -1, objPath, length, cost, entitiesVisited, relations, curves);
            return result;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void AStarSearch<T>(this Graph<T> graph, Guid start,ref Guid end)
            where T : IBHoMObject
        {
            m_Fragments[start].MinCostToSource = 0;
            List <Guid> prioQueue = new List<Guid>();
            prioQueue.Add(start);
            do
            {
                prioQueue = prioQueue.OrderBy(x => m_Fragments[x].MinCostToSource + m_Fragments[x].StraightLineDistanceToTarget).ToList();
                Guid currentEntity = prioQueue.First();
                prioQueue.Remove(currentEntity);
                List<IRelation<T>> relations = graph.Relations.FindAll(link => link.Source.Equals(currentEntity));
                T current = graph.Entities[currentEntity];
                //use weight AND length of the relation to define cost to end
                foreach (IRelation<T> r in relations)
                {
                    
                    double length = graph.RelationLength(r);
                    m_Fragments[r.Target].Cost = length * r.Weight;
                }
                    
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
                        //set nearest entity to start
                        m_Fragments[childEntity].NearestToSource = currentEntity;
                        //add to queue 
                        if (!prioQueue.Contains(childEntity))
                            prioQueue.Add(childEntity);
                    }
                }
                m_Fragments[currentEntity].Visited = true;
                if (currentEntity.Equals(end))
                    return;
            } 
            while (prioQueue.Any());

            //if we reach here the search failed to find the end
            end = FindClosestEnd();
        }

        /***************************************************/

        private static void AStarResult<T>(this Graph<T> graph, List<Guid> list, Guid entity, ref double length, ref double cost, ref List<ICurve> curves, ref List<IRelation<T>> relations)
            where T : IBHoMObject
        {
            if (m_Fragments[entity].NearestToSource == Guid.Empty)
                return;
            Guid n = m_Fragments[entity].NearestToSource;
            list.Add(n);

            //relations linking entities working backwards from end
            List<IRelation<T>> links = graph.Relation(graph.Entities[n], graph.Entities[entity]).ToList();

            //order by length and only use the shortest.
            links = links.OrderBy(sr => graph.RelationLength(sr)).ToList();

            length += graph.RelationLength(links[0]);

            relations.Add(links[0]);

            curves.Add(links[0].Curve);

            if (m_Fragments[n].Cost.HasValue)
                cost += m_Fragments[n].Cost.Value;

            graph.AStarResult(list, n, ref length, ref cost, ref curves, ref relations);
        }

        /***************************************************/

        private static Guid FindClosestEnd()
        {
            //finds the closest accessible entity to the end when the search fails
            Reflection.Compute.RecordWarning("Shortest path to target could not be computed. The shortest path to the accessible entity closest to the target has been computed.\n" +
                " Check the target entity is connected to the Graph through links that are all traversable from the start.");

            double minDist = double.MaxValue;
            Guid nearestEntity = Guid.Empty;
            foreach(KeyValuePair<Guid, RoutingFragment> kvp in m_Fragments)
            {
                if (kvp.Value.Visited)
                {
                    if(kvp.Value.StraightLineDistanceToTarget < minDist)
                    {
                        minDist = (double)kvp.Value.StraightLineDistanceToTarget;
                        nearestEntity = kvp.Key;
                    }
                }
            }

            return nearestEntity;
        }

    }
}

