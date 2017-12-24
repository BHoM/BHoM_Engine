using BH.oM.DataStructure;

namespace BH.Engine.DataStructure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static void AddNode<T>(this Graph<T> graph, GraphNode<T> node)
        {
            graph.Nodes.Add(node);
        }

        /***************************************************/

        public static GraphNode<T> AddNode<T>(this Graph<T> graph, T value)
        {
            GraphNode<T> newNode = Create.GraphNode<T>(value);
            graph.Nodes.Add(newNode);
            return newNode;
        }

        /***************************************************/
    }
}
