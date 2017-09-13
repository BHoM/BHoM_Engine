using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;
using BHoM.Geometry;

namespace BH.Engine.Geometry
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

        public static Point GetIntersection(this Line line, Mesh mesh, bool useInfiniteLine = false, double tolerance = Tolerance.Distance)
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

        private static Point _GetIntersections(this Arc curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static Point _GetIntersections(this Circle curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static Point _GetIntersections(this Line curve, Mesh mesh, double tolerance = Tolerance.Distance)   // TODO Add Mesh.GetTriangulated
        {
            // Mesh Points
            List<Point> meshPts = mesh.Vertices;
            Point p1 = meshPts[0];  /*Call function*/
            Point p2 = meshPts[1];  /*Call function*/
            Point p3 = meshPts[2];  /*Call function*/

            // Ray direction
            Vector d = curve.GetPointAtParameter(1) - curve.GetPointAtParameter(0);                     /*Call function*/

            // Vectors from p1 to p2/p3 (edges)
            Vector e1, e2;

            Vector p, q, t;
            double det, invDet, u, v;

            //Find vectors for two edges sharing vertex/point p1
            e1 = p2 - p1;
            e2 = p3 - p1;

            // calculating determinant 
            p = Measure.GetCrossProduct(d, e2);                                             /*Call function*/

            //Calculate determinat
            det = Measure.GetDotProduct(e1, p);                                             /*Call function*/

            //if determinant is near zero, ray lies in plane of triangle otherwise not
            if (det > -Double.Epsilon && det < Double.Epsilon) { return null; }
            invDet = 1.0f / det;

            //calculate distance from p1 to ray origin
            t = curve.GetPointAtParameter(0) - p1;                                              /*Call function*/

            //Calculate u parameter
            u = Measure.GetDotProduct(t, p) * invDet;   // can this be replaced by   t*p*invDet   ? Is ti faster?

            //Check for ray hit
            if (u < 0 || u > 1) { return null; }

            //Prepare to test v parameter
            q = Measure.GetCrossProduct(t, e1);                                             /*Call function*/

            //Calculate v parameter
            v = Measure.GetDotProduct(d, q) * invDet;                                       /*Call function*/

            //Check for ray hit
            if (v < 0 || u + v > 1) { return null; }

            if ((Measure.GetDotProduct(e2, q) * invDet) > Double.Epsilon)                   /*Call function*/
            {
                //ray does intersect
                return ((1 - u - v) * p1 + u * p2 + v * p3);
            }
            return null;
        }

        /***************************************************/

        private static Point _GetIntersections(this NurbCurve c, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static Point _GetIntersections(this PolyCurve curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static Point _GetIntersections(this Polyline curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }
    }
}
