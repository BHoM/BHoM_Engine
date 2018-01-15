using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Point Point(double x = 0, double y = 0, double z = 0)
        {
            return new Point { X = x, Y = y, Z = z };
        }

        /***************************************************/

        public static Point Point(Vector v)
        {
            return new Point { X = v.X, Y = v.Y, Z = v.Z };
        }

        /***************************************************/
    }
}
