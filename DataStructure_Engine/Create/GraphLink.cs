using BH.oM.DataStructure;

namespace BH.Engine.DataStructure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static GraphLink<T> GraphLink<T>(GraphNode<T> from, GraphNode<T> to, double weight = 1.0) 
        {
            return new GraphLink<T>
            {
                StartNode = from,
                EndNode = to,
                Weight = weight
            };
        }

        /***************************************************/
    }
}
