using BH.oM.Analytical.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Analytical
{
    public static partial class Query
    {
        public static List<ProcessResult> ProcessRelations(this Graph graph)
        {
            List<ProcessRelation> relations = graph.Relations.FindAll(rel => rel is ProcessRelation).Cast<ProcessRelation>().ToList();
            List<ProcessResult> results = new List<ProcessResult>();
            relations.ForEach(rel => results.AddRange(rel.Process(graph.Entities[rel.Source], graph.Entities[rel.Target])));
            return results;
        }
    }
}
