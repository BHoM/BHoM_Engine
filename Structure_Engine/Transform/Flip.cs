using BH.oM.Structural.Elements;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static void FlipNodes(this Bar bar)
        {
            Node tempNode = bar.StartNode;
            bar.StartNode = bar.EndNode;
            bar.EndNode = tempNode;
        }

        /***************************************************/
    }
}
