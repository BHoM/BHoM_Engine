using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Structural.Elements;
using BH.oM.Environmental.Elements;
using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        public static Storey Storey(this Building building, Point point)
        {
            if (building.Storeys == null || point == null || building.Storeys.Count < 1)
                return null;

            if (building.Storeys.Count == 1)
                return building.Storeys.First();

            if (point.Z >= building.Storeys.Last().Elevation)
                return building.Storeys.Last();

            if (point.Z <= building.Storeys.First().Elevation)
                return building.Storeys.First();

            for(int i = building.Storeys.Count - 1; i <= 1; i--)
            {
                if (building.Storeys[i - 1].Elevation < point.Z)
                    return building.Storeys[i];
            }

            return building.Storeys.First();
        }
    }
}
