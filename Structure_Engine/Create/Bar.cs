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
            bar.StartNode = new Node(line.Start);
            bar.EndNode = new Node(line.End);
            bar.SetGeometry(line);
            return bar;
        }

        /***************************************************/

        public static Bar Bar(Point pointA, Point pointB)
        {
            Bar bar = new Bar();
            bar.StartNode = new Node(pointA);
            bar.EndNode = new Node(pointB);
            bar.SetGeometry(new Line(pointA, pointB));
            return bar;
        }
    }
}
