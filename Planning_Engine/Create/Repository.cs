using BH.oM.Planning;

namespace BH.Engine.Planning
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Repository Repository(string name)
        {
            return new Repository { Name = name };
        }

        /***************************************************/
    }
}