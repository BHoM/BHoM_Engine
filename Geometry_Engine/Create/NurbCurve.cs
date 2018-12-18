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

        public static NurbsCurve NurbsCurve(IEnumerable<Point> controlPoints, int degree = 3)
        {
            int n = controlPoints.Count();
            int d = degree - 1;

            return new NurbsCurve
            {
                ControlPoints = controlPoints.ToList(),
                Weights = Enumerable.Repeat(1.0, n).ToList(),
                Knots = Enumerable.Repeat(0, d).Concat(Enumerable.Range(0, n - d).Concat(Enumerable.Repeat(n - d - 1, d))).Select(x => (double)x).ToList()
            };
        }

        /***************************************************/

        public static NurbsCurve NurbsCurve(IEnumerable<Point> controlPoints, IEnumerable<double> weights, int degree = 3)
        {
            int n = controlPoints.Count();
            int d = degree - 1;

            return new NurbsCurve
            {
                ControlPoints = controlPoints.ToList(),
                Weights = weights.ToList(),
                Knots = Enumerable.Repeat(0, d).Concat(Enumerable.Range(0, n - d).Concat(Enumerable.Repeat(n - d - 1, d))).Select(x => (double)x).ToList()
            };
        }

        /***************************************************/

        public static NurbsCurve NurbsCurve(IEnumerable<Point> controlPoints, IEnumerable<double> weights, IEnumerable<double> knots)
        {
            return new NurbsCurve
            {
                ControlPoints = controlPoints.ToList(),
                Weights = weights.ToList(),
                Knots = knots.ToList()
            };
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        public static NurbsCurve RandomNurbsCurve(int seed = -1, BoundingBox box = null, int minNbCPs = 5, int maxNbCPs = 20)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomNurbsCurve(rnd, box, minNbCPs, maxNbCPs);
        }

        /***************************************************/

        public static NurbsCurve RandomNurbsCurve(Random rnd, BoundingBox box = null, int minNbCPs = 5, int maxNbCPs = 20)
        {
            List<Point> points = new List<Point>();
            for (int i = 0; i < rnd.Next(minNbCPs, maxNbCPs + 1); i++)
                points.Add(RandomPoint(rnd, box));
            return NurbsCurve(points);
        }

        /***************************************************/

        public static NurbsCurve RandomNurbsCurve(Point from, int seed = -1, BoundingBox box = null, int minNbCPs = 5, int maxNbCPs = 20)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomNurbsCurve(from, rnd, box, minNbCPs, maxNbCPs);
        }

        /***************************************************/

        public static NurbsCurve RandomNurbsCurve(Point from, Random rnd, BoundingBox box = null, int minNbCPs = 5, int maxNbCPs = 20)
        {
            List<Point> points = new List<Point>();
            points.Add(from);
            for (int i = 0; i < rnd.Next(minNbCPs, maxNbCPs + 1)-1; i++)
                points.Add(RandomPoint(rnd, box));
            return NurbsCurve(points);
        }

        /***************************************************/
    }
}
