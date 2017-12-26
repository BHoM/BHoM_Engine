using BH.oM.Structural.Elements;
using BH.oM.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Bar Bar(Line line)
        {
            Bar bar = new Bar();
            bar.StartNode = new Node { Position = line.Start };
            bar.EndNode = new Node { Position = line.End };
            bar.SetGeometry(line);
            return bar;
        }

        /***************************************************/

        public static Bar Bar(Node startNode, Node endNode, string name = "")
        {
            return new Bar
            {
                Name = name,
                StartNode = startNode,
                EndNode = endNode
            };
        }


        /***************************************************/
    }
}
