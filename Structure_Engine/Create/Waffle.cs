using BH.oM.Structural.Properties;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Waffle Waffle(string name, double slabDepth, double depthX, double depthY, double stemWidthX, double stemWidthY, double spacingX, double spacingY, PanelType type = PanelType.Undefined)
        {
            return new Waffle
            {
                Name = name,
                Thickness = slabDepth,
                TotalDepthX = depthX,
                TotalDepthY = depthY,
                StemWidthX = stemWidthX,
                StemWidthY = stemWidthY,
                SpacingX = spacingX,
                SpacingY = spacingY,
                PanelType = type
            };
        }

        /***************************************************/
    }
}
