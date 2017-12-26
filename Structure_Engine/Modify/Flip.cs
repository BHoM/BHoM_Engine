using BH.oM.Structural.Elements;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Bar Flip(this Bar bar)
        {
            Bar flipped = bar.GetShallowClone() as Bar;

            Node tempNode = flipped.StartNode;
            flipped.StartNode = flipped.EndNode;
            flipped.EndNode = tempNode;

            return flipped;
        }

        /***************************************************/
    }
}
