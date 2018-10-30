using BH.oM.Geometry;
using BH.Engine.Geometry;
using System;
using System.Linq;
using BH.oM.Structure.Elements;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Node Scale(this Node node, Point origin, Vector scaleVector)
        {
            Node scaled = node.GetShallowClone() as Node;

            scaled.Position = node.Position.Scale(origin, scaleVector);

            return scaled;
        }

        /***************************************************/

        public static Node Scale(this Node node, Point origin, double scaleFactor)
        {
            Vector scaleVector = new Vector() { X = scaleFactor, Y = scaleFactor, Z = scaleFactor };

            return node.Scale(origin, scaleVector);
        }
        
        /***************************************************/

        public static Bar Scale(this Bar bar, Point origin, Vector scaleVector)
        {
            Bar scaled = bar.GetShallowClone() as Bar;

            scaled.StartNode = bar.StartNode.Scale(origin, scaleVector);
            scaled.EndNode = bar.EndNode.Scale(origin, scaleVector);

            if (bar.Offset != null)
            {
                scaled.Offset.Start = bar.Offset.Start.Scale(origin, scaleVector);
                scaled.Offset.End = bar.Offset.End.Scale(origin, scaleVector);
            }

            return scaled;
        }

        /***************************************************/

        public static Bar Scale(this Bar bar, Point origin, double scaleFactor)
        {
            Vector scaleVector = new Vector() { X = scaleFactor, Y = scaleFactor, Z = scaleFactor };
            
            return bar.Scale(origin, scaleVector);
        }

        /***************************************************/

    }
}
