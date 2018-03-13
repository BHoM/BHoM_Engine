using BH.oM.Common.Materials;

namespace BH.Engine.Common
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double ShearModulus(this Material material)
        {
            return material.YoungsModulus / (2 * (1 + material.PoissonsRatio));
        }

        /***************************************************/
    }
}
