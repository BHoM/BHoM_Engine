using BH.oM.Geometry;
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
    }
}
