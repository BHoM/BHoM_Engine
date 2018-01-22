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
        public static Building Add(this Building Building, IEnumerable<Storey> Storeys)
        {
            if (Storeys == null)
                return null;

            Building.Storeys.AddRange(Storeys);
            Building.Storeys.Sort((x, y) => x.Elevation.CompareTo(y.Elevation));
            return Building;
        }

        public static Building Add(this Building Building, Storey Storey)
        {
            if (Storey == null)
                return null;

            Building.Storeys.Add(Storey);
            Building.Storeys.Sort((x, y) => x.Elevation.CompareTo(y.Elevation));
            return Building;
        }

        public static Building Add(this Building Building, Space Space)
        {
            if (Space == null)
                return null;

            Building.Spaces.Add(Space);

            if(Space.Storey == null)
            {
                // BoundingBox
            }
            else
            {

            }

            return Building;
        }
    }
}
