using BH.oM.Environment.Elements;
using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IElement2D NewInternalElement2D(this Panel panel)
        {
            return new Opening();
        }

        /***************************************************/
    }
}
