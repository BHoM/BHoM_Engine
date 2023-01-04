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

using BH.oM.Data.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using BH.oM.Base.Attributes;
using System.Linq;
using System;

namespace BH.Engine.Data
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/
        [Description("Gets the depth dictionary of a graph using breadth first search, each key value pair in the resulting dictionary is in the form <graph node, depth>")]
        [Input("adjacency", "The adjacency dictionary to extract the depth dictionary from")]
        [Input("startNode", "The graph node from which the depth dictionary is created")]
        public static Dictionary<GraphNode<T>, int> DepthDictionary<T>(Dictionary<GraphNode<T>, List<GraphNode<T>>> adjacency, GraphNode<T> startNode)
        {
            //https://www.geeksforgeeks.org/level-node-tree-source-node-using-bfs/
            // dictionary to store level of each node  
            Dictionary<GraphNode<T>, int> level = new Dictionary<GraphNode<T>, int>();
            if (!adjacency.ContainsKey(startNode))
            {
                Base.Compute.RecordError("startNode provided cannot be found in the adjacency dictionary. Ensure the node exists in the original graph");
                return level;
            }   
            // dictionary to store when node has been visited
            Dictionary<GraphNode<T>, bool> marked = new Dictionary<GraphNode<T>, bool>();
            // create a queue  
            Queue<GraphNode<T>> que = new Queue<GraphNode<T>>();
            // enqueue element x  
            que.Enqueue(startNode);
            // initialize level of source node to 0  
            level[startNode] = 0;
            // marked it as visited  
            marked[startNode] = true;
            // do until queue is empty  
            while (que.Count > 0)
            {
                // dequeue element  
                startNode = que.Dequeue();
                // traverse neighbors of node x  
                foreach (GraphNode<T> b in adjacency[startNode])
                {
                    // b is neighbor of node x  
                    // if b is not marked already  
                    if (!marked.ContainsKey(b))
                    {
                        // enqueue b in queue  
                        que.Enqueue(b);
                        // level of b is level of x + 1  
                        level[b] = level[startNode] + 1;
                        // mark b  
                        marked[b] = true;
                    }
                }
            }
            return level;
        }
        /***************************************************/
        [Description("Gets the depth dictionary of a graph using breadth first search, each key value pair in the resulting dictionary is in the form <graph node, depth>")]
        [Input("graph", "The graph to extract the depth dictionary from")]
        [Input("startNode", "The graph node from which the depth dictionary is created")]
        public static Dictionary<GraphNode<T>, int> DepthDictionary<T>(this Graph<T> graph, GraphNode<T> startNode)
        {
            Dictionary<GraphNode<T>, List<GraphNode<T>>> adjacency = graph.AdjacencyDictionary();
            return DepthDictionary<T>(adjacency, startNode);
        }
    }
}


