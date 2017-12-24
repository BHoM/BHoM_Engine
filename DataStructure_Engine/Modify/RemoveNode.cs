using BH.oM.DataStructure;

namespace BH.Engine.DataStructure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static void RemoveNode<T>(this Graph<T> graph, GraphNode<T> node)
        {
            graph.Links.RemoveAll(x => x.StartNode == node || x.EndNode == node);
            graph.Nodes.Remove(node);
        }

        /***************************************************/
    }
}
