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
        public static Dictionary<GraphNode<T>, int> DepthDictionary<T>(Dictionary<GraphNode<T>, List<GraphNode<T>>> adjacency, GraphNode<T> x)
        {//https://www.geeksforgeeks.org/level-node-tree-source-node-using-bfs/
         // dictionary to store level of each node  
            Dictionary<GraphNode<T>, int> level = new Dictionary<GraphNode<T>, int>();
            Dictionary<GraphNode<T>, bool> marked = new Dictionary<GraphNode<T>, bool>();
            // create a queue  
            Queue<GraphNode<T>> que = new Queue<GraphNode<T>>();
            // enqueue element x  
            que.Enqueue(x);
            // initialize level of source node to 0  
            level[x] = 0;
            // marked it as visited  
            marked[x] = true;
            // do until queue is empty  
            while (que.Count > 0)
            {
                // dequeue element  
                x = que.Dequeue();
                // traverse neighbors of node x  
                foreach (GraphNode<T> b in adjacency[x])
                {
                    // b is neighbor of node x  
                    // if b is not marked already  
                    if (!marked.ContainsKey(b))
                    {
                        // enqueue b in queue  
                        que.Enqueue(b);
                        // level of b is level of x + 1  
                        level[b] = level[x] + 1;
                        // mark b  
                        marked[b] = true;
                    }
                }
            }

            return level;
        }
    }
}