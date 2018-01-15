using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BoundingBox Inflate(this BoundingBox box, double amount)
        {
            Vector extents = new Vector { X = amount, Y = amount, Z = amount };
            return new BoundingBox { Min = box.Min - extents, Max = box.Max + extents };
        }

        /***************************************************/
    }
}
