using BH.oM.Planning;

namespace BH.Engine.Planning
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static CodeAssembly CodeAssembly(string name, string repo)
        {
            return new CodeAssembly { Repository = repo, Name = name };
        }

        /***************************************************/
    }
}
