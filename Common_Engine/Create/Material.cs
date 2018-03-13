using BH.oM.Common.Materials;

namespace BH.Engine.Common
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Material Material(string name, MaterialType type = MaterialType.Steel, double E = 210000000000, double v = 0.3, double tC = 0.000012, double denisty = 7850)
        {
            return new Material
            {
                Name = name,
                Type = type,
                YoungsModulus = E,
                PoissonsRatio = v,
                CoeffThermalExpansion = tC,
                Density = denisty
            };
        }

        /***************************************************/
    }
}
