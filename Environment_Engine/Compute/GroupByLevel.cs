using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using BH.oM.Environment.Elements;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        public static void GroupByLevel (List<Panel>panels, double minLevel, double maxLevel, double tolerance)
        {
            var = lowerPanels = panels.Where(x => x.Bottom().IControlPoints().Select(y => y.Z).Min() > minLevel).ToList();
        }
    }
}
