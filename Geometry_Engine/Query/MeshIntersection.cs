using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Vector                   ****/
        /***************************************************/

        public static Line GetIntersection(this Plane p, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static List<Point> GetIntersections(this Arc curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static List<Point> GetIntersections(this Circle curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static List<Point> GetIntersections(this Line curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            // Outputs
            List<Point> points = new List<Point>();

            // Preprocessing Mesh
            Mesh tMesh = mesh.GetTriangulated();                                                /*Call function*/
            List<Face> faces = tMesh.Faces;                                                     /*Call function*/
            List<Point> meshPts = tMesh.Vertices;                                               /*Call function*/
            for (int i = 0; i < faces.Count; i++)
            {
                // Mesh Points
                Point p1 = meshPts[faces[i].A];                                                 /*Call function*/
                Point p2 = meshPts[faces[i].B];                                                 /*Call function*/
                Point p3 = meshPts[faces[i].C];                                                 /*Call function*/

                // Ray direction
                Vector d = curve.GetPointAtParameter(1) - curve.GetPointAtParameter(0);         /*Call function*/

                // Vectors from p1 to p2/p3 (edges)
                Vector e1, e2;

                Vector p, q, t;
                double det, invDet, u, v;

                //Find vectors for two edges sharing vertex/point p1
                e1 = p2 - p1;
                e2 = p3 - p1;

                // calculating determinant 
                p = Query.GetCrossProduct(d, e2);                                             /*Call function*/

                //Calculate determinat
                det = e1 * p;                                                                   /*Call function*/

                //if determinant is near zero, ray lies in plane of triangle otherwise not
                if (det > -Double.Epsilon && det < Double.Epsilon) { continue; }
                invDet = 1.0f / det;

                //calculate distance from p1 to ray origin
                t = curve.GetPointAtParameter(0) - p1;                                          /*Call function*/

                //Calculate u parameter
                u = t * p * invDet;                                                             /*Call function*/

                //Check for ray hit
                if (u < 0 || u > 1) { continue; }

                //Prepare to test v parameter
                q = Query.GetCrossProduct(t, e1);                                             /*Call function*/

                //Calculate v parameter
                v = d * q * invDet;                                                             /*Call function*/

                //Check for ray hit
                if (v < 0 || u + v > 1) { continue; }

                if ((e2 * q * invDet) > Double.Epsilon)                                         /*Call function*/
                {
                    //ray does intersect
                    points.Add((1 - u - v) * p1 + u * p2 + v * p3);
                }
            }
            return points;
        }

        /***************************************************/

        public static List<Point> GetIntersections(this NurbCurve c, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();    // TODO GetIntersections(NurbsCurve, Mesh)
        }

        /***************************************************/

        public static List<Point> GetIntersections(this PolyCurve curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();    // TODO GetIntersections(PolyCurve, Mesh)
        }

        /***************************************************/

        public static List<Point> GetIntersections(this Polyline curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            List<Line> lines = curve.GetExploded() as List<Line>;
            List<Point> points = new List<Point>();
            for (int i = 0; i < lines.Count; i++)
            {
                points.AddRange(GetIntersections(lines[i], mesh));
            }
            return points;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<Point> IGetIntersections(this ICurve curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            return GetIntersections(curve as dynamic, mesh);
        }
    }
}
