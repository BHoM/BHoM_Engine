using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BoundingBox Bounds(this Panel buildingElementPanel)
        {
            return Geometry.Query.Bounds(buildingElementPanel.PolyCurve);
        }

        /***************************************************/
    }
}
