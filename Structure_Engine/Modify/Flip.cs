using BH.oM.Structural.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;

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

        public static Edge Flip(this Edge edge)
        {
            Edge flipped = edge.GetShallowClone() as Edge;
            flipped.Curve = flipped.Curve.IFlip();

            return flipped;
        }
    }
}
