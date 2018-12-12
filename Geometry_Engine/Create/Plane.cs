using BH.oM.Geometry;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Plane Plane(Point origin, Vector normal)
        {
            return new Plane { Origin = origin, Normal = normal };
        }

        /***************************************************/

        public static Plane Plane(Point p1, Point p2, Point p3)
        {
            Vector normal = Query.CrossProduct(p2 - p1, p3 - p1).Normalise();
            return new Plane { Origin = p1.Clone(), Normal = normal };
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        public static Plane RandomPlane(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomPlane(rnd, box);
        }

        /***************************************************/

        public static Plane RandomPlane(Random rnd, BoundingBox box = null)
        {
            return new Plane
            {
                Origin = RandomPoint(rnd, box),
                Normal = RandomVector(rnd).Normalise()
            };
        }

        /***************************************************/
    }
}
