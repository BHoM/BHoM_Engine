using BH.oM.Structure.Loads;

namespace BH.Engine.Structure
{
    public static partial class Query 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static CaseType CaseType(this Loadcase loadcase)
        {
            return oM.Structure.Loads.CaseType.Simple;
        }

        /***************************************************/

        public static CaseType CaseType(this LoadCombination loadCombination)
        {
            return oM.Structure.Loads.CaseType.Combination;
        }

        /***************************************************/
    }
}
