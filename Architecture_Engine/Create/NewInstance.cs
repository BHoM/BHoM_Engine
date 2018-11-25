using BH.oM.Architecture.Elements;

namespace BH.Engine.Architecture
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Grid NewInstance(this Grid grid)
        {
            return new Grid();
        }

        /***************************************************/
    }
}
