using BH.oM.Structural.Loads;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Loadcase Loadcase(string name, LoadNature nature, double selfWeightMultiplier = 0)
        {
            return new Loadcase { Name = name, Nature = nature };
        }

        /***************************************************/
    }
}
