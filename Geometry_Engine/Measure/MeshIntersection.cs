using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;
using BHoM.Geometry;

namespace BH.Engine.Geometry.Measure
{
    public static partial class Measure
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Point> GetIntersections(this ICurve curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static Point GetIntersection(this Line line, Mesh mesh, bool useInfiniteLine = true, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static Line GetIntersection(this Plane p, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<Point> _GetIntersections(this Arc curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static List<Point> _GetIntersections(this Circle curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static List<Point> _GetIntersections(this Line curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static List<Point> _GetIntersections(this NurbCurve c, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static List<Point> _GetIntersections(this PolyCurve curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static List<Point> _GetIntersections(this Polyline curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }
    }
}
