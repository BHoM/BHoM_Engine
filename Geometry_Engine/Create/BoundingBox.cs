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
                Min = new Point(centre.X - extent.X, centre.Y - extent.Y, centre.Z - extent.Z),
                Max = new Point(centre.X + extent.X, centre.Y + extent.Y, centre.Z + extent.Z)
            };
        }

        /***************************************************/
    }
}
