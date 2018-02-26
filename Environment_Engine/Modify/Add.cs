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
        public static Building Add(this Building building, IEnumerable<Storey> Storeys)
        {
            Building aBuilding = building.GetShallowClone() as Building;
            aBuilding.Storeys = new List<Storey>(building.Storeys);

            if (Storeys == null)
                return null;

            aBuilding.Storeys.AddRange(Storeys);
            aBuilding.Storeys.Sort((x, y) => x.Elevation.CompareTo(y.Elevation));
            return aBuilding;
        }

        public static Building Add(this Building Building, Storey Storey)
        {
            Building aBuilding = Building.GetShallowClone() as Building;
            aBuilding.Storeys = new List<Storey>(Building.Storeys);

            if (Storey == null)
                return null;

            aBuilding.Storeys.Add(Storey);
            aBuilding.Storeys.Sort((x, y) => x.Elevation.CompareTo(y.Elevation));
            return aBuilding;
        }

        public static Building Add(this Building Building, Space Space)
        {
            Building aBuilding = Building.GetShallowClone() as Building;
            aBuilding.Spaces = new List<Space>(Building.Spaces);

            if (Space == null)
                return null;

            aBuilding.Spaces.Add(Space);

            if (Space.Storey == null)
            {
                // BoundingBox
            }
            else
            {

            }

            return aBuilding;
        }
    }
}
