using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environmental.Elements;
using BH.oM.Structural.Elements;
using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Building Add(this Building building, IEnumerable<Storey> storeys)
        {
            Building aBuilding = building.GetShallowClone() as Building;
            aBuilding.Storeys = new List<Storey>(building.Storeys);

            if (storeys == null)
                return null;

            aBuilding.Storeys.AddRange(storeys);
            aBuilding.Storeys.Sort((x, y) => x.Elevation.CompareTo(y.Elevation));
            return aBuilding;
        }

        /***************************************************/

        public static Building Add(this Building building, Storey storey)
        {
            Building aBuilding = building.GetShallowClone() as Building;
            aBuilding.Storeys = new List<Storey>(building.Storeys);

            if (storey == null)
                return null;

            aBuilding.Storeys.Add(storey);
            aBuilding.Storeys.Sort((x, y) => x.Elevation.CompareTo(y.Elevation));
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

            if (space.Storey == null)
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
