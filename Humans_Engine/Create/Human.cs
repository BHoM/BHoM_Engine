using BH.oM.Humans;

namespace BH.Engine.Humans
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Human Human(string name)
        {
            return new Human { Name = name };
        }

        /***************************************************/
    }
}
