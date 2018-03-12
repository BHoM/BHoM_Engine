using System.Linq;

using BH.oM.Environmental.Elements;
using BH.oM.Geometry;
using BH.oM.Architecture.Elements;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Level Level(this Building building, Point point)
        {
            if (building.Levels == null || point == null || building.Levels.Count < 1)
                return null;

            if (building.Levels.Count == 1)
                return building.Levels.First();

            if (point.Z >= building.Levels.Last().Elevation)
                return building.Levels.Last();

            if (point.Z <= building.Levels.First().Elevation)
                return building.Levels.First();

            for(int i = building.Levels.Count - 1; i <= 1; i--)
            {
                if (building.Levels[i - 1].Elevation < point.Z)
                    return building.Levels[i];
            }

            return building.Levels.First();
        }

        /***************************************************/
    }
}
