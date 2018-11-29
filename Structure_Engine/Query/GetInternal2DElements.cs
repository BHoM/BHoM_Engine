using BH.oM.Common;
using BH.oM.Structure.Elements;
using System.Collections.Generic;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        public static List<IElement2D> GetInternal2DElements(this Opening opening)
        {
            return new List<IElement2D>();
        }

        /***************************************************/

        public static List<IElement2D> GetInternal2DElements(this PanelPlanar panelPlanar)
        {
            return new List<IElement2D>(panelPlanar.Openings);
        }

        /***************************************************/
    }
}
