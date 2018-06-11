using BH.oM.Geometry;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double Angle(this Vector v1, Vector v2)
        {
            double dotProduct = v1.DotProduct(v2);
            double length = v1.Length() * v2.Length();

            return (Math.Abs(dotProduct) < length) ? Math.Acos(dotProduct / length) : (dotProduct < 0) ? Math.PI : 0;
        }

        /***************************************************/

        public static double Angle(this Arc arc)
        {
            return arc.Angle;
        }

        /***************************************************/

        public static double SignedAngle(this Vector a, Vector b, Vector normal) // use normal vector to define the sign of the angle
        {
            double angle = Angle(a, b);

            Vector crossproduct = a.CrossProduct(b);
            if (crossproduct.DotProduct(normal) < 0)
                return -angle;
            else
                return angle;
        }

        /***************************************************/
    }
}
