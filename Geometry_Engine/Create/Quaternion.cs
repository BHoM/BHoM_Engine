using BH.oM.Geometry;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Quaternion Quaternion(double x, double y, double z, double w)
        {
            return new Quaternion { X = x, Y = y, Z = z, W = w };
        }

        /***************************************************/

        public static Quaternion Quaternion(Vector axis, double angle)
        {
            double sin = Math.Sin(angle / 2);
            return new Quaternion
            {
                X = axis.X * sin,
                Y = axis.Y * sin,
                Z = axis.Z * sin,
                W = Math.Cos(angle / 2)
            }.Normalise();
        }

        /***************************************************/

        public static Quaternion RandomQuaternion(int seed = -1)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomQuaternion(new Random(seed));
        }

        /***************************************************/

        public static Quaternion RandomQuaternion(Random rnd)
        {
            return new Quaternion { X = rnd.NextDouble(), Y = rnd.NextDouble(), Z = rnd.NextDouble(), W = rnd.NextDouble() }.Normalise();
        }

        /***************************************************/
    }
}
