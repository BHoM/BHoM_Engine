using BH.oM.Planning;

namespace BH.Engine.Planning
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Commit Commit(string name)
        {
            return new Commit { Name = name };
        }

        /***************************************************/
    }
}
