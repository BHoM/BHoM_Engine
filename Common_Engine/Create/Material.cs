using BH.oM.Common.Materials;

namespace BH.Engine.Common
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Material Material(string name)
        {
            return new Material { Name = name };
        }

        /***************************************************/

        public static Material Material(string name, MaterialType type, double E, double v, double tC, double G, double denisty)
        {
            return new Material
            {
                Name = name,
                Type = type,
                YoungsModulus = E,
                PoissonsRatio = v,
                CoeffThermalExpansion = tC,
                ShearModulus = G,
                Density = denisty
            };
        }

        /***************************************************/
    }
}
