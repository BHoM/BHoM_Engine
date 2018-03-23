using BH.oM.Structural.Properties;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Ribbed Ribbed(string name)
        {
            return new Ribbed { Name = name };
        }

        /***************************************************/

        public static Ribbed Ribbed(string name, double slabDepth, double totalDepth, double stemWidth, double spacing, PanelDirection direction)
        {
            return new Ribbed
            {
                Name = name,
                Thickness = slabDepth,
                TotalDepth = totalDepth,
                StemWidth = stemWidth,
                Spacing = spacing,
                Direction = direction
            };
        }

        /***************************************************/
    }
}
