using BH.oM.DataStructure;

namespace BH.Engine.DataStructure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static GraphNode<T> GraphNode<T>(T value = default(T))
        {
            return new GraphNode<T>
            {
                Value = value
            };
        }

        /***************************************************/
    }
}
