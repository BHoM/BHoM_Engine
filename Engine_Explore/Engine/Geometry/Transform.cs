using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine_Explore.BHoM.Geometry;

namespace Engine_Explore.Engine.Geometry
{
    public static class Transform
    {
        public static Vector Normalise(Vector vector)
        {
            double x = vector.X;
            double y = vector.Y;
            double z = vector.Z;
            double d = Math.Sqrt(x * x + y * y + z * z);

            return new Vector(x / d, y / d, z / d);
        }
    }
}
