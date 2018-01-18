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
        public static Storey Storey(this Building Building, Point Point)
        {
            if (Building.Storeys == null || Point == null || Building.Storeys.Count < 1)
                return null;

            if (Building.Storeys.Count == 1)
                return Building.Storeys.First();

            if (Point.Z >= Building.Storeys.Last().Elevation)
                return Building.Storeys.Last();

            if (Point.Z <= Building.Storeys.First().Elevation)
                return Building.Storeys.First();

            for(int i = Building.Storeys.Count - 1; i <= 1; i--)
            {
                if (Building.Storeys[i - 1].Elevation < Point.Z)
                    return Building.Storeys[i];
            }

            return Building.Storeys.First();
        }
    }
}
