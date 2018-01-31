using BH.oM.Structural.Properties;
using BH.oM.Common.Materials;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ConstantThickness ConstantThickness(double thickness, Material material = null ,string name = "", PanelType type = PanelType.Slab)
        {
            ConstantThickness ct = new ConstantThickness { Thickness = thickness, Type = type};

            if (material != null)
                ct.Material = material;

            if (name != null)
                ct.Name = name;

            return ct;
        }

        /***************************************************/
    }
}
