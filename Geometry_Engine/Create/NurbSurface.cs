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

        public static NurbSurface NurbSurface(IEnumerable<Point> controlPoints, int degree = 3)
        {
            int n = controlPoints.Count();
            int d = degree - 1;

            return new NurbSurface
            {
                ControlPoints = controlPoints.ToList(),
                Weights = Enumerable.Repeat(1.0, n).ToList()
                //Knots = Enumerable.Repeat(0, d).Concat(Enumerable.Range(0, n - d).Concat(Enumerable.Repeat(n - d - 1, d))).Select(x => (double)x).ToList();
                //TODO: Calculate the U-knots and V-knots
            };
        }

        /***************************************************/

        public static NurbSurface NurbSurface(IEnumerable<Point> controlPoints, IEnumerable<double> weights, int degree = 3)
        {
            int n = controlPoints.Count();
            int d = degree - 1;

            return new NurbSurface
            {
                ControlPoints = controlPoints.ToList(),
                Weights = weights.ToList()
                //Knots = Enumerable.Repeat(0, d).Concat(Enumerable.Range(0, n - d).Concat(Enumerable.Repeat(n - d - 1, d))).Select(x => (double)x).ToList();
                //TODO: Calculate the U-knots and V-knots
            };
        }

        /***************************************************/

        public static NurbSurface NurbSurface(IEnumerable<Point> controlPoints, IEnumerable<double> weights, IEnumerable<double> uKnots, IEnumerable<double> vKnots)
        {
            return new NurbSurface
            {
                ControlPoints = controlPoints.ToList(),
                Weights = weights.ToList(),
                UKnots = uKnots.ToList(),
                VKnots = vKnots.ToList()
            };
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        public static NurbSurface RandomNurbSurface(int seed = -1, BoundingBox box = null, int minNbCPs = 4, int maxNbCPs = 20)
        {
            if (seed == -1)
                seed = m_Random.Next();
            Random rnd = new Random(seed);
            return RandomNurbSurface(rnd, box, minNbCPs, maxNbCPs);
        }

        /***************************************************/

        public static NurbSurface RandomNurbSurface(Random rnd, BoundingBox box = null, int minNbCPs = 4, int maxNbCPs = 20)
        {
            if (box == null)
                box = new BoundingBox { Min = Point(0,0,0), Max = Point(1, 1, 1) };

            int nb1 = rnd.Next(2, 1 + maxNbCPs / 2);
            int nb2 = rnd.Next(minNbCPs / nb1, 1 + maxNbCPs / nb1);
            double maxNoise = rnd.NextDouble() * Math.Min(box.Max.X - box.Min.X, Math.Min(box.Max.Y-box.Min.Y, box.Max.Z - box.Min.Z)) / 5;
            Ellipse ellipse = RandomEllipse(rnd, box.Inflate(maxNoise));  // TODO: Using Ellipse doesn't guarantee the grid will be in the bounding box
            Point start = ellipse.Centre - ellipse.Radius1 * ellipse.Axis1 - ellipse.Radius2 * ellipse.Axis2;
            Vector normal = ellipse.Axis1.CrossProduct(ellipse.Axis2);
            List<Point> points = PointGrid(start, ellipse.Axis1, ellipse.Axis2, nb1, nb2)
                .SelectMany(x => x)
                .Select(x => x + 2*maxNoise*(rnd.NextDouble()-0.5)*normal)
                .ToList();

            return NurbSurface(points);
        }

        /***************************************************/
    }
}
