using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double DotProduct(this Vector a, Vector b)
        {
            return (a.X * b.X + a.Y * b.Y + a.Z * b.Z);
        }

        /***************************************************/

        public static Vector CrossProduct(this Vector a, Vector b)
        {
            return new Vector(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
        }

        /***************************************************/

        public static Vector CrossProduct(this Point a, Point b)
        {
            return new Vector(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);
        }

        /***************************************************/

        public static Quaternion Product(this Quaternion q1, Quaternion q2)
        {
            return q1 * q2;
        }

        /***************************************************/
    }
}
