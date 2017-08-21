using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Transform
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Vector GetNormalised(this Vector vector)
        {
            double x = vector.X;
            double y = vector.Y;
            double z = vector.Z;
            double d = Math.Sqrt(x * x + y * y + z * z);

            return new Vector(x / d, y / d, z / d);
        }

        /***************************************************/

        public static Quaternion GetNormalised(this Quaternion q)
        {
            double x = q.X;
            double y = q.Y;
            double z = q.Z;
            double w = q.W;
            double d = Math.Sqrt(x * x + y * y + z * z + w * w);

            return new Quaternion(x / d, y / d, z / d, w / d);
        }
    }
}
