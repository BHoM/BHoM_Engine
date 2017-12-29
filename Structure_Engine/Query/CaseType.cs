using BH.oM.Structural.Loads;

namespace BH.Engine.Structure
{
    public static partial class Query 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static CaseType CaseType(Loadcase loadcase)
        {
            return oM.Structural.Loads.CaseType.Simple;
        }

        /***************************************************/

        public static CaseType CaseType(LoadCombination loadCombination)
        {
            return oM.Structural.Loads.CaseType.Combination;
        }

        /***************************************************/
    }
}
