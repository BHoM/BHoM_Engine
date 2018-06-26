using BH.oM.Geometry;
using System;
using System.ComponentModel;

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

        [Description("Calculates the counterclockwise angle between two vectors in a plane")]
        public static double Angle(this Vector v1, Vector v2, Plane p)
        {
            v1 = v1.Project(p).Normalise();
            v2 = v2.Project(p).Normalise();

            double dotProduct = v1.DotProduct(v2);
            Vector n = p.Normal.Normalise();
           
            double det = v1.X * v2.Y * n.Z + v2.X * n.Y * v1.Z + n.X * v1.Y * v2.Z - v1.Z * v2.Y * n.X - v2.Z * n.Y * v1.X - n.Z * v1.Y * v2.X;

            double angle = Math.Atan2(det, dotProduct);
            return angle >= 0 ? angle : Math.PI * 2 + angle;

        }

        /***************************************************/

        public static double Angle(this Arc arc)
        {
            return arc.EndAngle - arc.StartAngle;
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
