using BH.Engine.Base;
using BH.oM.Analytical.Elements;
using BH.oM.Base;
using BH.oM.Dimensional;
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
        [Description("Find all subgraphs around all entities at maximum depth 1 within a graph")]
        public static List<Graph> EntityNeighbourhood(this Graph graph, RelationDirection relationDirection = RelationDirection.Forwards)
        {
            List<Graph> subGraphs = new List<Graph>();
            m_Adjacency = graph.Adjacency(relationDirection);

            foreach (KeyValuePair<Guid, List<Guid>> kvp in m_Adjacency)
                subGraphs.Add(graph.SetSubGraph(kvp.Key, kvp.Value));
            
            return subGraphs;
        }
        /***************************************************/
        [Description("Find the subgraph around an entity at a specified depth within a graph")]
        public static Graph EntityNeighbourhood(this Graph graph, IBHoMObject entity, int maxDepth, RelationDirection relationDirection = RelationDirection.Forwards)
        {
            List<Graph> subGraphs = new List<Graph>();
            m_Adjacency = graph.Adjacency(relationDirection);
            m_AccessibleEntities = new List<Guid>();
            m_AccessibleRelations = new List<Guid>();

            graph.Traverse(entity.BHoM_Guid,maxDepth,0);

            Graph subgraph = new Graph();
            foreach(Guid guid in m_AccessibleEntities)
            {
                if (!subgraph.Entities.ContainsKey(guid))
                    subgraph.Entities.Add(guid, graph.Entities[guid]);
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
        private static void Traverse(this Graph graph, Guid node, int maxDepth, int currentDepth)
        {
            if (currentDepth >= maxDepth)
                return;
            foreach (Guid c in m_Adjacency[node])
            {
                m_AccessibleEntities.Add(c);
                m_AccessibleRelations.Add(graph.Relations.Find(r => r.Source.Equals(node) && r.Target.Equals(c)).BHoM_Guid);
                graph.Traverse(c,maxDepth, currentDepth + 1);
            }
        }
        /***************************************************/
        private static Graph SetSubGraph(this Graph graph, Guid sourceEntity, List<Guid> entityAdjacency)
        {

            Graph subgraph = new Graph();

            subgraph.Entities.Add(sourceEntity, graph.Entities[sourceEntity]);
            entityAdjacency.ForEach(ent => subgraph.Entities.Add(ent, graph.Entities[ent]));

            entityAdjacency.ForEach(ent => subgraph.Relations.AddRange(graph.RelationMatch(sourceEntity, ent)));
           
            return subgraph;
        }
        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/
        private static List<Guid> m_AccessibleEntities = new List<Guid>();
        private static List<Guid> m_AccessibleRelations = new List<Guid>();

    }
}
