using BH.oM.DataStructure;

namespace BH.Engine.DataStructure
{
    public static partial class Modify
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static void RemoveLink<T>(this Graph<T> graph, GraphNode<T> from, GraphNode<T> to, bool bothDirection = false)
        {
            graph.Links.RemoveAll(x => x.StartNode == from && x.EndNode == to);

            if (bothDirection)
                graph.Links.RemoveAll(x => x.StartNode == to && x.EndNode == from);
        }

        /***************************************************/

        public static void RemoveLink<T>(this Graph<T> graph, GraphLink<T> link)
        {
            graph.Links.Remove(link);
        }

        /***************************************************/
    }
}
