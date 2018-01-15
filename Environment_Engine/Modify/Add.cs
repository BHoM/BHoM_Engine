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
        public static void Add(this Building Building, Storey Storey)
        {
            Building.Storeys.Add(Storey);
            Building.Storeys.Sort((x, y) => x.Elevation.CompareTo(y.Elevation));
        }

        public static void Add(this Building Building, Space Space)
        {
            Building.Spaces.Add(Space);

            if(Space.Storey == null)
            {
                // BoundingBox
            }
            else
            {

            }
        }
    }
}
