using BH.oM.Structure.Elements;
using BH.oM.Common;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IElement2D NewInternalElement2D(this PanelPlanar panelPlanar)
        {
            return new Opening();
        }

        /***************************************************/
    }
}
