using BH.oM.Geometry;
using BH.oM.Structure.Elements;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Node SetGeometry(this Node node, Point point)
        {
            Node clone = node.GetShallowClone() as Node;
            clone.Position = point;
            return clone;
        }

        /***************************************************/

        public static Bar SetGeometry(this Bar bar, ICurve curve)
        {
            Line line = curve as Line;
            if (line == null)
                return null;

            Bar clone = bar.GetShallowClone() as Bar;
            clone.StartNode = clone.StartNode.SetGeometry(line.Start);
            clone.EndNode = clone.EndNode.SetGeometry(line.End);
            return clone;
        }

        /***************************************************/

        public static Edge SetGeometry(this Edge edge, ICurve curve)
        {
            Edge clone = edge.GetShallowClone() as Edge;
            clone.Curve = curve;
            return clone;
        }

        /***************************************************/

        public static PanelFreeForm SetGeometry(this PanelFreeForm contour, ISurface surface)
        {
            PanelFreeForm clone = contour.GetShallowClone() as PanelFreeForm;
            clone.Surface = surface as ISurface;
            return clone;
        }

        /***************************************************/
    }
}
