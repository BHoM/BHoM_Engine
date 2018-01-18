using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environmental.Elements;
using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        public static BuildingElementCurve BuildingElementCurve(ICurve Curve)
        {
            return new BuildingElementCurve
            {
                Curve = Curve
            };
        }
    }
}
