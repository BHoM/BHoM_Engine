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

using BH.oM.Data.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        public static Dictionary<GraphNode<T>, GraphNode<T>> DepthFirstTraversal<T>(GraphNode<T> start, Graph<T> graph)
        {
            //Depth first graph traversal to get all Node-Neighbour predecessor set 
            //https://stackoverflow.com/questions/615202/c-sharp-graph-traversal

            Dictionary<GraphNode<T>, List<GraphNode<T>>> adjacency = graph.AdjacencyDictionary();
            Dictionary<GraphNode<T>, bool> visited = new Dictionary<GraphNode<T>, bool>();
            //predecessors is in the form <nodeLevel_n-1,nodeLevel_n>
            Dictionary<GraphNode<T>, GraphNode<T>> predecessors = new Dictionary<GraphNode<T>, GraphNode<T>>();
            //first in first out queue
            Queue<GraphNode<T>> worklist = new Queue<GraphNode<T>>();

            visited.Add(start, false);

            worklist.Enqueue(start);
            //root node is double tagged
            predecessors.Add(start, start);
            while (worklist.Count != 0)
            {
                //get the next in the queue
                GraphNode<T> node = worklist.Dequeue();
                //get the neighbours 
                List<GraphNode<T>> neighbours = adjacency[node];
                foreach (GraphNode<T> neighbour in neighbours)
                {
                    //if we haven't visited?
                    if (!visited.ContainsKey(neighbour))
                    {
                        visited.Add(neighbour, false);
                        predecessors.Add(neighbour, node);
                        worklist.Enqueue(neighbour);
                    }
                }
            }
            return predecessors;
        }
    }
}