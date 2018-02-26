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
    }
}
