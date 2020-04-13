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

using System.Collections.Generic;
using BH.oM.Data.Collections;
using System.Linq;
using System.ComponentModel;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/
        [Description("Extracts a list of leaf nodes from a graph, leaf nodes have no neighbours deeper in the graph")]
        [Input("graph", "The graph to extract the leaf nodes from")]
        public static List<GraphNode<T>> LeafNodes<T>(this Graph<T> graph, GraphNode<T> startNode)
        {
            Dictionary<GraphNode<T>, int> depthDictionary = graph.DepthDictionary(startNode);
            List<GraphNode<T>> leafnodes = new List<GraphNode<T>>();
            //get the adjacency dict
            Dictionary<GraphNode<T>, List<GraphNode<T>>> adjacency = graph.AdjacencyDictionary();
            if (!adjacency.ContainsKey(startNode))
            {
                Reflection.Compute.RecordError("startNode provided cannot be found in the adjacency dictionary. Ensure the node exists in the original graph");
                return leafnodes;
            }
            // dictionary to store when node has been visited
            Dictionary<GraphNode<T>, bool> marked = new Dictionary<GraphNode<T>, bool>();
            // create a queue  
            Queue<GraphNode<T>> que = new Queue<GraphNode<T>>();
            // enqueue element x  
            que.Enqueue(startNode);
            // marked it as visited  
            marked[startNode] = true;
            // do until queue is empty  
            while (que.Count > 0)
            {
                // dequeue element  
                startNode = que.Dequeue();
                // traverse neighbours of node x  
                int totalMarked = 0;
                foreach (GraphNode<T> b in adjacency[startNode])
                {
                    // b is neighbor of node x  
                    // if b is not marked already  
                    if (!marked.ContainsKey(b))
                    {
                        // enqueue b in queue  
                        que.Enqueue(b);
                        // mark b  
                        marked[b] = true;
                    }
                    else
                        totalMarked++;
                }
                if (totalMarked == adjacency[startNode].Count)
                    leafnodes.Add(startNode);
            }
            
            return leafnodes;
        }
    }
}
