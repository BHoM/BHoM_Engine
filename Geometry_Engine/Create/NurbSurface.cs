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
        public static NurbsSurface NurbsSurface(IEnumerable<Point> controlPoints, int degree = 3)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static NurbsSurface NurbsSurface(IEnumerable<Point> controlPoints, IEnumerable<double> weights, int degree = 3)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static NurbsSurface NurbsSurface(IEnumerable<Point> controlPoints, IEnumerable<double> weights, IEnumerable<double> uKnots, IEnumerable<double> vKnots)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Random Geometry                           ****/
        /***************************************************/

        [NotImplemented]
        public static NurbsSurface RandomNurbsSurface(int seed = -1, BoundingBox box = null, int minNbCPs = 4, int maxNbCPs = 20)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static NurbsSurface RandomNurbsSurface(Random rnd, BoundingBox box = null, int minNbCPs = 4, int maxNbCPs = 20)
        {
            throw new NotImplementedException();
        }

        /***************************************************/
    }
}
