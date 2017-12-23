using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.DataStructure;

namespace BH.Engine.DataStructure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Graph<T> Graph<T>(List<GraphNode<T>> nodes = null, List<GraphLink<T>> links = null)
        {
            Graph<T> graph = new Graph<T>();
            if (nodes != null)
                graph.Nodes = nodes;
            if (links != null)
                graph.Links = links;

            return graph;
        }

        /***************************************************/
    }
}
