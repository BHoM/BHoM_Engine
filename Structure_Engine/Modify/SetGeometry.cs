using BH.oM.Geometry;
using BH.oM.Structural.Elements;

namespace BH.Engine.Structure
{
    public static partial class Modify
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static void SetGeometry(this Bar bar, Line line)
        {
            bar.StartNode.Position = line.Start;
            bar.EndNode.Position = line.End;
        }

        /***************************************************/

        public static void SetGeometry(this Node node, Point point)
        {
            node.Position = point;
        }

        /***************************************************/

        public static void SetGeometry(this PanelFreeForm contour, IGeometry geometry)
        {
            if (typeof(ISurface).IsAssignableFrom(geometry.GetType()))
            {
                contour.Surface = geometry as ISurface;
            }
        }

        /***************************************************/

    }
}
