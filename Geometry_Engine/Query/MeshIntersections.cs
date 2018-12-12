using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [NotImplemented]
        public static Line MeshIntersection(this Plane p, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }
        
        /***************************************************/

        [NotImplemented]
        public static List<Point> MeshIntersections(this Arc curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        [NotImplemented]
        public static List<Point> MeshIntersections(this Circle curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static List<Point> MeshIntersections(this Line curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            // Outputs
            List<Point> points = new List<Point>();

            // Preprocessing Mesh
            Mesh tMesh = mesh.Triangulate();
            List<Face> faces = tMesh.Faces;
            List<Point> meshPts = tMesh.Vertices;

            for (int i = 0; i < faces.Count; i++)
            {
                // Mesh Points
                Point p1 = meshPts[faces[i].A];
                Point p2 = meshPts[faces[i].B];
                Point p3 = meshPts[faces[i].C];

                // Ray direction
                Vector d = curve.PointAtParameter(1) - curve.PointAtParameter(0);

                // Vectors from p1 to p2/p3 (edges)
                Vector e1, e2;

                Vector p, q, t;
                double det, invDet, u, v;

                //Find vectors for two edges sharing vertex/point p1
                e1 = p2 - p1;
                e2 = p3 - p1;

                // calculating determinant 
                p = Query.CrossProduct(d, e2);

                //Calculate determinat
                det = e1 * p;

                //if determinant is near zero, ray lies in plane of triangle otherwise not
                if (det > -tolerance && det < tolerance)
                    continue;

                invDet = 1.0f / det;

                //calculate distance from p1 to ray origin
                t = curve.PointAtParameter(0) - p1;

                //Calculate u parameter
                u = t * p * invDet;

                //Check for ray hit
                if (u < 0 || u > 1)
                    continue;

                //Prepare to test v parameter
                q = Query.CrossProduct(t, e1);

                //Calculate v parameter
                v = d * q * invDet;

                //Check for ray hit
                if (v < 0 || u + v > 1)
                    continue;

                if ((e2 * q * invDet) > Double.Epsilon)
                    points.Add((1 - u - v) * p1 + u * p2 + v * p3);                             //ray does intersect
            }
            return points;
        }

        /***************************************************/

        [NotImplemented]
        public static List<Point> MeshIntersections(this NurbCurve c, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();    // TODO Intersections(NurbsCurve, Mesh)
        }

        /***************************************************/

        [NotImplemented]
        public static List<Point> MeshIntersections(this PolyCurve curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();    // TODO Intersections(PolyCurve, Mesh)
        }

        /***************************************************/

        public static List<Point> MeshIntersections(this Polyline curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            List<Point> points = new List<Point>();
            foreach (Line line in curve.SubParts())
            {
                points.AddRange(MeshIntersections(line, mesh, tolerance));
            }
            return points;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<Point> IMeshIntersections(this ICurve curve, Mesh mesh, double tolerance = Tolerance.Distance)
        {
            return MeshIntersections(curve as dynamic, mesh);
        }

        /***************************************************/
    }
}