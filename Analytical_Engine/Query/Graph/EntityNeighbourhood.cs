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

using BH.Engine.Base;
using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.oM.Dimensional;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [Description("Find all sub Graphs around all entities at maximum depth 1 within a Graph.")]
        [Input("graph", "The Graph to search.")]
        [Input("relationDirection", "Optional RelationDirection used to determine the direction that relations can be traversed. Defaults to Forward indicating traversal is from source to target.")]
        [Output("graphs", "The collection of sub Graphs found in the input Graph.")]

        public static List<Graph> EntityNeighbourhood(this Graph graph, RelationDirection relationDirection = RelationDirection.Forwards)
        {
            List<Graph> subGraphs = new List<Graph>();
            m_Adjacency = graph.Adjacency(relationDirection);

            foreach (KeyValuePair<Guid, List<Guid>> kvp in m_Adjacency)
                subGraphs.Add(graph.SetSubGraph(kvp.Key, kvp.Value, relationDirection));
            
            return subGraphs;
        }
        /***************************************************/

        [Description("Find the sub Graph around an entity at a specified depth within a Graph.")]
        [Input("graph", "The Graph to search.")]
        [Input("entity", "The IBHoMObject entity to search from.")]
        [Input("maximumDepth", "The maximum traversal depth from the given entity.")]
        [Input("relationDirection", "Optional RelationDirection used to determine the direction that relations can be traversed. Defaults to Forward indicating traversal is from source to target.")]
        [Output("graph", "The sub Graph found in the input Graph.")]

        public static Graph EntityNeighbourhood(this Graph graph, IBHoMObject entity, int maximumDepth, RelationDirection relationDirection = RelationDirection.Forwards)
        {
            List<Graph> subGraphs = new List<Graph>();
            m_Adjacency = graph.Adjacency(relationDirection);
            m_AccessibleEntities = new List<Guid>();
            m_AccessibleRelations = new List<Guid>();

            //add start entity
            m_AccessibleEntities.Add(entity.BHoM_Guid);

            graph.Traverse(entity.BHoM_Guid, maximumDepth, 0, relationDirection);

            Graph subgraph = new Graph();
            foreach(Guid guid in m_AccessibleEntities)
            {
                if (!subgraph.Entities.ContainsKey(guid))
                    subgraph.Entities.Add(guid, graph.Entities[guid].DeepClone());
            }

            foreach (Guid guid in m_AccessibleRelations)
            {
                if (!subgraph.Relations.Any(r => r.BHoM_Guid.Equals(guid)))
                    subgraph.Relations.Add(graph.Relations.Find(r => r.BHoM_Guid.Equals(guid)));
            }
            
            return subgraph;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void Traverse(this Graph graph, Guid entity, int maxDepth, int currentDepth, RelationDirection relationDirection)
        {
            if (currentDepth >= maxDepth)
                return;
            foreach (Guid c in m_Adjacency[entity])
            {
                m_AccessibleEntities.Add(c);
                m_AccessibleRelations.AddRange(graph.Relation(entity, c, relationDirection));
                graph.Traverse(c, maxDepth, currentDepth + 1, relationDirection);
            }
        }
        /***************************************************/
        private static Graph SetSubGraph(this Graph graph, Guid sourceEntity, List<Guid> entityAdjacency, RelationDirection relationDirection)
        {

            Graph subgraph = new Graph();
            //add start entity
            subgraph.Entities.Add(sourceEntity, graph.Entities[sourceEntity].DeepClone());

            entityAdjacency.ForEach(ent => subgraph.Entities.Add(ent, graph.Entities[ent].DeepClone()));

            entityAdjacency.ForEach(ent => subgraph.Relations.AddRange(graph.Relation(graph.Entities[sourceEntity], graph.Entities[ent], relationDirection)));
           
            return subgraph;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static List<Guid> m_AccessibleEntities = new List<Guid>();
        private static List<Guid> m_AccessibleRelations = new List<Guid>();

    }
}
