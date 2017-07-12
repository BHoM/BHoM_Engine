using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Geometry
{
    public static partial class Create
    {
        public static Vector Vector(double x, double y, double z)
        {
            return new BH.oM.Geometry.Vector(x, y, z);
        }
    }
}
