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
            Point p1 = RandomPoint(rnd, box);
            Point p2 = RandomPoint(rnd, box);
            return new BoundingBox
            {
                Min = new Point()
                {
                    X = Math.Min(p1.X, p2.X),
                    Y = Math.Min(p1.Y, p2.Y),
                    Z = Math.Min(p1.Z, p2.Z),
                },
                Max = new Point()
                {
                    X = Math.Max(p1.X, p2.X),
                    Y = Math.Max(p1.Y, p2.Y),
                    Z = Math.Max(p1.Z, p2.Z),
                },
            };
        }

        /***************************************************/
    }
}
