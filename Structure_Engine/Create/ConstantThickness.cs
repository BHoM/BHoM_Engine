using BH.oM.Structural.Properties;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ConstantThickness ConstantThickness(string name)
        {
            return new ConstantThickness { Name = name, Type = PanelType.Undefined };
        }

        /***************************************************/

        public static ConstantThickness ConstantThickness(string name, double thickness, PanelType type)
        {
            return new ConstantThickness { Name = name, Thickness = thickness, Type = type };
        }

        /***************************************************/
    }
}
