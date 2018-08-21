using BH.oM.Structure.Loads;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Loadcase Loadcase(string name, int number, LoadNature nature= LoadNature.Other)
        {
            return new Loadcase { Name = name, Number = number, Nature = nature };
        }

        /***************************************************/
    }
}
