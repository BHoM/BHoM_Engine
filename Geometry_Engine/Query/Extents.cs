using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Vector Extents(this BoundingBox box)
        {
            return new Vector { X = box.Max.X - box.Min.X, Y = box.Max.Y - box.Min.Y, Z = box.Max.Z - box.Min.Z };
        }

        /***************************************************/
    }
}
