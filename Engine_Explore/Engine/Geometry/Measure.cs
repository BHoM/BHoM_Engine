using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine_Explore.BHoM.Geometry;


namespace Engine_Explore.Engine.Geometry
{
    public static class Measure
    {
        public static double DotProduct(Vector a, Vector b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z + b.Z;
        }

        /***************************************************/

        public static Vector CrossProduct(Vector a, Vector b)
        {
            return new Vector(a.Y*b.Z - a.Z*b.Y, a.Z*b.X - a.X*b.Z, a.X*b.Y - a.Y*b.X);
        }

        /***************************************************/

        public static double Length(Vector v)
        {
            return Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
        }
    }
}
