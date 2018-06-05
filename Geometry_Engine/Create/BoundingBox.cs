using BH.oM.Geometry;
using System;

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

        public static BoundingBox RandomBoundingBox(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomBoundingBox(rnd, box);
        }

        /***************************************************/

        public static BoundingBox RandomBoundingBox(Random rnd, BoundingBox box = null)
        {
            return new BoundingBox
            {
                Min = RandomPoint(rnd, box),
                Max = RandomPoint(rnd, box)
            };
        }

        /***************************************************/
    }
}
