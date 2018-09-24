using System;
using System.Linq;

using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.oM.Architecture.Elements;

using System.Collections.Generic;

using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Panel AnalyticalPanel(this BuildingElement bElement)
        {
            return Create.Panel(bElement.PanelCurve, bElement.Openings);
        }
    }
}
