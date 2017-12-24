using BH.oM.Geometry;
using BH.oM.Structural.Elements;

namespace BH.Engine.Structure
{
    public static partial class Transform
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static void SetGeometry(this Bar bar, Line line)
        {
            bar.StartNode.Point = line.Start;
            bar.EndNode.Point = line.End;
        }

        /***************************************************/

        public static void SetGeometry(this Node node, Point point)
        {
            node.Point = point;
        }

        /***************************************************/

        public static void SetGeometry(this PanelFreeForm contour, IBHoMGeometry geometry)
        {
            if (typeof(ISurface).IsAssignableFrom(geometry.GetType()))
            {
                contour.Surface = geometry as ISurface;
            }
        }

        /***************************************************/

    }
}
