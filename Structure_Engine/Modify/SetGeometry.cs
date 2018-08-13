using BH.oM.Geometry;
using BH.oM.Structural.Elements;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Bar SetGeometry(this Bar bar, Line line)
        {
            Bar clone = bar.GetShallowClone() as Bar;
            clone.StartNode = clone.StartNode.SetGeometry(line.Start);
            clone.EndNode = clone.EndNode.SetGeometry(line.End);
            return clone;
        }

        /***************************************************/

        public static Node SetGeometry(this Node node, Point point)
        {
            Node clone = node.GetShallowClone() as Node;
            clone.Position = point;
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
