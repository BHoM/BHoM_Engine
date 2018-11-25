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

        public static List<IElement2D> GetInternal2DElements(this Opening opening)
        {
            return new List<IElement2D>();
        }

        /***************************************************/

        public static List<IElement2D> GetInternal2DElements(this Panel panel)
        {
            return panel.Openings.Select(o => o as IElement2D).ToList();
        }

        /***************************************************/
    }
}
