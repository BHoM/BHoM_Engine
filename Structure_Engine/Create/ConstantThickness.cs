using BH.oM.Structure.Properties.Surface;
using BH.oM.Common.Materials;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ConstantThickness ConstantThickness(double thickness, Material material = null ,string name = "", PanelType type = PanelType.Undefined)
        {
            return new ConstantThickness { Thickness = thickness, PanelType = type, Material = material, Name = name};
        }

        /***************************************************/
    }
}
