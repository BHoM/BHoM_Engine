using BH.oM.Analytical.Elements;
using BH.oM.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Modify
    {
        /***************************************************/
        /****           Public Methods                  ****/
        /***************************************************/
        [Description("Enforce unique entities on a graph")]
        public static Graph UniqueEntities(this Graph graph, Dictionary<Guid, IBHoMObject> replaceMap)
        {
            Dictionary<Guid, IBHoMObject> uniqueEntities = new Dictionary<Guid, IBHoMObject>();

            foreach (KeyValuePair<Guid, IBHoMObject> kvp in graph.Entities)
            {
                IBHoMObject unique = replaceMap[kvp.Key];
                if (!uniqueEntities.ContainsKey(unique.BHoM_Guid))
                    uniqueEntities.Add(unique.BHoM_Guid, unique);
            }

            graph.Entities = uniqueEntities;

            List<IRelation> uniqueRelations = new List<IRelation>();
            foreach (IRelation relation in graph.Relations)
            {
                IRelation relation1 = relation.UniqueEntities(replaceMap);
                //keep if it does not already exist
                if(!uniqueRelations.Any(r => r.Source.Equals(relation1.Source) && r.Target.Equals(relation1.Target)))
                    uniqueRelations.Add(relation1);

            }
            graph.Relations = uniqueRelations;
            return graph;
        }
        /***************************************************/
        public static IRelation UniqueEntities(this IRelation relation, Dictionary<Guid, IBHoMObject> replaceMap)
        {
            relation.Source = replaceMap[relation.Source].BHoM_Guid;
            relation.Target = replaceMap[relation.Target].BHoM_Guid;
            relation.Subgraph.UniqueEntities(replaceMap);
            return relation;
        }
    }
}
