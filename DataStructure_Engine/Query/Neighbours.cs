using BH.oM.DataStructure;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.DataStructure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<GraphNode<T>> Neighbours<T>(this Graph<T> graph, GraphNode<T> node)
        {
            return graph.Links.Where(x => x.StartNode == node).Select(x => x.EndNode).ToList();
        }

        /***************************************************/
    }
}
