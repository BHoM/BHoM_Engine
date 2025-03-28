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

using BH.oM.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Data
{
    public static partial class Compute
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<GraphNode<T>> Path<T>(this Graph<T> graph, GraphNode<T> startNode, GraphNode<T> endNode, Func<T, T, double> costHeuristic)
        {
            // Create the inital close set
            HashSet<GraphNode<T>> closedSet = new HashSet<GraphNode<T>>();

            // Create initial set of boundary nodes and add start node to it
            SortedSet<PathItem<T>> openSet = new SortedSet<PathItem<T>>();
            openSet.Add(new PathItem<T>(startNode, 0, costHeuristic(startNode.Value, endNode.Value)));

            int iter = 0;
            while (openSet.Count > 0)
            {
                PathItem<T> currentItem = openSet.First();
                if (currentItem.Node == endNode || iter++ > 1000)
                    return ReconstructPath(currentItem);

                openSet.Remove(currentItem);
                closedSet.Add(currentItem.Node);

                foreach (GraphLink<T> link in graph.Links.Where(x => x.StartNode == currentItem.Node))
                {
                    GraphNode<T> neighbour = link.EndNode;
                    if (closedSet.Contains(neighbour))
                        continue;

                    double tentativeCost = currentItem.StartCost + link.Weight;
                    PathItem<T> item = openSet.FirstOrDefault(x => x.Node == neighbour);
                    if (item == null)
                    {
                        PathItem<T> newItem = new PathItem<T>(neighbour, tentativeCost, costHeuristic(neighbour.Value, endNode.Value));
                        newItem.Previous = currentItem;
                        openSet.Add(newItem);
                    }
                    else if (tentativeCost < item.StartCost)
                    {
                        openSet.Remove(item);
                        item.Previous = currentItem;
                        item.StartCost = tentativeCost;
                        openSet.Add(item);
                    }
                }
            }

            return null;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<GraphNode<T>> ReconstructPath<T>(PathItem<T> endItem)
        {
            List<GraphNode<T>> path = new List<GraphNode<T>>();
            path.Add(endItem.Node);

            while (endItem.Previous != null)
            {
                endItem = endItem.Previous;
                path.Add(endItem.Node);
            }
            path.Reverse();

            return path;
        }

        /***************************************************/
    }
}






