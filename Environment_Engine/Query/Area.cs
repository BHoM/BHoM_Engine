using BH.oM.Environment.Elements;
using System;
using System.Collections.Generic;

using System.Linq;
using BH.oM.Geometry;

using BH.Engine.Geometry;

using BH.Engine.Common;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double Area(this BuildingElement element)
        {
            return BH.Engine.Common.Query.Area(element);
        }

        public static double Area(this List<BuildingElement> space)
        {
            if (space.FloorGeometry() == null) return 0;
            else return space.FloorGeometry().Area();
        }
    }
}