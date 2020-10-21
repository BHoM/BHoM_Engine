using BH.Engine.Base;
using BH.oM.Analytical.Elements;
using System;
using System.Collections.Generic;
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
        public static Graph Reverse(this Graph graph)
        {
            List<IRelation> reversed = new List<IRelation>();
            foreach (IRelation relation in graph.Relations)
                reversed.Add(relation.Reverse());

            graph.Relations = reversed;
            return graph;
        }
        /***************************************************/
        public static IRelation Reverse(this IRelation relation)
        {
            Guid oldSource = relation.Source;
            Guid oldTarget = relation.Target;
            relation.Source = oldTarget;
            relation.Target = oldSource;
            relation.Subgraph.Reverse();

            return relation;

        }
        /***************************************************/
    }
}
