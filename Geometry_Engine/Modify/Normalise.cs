using BH.oM.Geometry;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Vector Normalise(this Vector vector)
        {
            double x = vector.X;
            double y = vector.Y;
            double z = vector.Z;
            double d = Math.Sqrt(x * x + y * y + z * z);

            if (d == 0) return vector.Clone();
            return new Vector { X = x / d, Y = y / d, Z = z / d };
        }

        /***************************************************/

        public static Quaternion Normalise(this Quaternion q)
        {
            double x = q.X;
            double y = q.Y;
            double z = q.Z;
            double w = q.W;
            double d = Math.Sqrt(x * x + y * y + z * z + w * w);

            return new Quaternion { X = x / d, Y = y / d, Z = z / d, W = w / d };
        }

        /***************************************************/
    }
}
