using BH.oM.Environment.Elements;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Opening NewInstance(this Opening opening)
        {
            return new Opening();
        }

        /***************************************************/

        public static Panel NewInstance(this Panel panel)
        {
            return new Panel();
        }

        /***************************************************/
    }
}
