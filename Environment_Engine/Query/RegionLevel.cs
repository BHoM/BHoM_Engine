using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Geometry.SettingOut;

using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Environment.Elements;
using BH.oM.Analytical.Elements;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        public static Level RegionLevel(this IRegion region, List<Level> searchLevels, double distanceTolerance = BH.oM.Geometry.Tolerance.Distance, double angleTolerance = BH.oM.Geometry.Tolerance.Angle)
        {
            double elevation = region.Perimeter.ICollapseToPolyline(angleTolerance).MinimumLevel();

            return searchLevels.Where(x => x.Elevation >= (elevation - distanceTolerance) && x.Elevation <= (elevation + distanceTolerance)).FirstOrDefault();
        }
    }
}
