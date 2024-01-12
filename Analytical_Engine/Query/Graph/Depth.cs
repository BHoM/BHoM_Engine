/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using BH.oM.Analytical.Graph;
using BH.oM.Geometry;
using BH.oM.Base.Attributes;
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

        [Description("Returns the Dictionary of entity depths in a Graph given a start entity. The query uses breadth first search, each key value pair in the resulting dictionary is in the form <entity, depth>.")]
        [Input("adjacency", "The adjacency dictionary of the Graph to extract the depth dictionary from.")]
        [Input("startEntity", "The Guid of the entity from which the depth dictionary is created.")]
        [Output("depths", "A Dictionary of the depths of the entities in the Graph.")]
        public static Dictionary<Guid, int> Depth(this Dictionary<Guid, List<Guid>> adjacency, Guid startEntity)
        {
            //https://www.geeksforgeeks.org/level-node-tree-source-node-using-bfs/
            // dictionary to store level of each entity  
            Dictionary<Guid, int> level = new Dictionary<Guid, int>();
            if (!adjacency.ContainsKey(startEntity))
            {
                Base.Compute.RecordError("startEntity provided cannot be found in the adjacency dictionary. Ensure the entity exists in the original graph.");
                return level;
            }
            // dictionary to store when entity has been visited
            Dictionary<Guid, bool> marked = new Dictionary<Guid, bool>();
            // create a queue  
            Queue<Guid> que = new Queue<Guid>();
            // enqueue element x  
            que.Enqueue(startEntity);
            // initialize level of source entity to 0  
            level[startEntity] = 0;
            // marked it as visited  
            marked[startEntity] = true;
            // do until queue is empty  
            while (que.Count > 0)
            {
                // dequeue element  
                startEntity = que.Dequeue();
                // traverse neighbours of startEntity 
                foreach (Guid b in adjacency[startEntity])
                {
                    // b is neighbour of startEntity 
                    // if b is not marked already  
                    if (!marked.ContainsKey(b))
                    {
                        // enqueue b in queue  
                        que.Enqueue(b);
                        // level of b is level of x + 1  
                        level[b] = level[startEntity] + 1;
                        // mark b  
                        marked[b] = true;
                    }
                }
            }
            return level;
        }

        /***************************************************/

        [Description("Returns the Dictionary of entity depths in a Graph given a start entity. The query uses breadth first search, each key value pair in the resulting dictionary is in the form <entity, depth>.")]
        [Input("graph", "The graph to extract the depth dictionary from.")]
        [Input("startEntity", "The Guid of the entity from which the depth dictionary is created.")]
        [Input("relationDirection", "Optional RelationDirection used to determine the direction that relations can be traversed. Defaults to Forwards indicating traversal is from source to target.")]
        [Output("depths", "A Dictionary of the depths of the entities in the Graph.")]
        public static Dictionary<Guid, int> Depth(this Graph graph, Guid startEntity, RelationDirection relationDirection = RelationDirection.Forwards)
        {
            if(graph == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the depth of a null graph.");
                return new Dictionary<Guid, int>();
            }

            return graph.Adjacency(relationDirection).Depth(startEntity);
        }

        /***************************************************/
    }
}



