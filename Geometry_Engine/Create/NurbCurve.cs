using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
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

        [NotImplemented]
        public static NurbsCurve NurbsCurve(IEnumerable<Point> controlPoints, int degree = 3)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static NurbsCurve NurbsCurve(IEnumerable<Point> controlPoints, IEnumerable<double> weights, int degree = 3)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static NurbsCurve NurbsCurve(IEnumerable<Point> controlPoints, IEnumerable<double> weights, IEnumerable<double> knots)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        [NotImplemented]
        public static NurbsCurve RandomNurbsCurve(int seed = -1, BoundingBox box = null, int minNbCPs = 5, int maxNbCPs = 20)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static NurbsCurve RandomNurbsCurve(Random rnd, BoundingBox box = null, int minNbCPs = 5, int maxNbCPs = 20)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static NurbsCurve RandomNurbsCurve(Point from, int seed = -1, BoundingBox box = null, int minNbCPs = 5, int maxNbCPs = 20)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static NurbsCurve RandomNurbsCurve(Point from, Random rnd, BoundingBox box = null, int minNbCPs = 5, int maxNbCPs = 20)
        {
            throw new NotImplementedException();
        }

        /***************************************************/
    }
}
