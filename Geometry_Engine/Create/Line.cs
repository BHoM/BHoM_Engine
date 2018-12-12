using BH.oM.Geometry;
using System;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Line Line(Point start, Point end)
        {
            return new Line
            {
                Start = start,
                End = end
            };
        }

        /***************************************************/

        public static Line Line(Point start, Vector direction)
        {
            return new Line
            {
                Start = start,
                End = start + direction,
                Infinite = true
            };
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        public static Line RandomLine(int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomLine(rnd, box);
        }

        /***************************************************/

        public static Line RandomLine(Random rnd, BoundingBox box = null)
        {
            return new Line
            {
                Start = RandomPoint(rnd, box),
                End = RandomPoint(rnd, box)
            };
        }

        /***************************************************/

        public static Line RandomLine(Point from, int seed = -1, BoundingBox box = null)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomLine(from, rnd, box);
        }

        /***************************************************/

        public static Line RandomLine(Point from, Random rnd, BoundingBox box = null)
        {
            return new Line
            {
                Start = from,
                End = RandomPoint(rnd, box)
            };
        }

        /***************************************************/
    }
}
