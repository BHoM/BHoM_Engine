using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Analytical.Elements;
using BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Compute
    {
        public static List<IRegion> RegionNameMapping(List<IRegion> regions, List<Point> points, List<string> names, double tolerance = Tolerance.Distance)
        {
            List<IRegion> sortedRegions = regions.OrderBy(x => x.Perimeter.IArea()).ToList();
            for (int x = 0; x < points.Count; x++)
            {
                IRegion selRegion = regions.Where(y => y.Perimeter.IIsContaining(new List<Point> { points[x] }, true, tolerance)).FirstOrDefault();
                if (selRegion != null)
                    selRegion.Name = names[x];
            }
            return regions;
        }
    }
}
