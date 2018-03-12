using System.Collections.Generic;

using BH.oM.Environmental.Elements;
using BH.oM.Structural.Elements;
using BH.oM.Architecture.Elements;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Building Add(this Building building, IEnumerable<Level> levels)
        {
            Building aBuilding = building.GetShallowClone() as Building;
            aBuilding.Levels = new List<Level>(building.Levels);

            if (levels == null)
                return null;

            aBuilding.Levels.AddRange(levels);
            aBuilding.Levels.Sort((x, y) => x.Elevation.CompareTo(y.Elevation));
            return aBuilding;
        }

        /***************************************************/

        public static Building Add(this Building building, Level level)
        {
            Building aBuilding = building.GetShallowClone() as Building;
            aBuilding.Levels = new List<Level>(building.Levels);

            if (level == null)
                return null;

            aBuilding.Levels.Add(level);
            aBuilding.Levels.Sort((x, y) => x.Elevation.CompareTo(y.Elevation));
            return aBuilding;
        }

        /***************************************************/

        public static Building Add(this Building building, Space space)
        {
            Building aBuilding = building.GetShallowClone() as Building;
            aBuilding.Spaces = new List<Space>(building.Spaces);

            if (space == null)
                return null;

            aBuilding.Spaces.Add(space);

            if (space.Level == null)
            {
                // BoundingBox
            }
            else
            {

            }

            return aBuilding;
        }

        /***************************************************/
    }
}
