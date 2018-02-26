using BH.oM.DataStructure;

namespace BH.Engine.DataStructure
{
    public static partial class Modify
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static void AddLink<T>(this Graph<T> graph, GraphNode<T> from, GraphNode<T> to, double weight = 1, bool bothDirection = false)
        {
            graph.Links.Add(new GraphLink<T> { StartNode = from, EndNode = to, Weight = weight });
            if (bothDirection)
                graph.Links.Add(new GraphLink<T> { StartNode = to, EndNode = from, Weight = weight });
        }

        /***************************************************/

        public static void AddLink<T>(this Graph<T> graph, GraphLink<T> link)
        {
            graph.Links.Add(link);
        }

        /***************************************************/
    }
}
