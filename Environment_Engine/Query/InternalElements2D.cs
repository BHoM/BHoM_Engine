using BH.oM.Common;
using BH.oM.Environment.Elements;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        public static List<IElement2D> InternalElements2D(this Opening opening)
        {
            return new List<IElement2D>();
        }

        /***************************************************/

        public static List<IElement2D> InternalElements2D(this Panel panel)
        {
            return panel.Openings.Cast<IElement2D>().ToList();
        }

        /***************************************************/

        public static List<IElement2D> InternalElements2D(this BuildingElement element)
        {
            return element.Openings.Cast<IElement2D>().ToList();
        }

        /***************************************************/
    }
}
