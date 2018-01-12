using BH.oM.Geometry;
using System.Linq;

namespace BH.Engine.Geometry.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static Point Orient(this Point point, Plane fromPlane, Plane toPlane)
        {
            Point o1 = fromPlane.Origin;
            Point o2 = toPlane.Origin;
            Vector n1 = fromPlane.Normal;
            Vector n2 = toPlane.Normal;

            Point newPoint = o1.Translate(new Vector { X = o2.X - o1.X, Y = o2.Y - o1.Y, Z = o2.Z - o1.Z });
            if (!(n1.IsParallel(n2) == 0))
            {
                Vector axis = n1.CrossProduct(n2);
                double angle = n1.Angle(n2);
                newPoint = newPoint.Rotate(o2, axis, angle);
            }
            return newPoint;
        }
    }
}
