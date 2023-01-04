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

using BH.Engine.Base;
using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.oM.Dimensional;
using BH.oM.Base.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

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
            if (graph == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the entity neighbourhood of a null graph.");
                return new List<Graph>();
            }

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
            if (graph == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the entity neighbourhood of a null graph.");
                return null;
            }

            if (entity == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot search from a null entity.");
                return null;
            }

            if (!graph.Entities.ContainsKey(entity.BHoM_Guid))
            {
                Engine.Base.Compute.RecordError("Graph does not contain provided entity.");
                return null;
            }

            Dictionary<Guid, List<Guid>> adjecency = graph.Adjacency(relationDirection);
            //Get depth map of all entities in relation to the current
            Dictionary<Guid, int> depths = adjecency.Depth(entity.BHoM_Guid);
            //Extract all entities with depth less than maximum
            Dictionary<Guid, IBHoMObject> entities = depths.Where(x => x.Value <= maximumDepth).ToDictionary(x => x.Key, x => graph.Entities[x.Key]);

            //Lookup based on outer key as source and inner key as target for relations
            ILookup<Guid, ILookup<Guid, IRelation>> sourceTargetLookup = graph.Relations.GroupBy(x => x.Source).ToDictionary(x => x.Key, x => x.ToLookup(y => y.Target)).ToLookup(x => x.Key, x => x.Value);

            List<IRelation> relations = new List<IRelation>();

            //Loop through all adjaciencies and add relations matching
            foreach (KeyValuePair<Guid, List<Guid>> kvp in adjecency)
            {
                int depth;
                if (depths.TryGetValue(kvp.Key, out depth) && depth < maximumDepth)
                {
                    foreach (Guid guid in kvp.Value)
                    {
                        //Extract all relations that match the adjaciency
                        relations.AddRange(sourceTargetLookup.Relations(kvp.Key, guid, relationDirection));
                    }
                }
            }

            //Return new graph
            return new Graph
            {
                Entities = entities,
                Relations = relations.GroupBy(x => x.BHoM_Guid).Select(x => x.First()).ToList() //Unique Relations
            };
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/


        private static IEnumerable<IRelation> Relations(this ILookup<Guid, ILookup<Guid, IRelation>> sourceTargetLookup, Guid from, Guid to, RelationDirection direction)
        {
             switch (direction)
            {
                case RelationDirection.Forwards:
                    return sourceTargetLookup[from].SelectMany(x => x[to]);
                case RelationDirection.Backwards:
                    return sourceTargetLookup[to].SelectMany(x => x[from]);
                case RelationDirection.Both:
                default:
                    IEnumerable<IRelation> relations = sourceTargetLookup[from].SelectMany(x => x[to]);
                    return relations.Concat(sourceTargetLookup[to].SelectMany(x => x[from]));
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


    }
}


