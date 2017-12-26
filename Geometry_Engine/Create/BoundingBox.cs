using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BoundingBox BoundingBox(Point min, Point max)
        {
            return new BoundingBox
            {
                Min = min,
                Max = max
            };
        }

        /***************************************************/

        public static BoundingBox BoundingBox(Point centre, Vector extent)
        {
            return new BoundingBox
            {
                Min = new Point { X = centre.X - extent.X, Y = centre.Y - extent.Y, Z = centre.Z - extent.Z },
                Max = new Point { X = centre.X + extent.X, Y = centre.Y + extent.Y, Z = centre.Z + extent.Z }
            };
        }

        /***************************************************/
    }
}
