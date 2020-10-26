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
        [Description("Find all subgraphs around each entity at depth 1 within a graph")]
        public static List<Graph> EntityNeighbourhoods(this Graph graph, RelationDirection relationDirection = RelationDirection.Forwards)
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
            Dictionary<Guid, int> depths = graph.DepthDictionary(entity.BHoM_Guid);

            List<Guid> entities = depths.Where(pair => pair.Value == maxDepth)
                  .Select(pair => pair.Key).ToList();

            return graph.SetSubGraph(entity.BHoM_Guid, entities);
        }
        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/
        private static Graph SetSubGraph(this Graph graph, Guid sourceEntity, List<Guid> entityAdjacency)
        {

            Graph subgraph = new Graph();

            subgraph.Entities.Add(sourceEntity, graph.Entities[sourceEntity]);
            entityAdjacency.ForEach(ent => subgraph.Entities.Add(ent, graph.Entities[ent]));

            entityAdjacency.ForEach(ent => subgraph.Relations.AddRange(graph.RelationMatch(sourceEntity, ent)));
           
            return subgraph;
        }
    }
}
