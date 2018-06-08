using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Polyline Polyline(IEnumerable<Point> points)
        {
            return new Polyline { ControlPoints = points.ToList() };
        }

        /***************************************************/

        public static Polyline RandomPolyline(int seed = -1, BoundingBox box = null, int minNbCPs = 3, int maxNbCPs = 20)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomPolyline(rnd, box, minNbCPs, maxNbCPs);
        }

        /***************************************************/

        public static Polyline RandomPolyline(Random rnd, BoundingBox box = null, int minNbCPs = 3, int maxNbCPs = 20)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < rnd.Next(minNbCPs, maxNbCPs + 1); i++)
                points.Add(RandomPoint(rnd, box));
            return new Polyline { ControlPoints = points };
        }

        /***************************************************/

        public static Polyline RandomPolyline(Point from, int seed = -1, BoundingBox box = null, int minNbCPs = 3, int maxNbCPs = 20)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomPolyline(from, rnd, box, minNbCPs, maxNbCPs);
        }

        /***************************************************/

        public static Polyline RandomPolyline(Point from, Random rnd, BoundingBox box = null, int minNbCPs = 3, int maxNbCPs = 20)
        {
            List<Point> points = new List<Point>();
            points.Add(from);
            for (int i = 0; i < rnd.Next(minNbCPs, maxNbCPs + 1)-1; i++)
                points.Add(RandomPoint(rnd, box));
            return new Polyline { ControlPoints = points };
        }

        /***************************************************/
    }
}
