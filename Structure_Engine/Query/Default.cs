using BH.oM.Geometry;
using BH.oM.Structural.Properties;
using BH.oM.Common.Materials;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Material Default(this MaterialType materialType)
        {
            string libraryName = "Materials";
            string matName = null;
            switch (materialType)
            {
                case MaterialType.Aluminium:
                    matName = "ALUM";
                    break;
                case MaterialType.Steel:
                    matName = "S355";
                    break;
                case MaterialType.Concrete:
                    matName = "C30/37";
                    break;
                case MaterialType.Timber:
                    matName = "TIMBER";
                    break;
                case MaterialType.Rebar:
                    matName = "B500B";
                    break;
                case MaterialType.Cable:
                    matName = "CaFullLock";
                    break;
                case MaterialType.Tendon:
                case MaterialType.Glass:
                default:
                    break;
            }

            if (matName != null)
                return (Material)Library.Query.Match(libraryName, matName, true, true);

            return null;
        }

    }
}
