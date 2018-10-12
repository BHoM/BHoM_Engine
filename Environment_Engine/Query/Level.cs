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

        public static double MinimumLevel(this BuildingElement bElement)
        {
            List<Point> crvPts = bElement.PanelCurve.IControlPoints();

            double min = 1e10;
            foreach (Point p in crvPts)
                min = Math.Min(min, p.Z);

            return min;
        }

        public static double MaximumLevel(this BuildingElement bElement)
        {
            List<Point> crvPts = bElement.PanelCurve.IControlPoints();

            double max = -1e10;
            foreach (Point p in crvPts)
                max = Math.Max(max, p.Z);

            return max;
        }

        public static Level Level(this BuildingElement bElement, IEnumerable<Level> levels)
        {
            double min = bElement.MinimumLevel();
            double max = bElement.MaximumLevel();

            return levels.Where(x => x.Elevation >= min && x.Elevation <= max).FirstOrDefault();
        }

        public static Level Level(this Space space, IEnumerable<Level> levels)
        {
            return levels.Where(x => x.Elevation >= space.Location.Z && x.Elevation <= space.Location.Z).FirstOrDefault();
        }

        public static Level Level(this List<BuildingElement> space, Level level)
        {
            Polyline floor = space.FloorGeometry();
            List<Point> floorPts = floor.IControlPoints();

            bool allPointsOnLevel = true;
            foreach(Point pt in floorPts)
                allPointsOnLevel &= (pt.Z > (level.Elevation - BH.oM.Geometry.Tolerance.Distance) && pt.Z < (level.Elevation + BH.oM.Geometry.Tolerance.Distance));

            if (!allPointsOnLevel) return null;
            return level;
        }

        public static Level Level(this List<BuildingElement> space, List<Level> levels)
        {
            foreach(Level l in levels)
            {
                Level match = space.Level(l);
                if (match != null) return match;
            }

            return null;
        }
    }
}
