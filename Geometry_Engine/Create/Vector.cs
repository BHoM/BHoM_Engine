using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Vector Vector(double x = 0, double y = 0, double z = 0)
        {
            return new Vector { X = x, Y = y, Z = z };
        }

        /***************************************************/

        public static Vector Vector(Point v)
        {
            return new Vector { X = v.X, Y = v.Y, Z = v.Z };
        }

        /***************************************************/
    }
}
