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

using BH.oM.Analytical.Elements;
using BH.oM.Base.Attributes;
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

        [Description("Find all disconnected sub Graphs within a Graph.")]
        [Input("graph", "The Graph to search.")]
        [Input("relationDirection", "The RelationDirection used to determine the direction that relations can be traversed. Defaults to Forward indicating traversal is from source to target.")]
        [Output("graphs", "The collection of sub Graphs found in the input Graph.")]
        public static List<Graph> SubGraphs(this Graph graph, RelationDirection relationDirection = RelationDirection.Forwards)
        {
            if(graph == null)
            {
                BH.Engine.Base.Compute.RecordError("Cannot query the sub graphs of a null graph.");
                return new List<Graph>();
            }

            List<Graph> subGraphs = new List<Graph>();
            m_Adjacency = graph.Adjacency(relationDirection);
            m_MarkedEntity = new Dictionary<Guid, int>();
            m_MarkedRelation = new Dictionary<Guid, int>();
            m_SubNumber = 0;
            graph.Entities.Keys.ToList().ForEach(entity => m_MarkedEntity[entity] = -1);
            graph.Relations.ForEach(rel => m_MarkedRelation[rel.BHoM_Guid] = -1);
            Random random = new Random(); 
            
            while (m_MarkedEntity.ContainsValue(-1))
            {
                List<Guid> entitys = m_MarkedEntity.Where(pair => pair.Value == -1)
                                           .Select(pair => pair.Key).ToList();
                //random start entity
                Guid start = entitys[random.Next(0, entitys.Count)];

                graph.Traverse(start);

                subGraphs.Add(graph.SetSubGraph());

                m_SubNumber++;
            }

            return subGraphs;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void Traverse(this Graph graph, Guid entity)
        {
            m_MarkedEntity[entity] = m_SubNumber;
           
            foreach(Guid c in m_Adjacency[entity])
            {
                //tag the relation
                IRelation relation = graph.Relations.Find(r => r.Source.Equals(entity) && r.Target.Equals(c));
                if (relation != null)
                    m_MarkedRelation[relation.BHoM_Guid] = m_SubNumber;

                if (m_MarkedEntity[c] < 0)
                    graph.Traverse(c);
            }
        }

        /***************************************************/
        private static Graph SetSubGraph(this Graph graph)
        {
            List<Guid> subEntities = m_MarkedEntity.Where(pair => pair.Value == m_SubNumber)
                                           .Select(pair => pair.Key).ToList();

            List<Guid> subRelations = m_MarkedRelation.Where(pair => pair.Value == m_SubNumber)
                                       .Select(pair => pair.Key).ToList();

            Graph subgraph = new Graph();

            subEntities.ForEach(ent => subgraph.Entities.Add(ent, graph.Entities[ent]));

            List<IRelation> relations = new List<IRelation>();

            subgraph.Relations.AddRange(graph.Relations.FindAll(r => subRelations.Contains(r.BHoM_Guid)));

            return subgraph;
        }

        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private static Dictionary<Guid, List<Guid>> m_Adjacency;
        private static Dictionary<Guid, int> m_MarkedEntity = new Dictionary<Guid, int>();
        private static Dictionary<Guid, int> m_MarkedRelation = new Dictionary<Guid, int>();
        private static int m_SubNumber = 0;
    }
}




