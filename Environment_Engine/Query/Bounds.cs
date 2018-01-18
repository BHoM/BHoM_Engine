using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environmental.Elements;
using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        public static BoundingBox Bounds(this BuildingElementPanel BuildingElementPanel)
        {
            return Geometry.Query.Bounds(BuildingElementPanel.PolyCurve);
        }
    }
}
