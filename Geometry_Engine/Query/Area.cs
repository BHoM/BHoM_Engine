using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static double Area(this Arc curve)
        {
            return curve.Angle() * Math.Pow(curve.Radius(), 2);
        }

        /***************************************************/

        public static double Area(this Circle curve)
        {
            return Math.PI * Math.Pow(curve.Radius, 2);
        }

        /***************************************************/

        public static double Area(this Line curve)
        {
            return 0;
        }

        /***************************************************/

        public static double Area(this NurbCurve curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static double Area(this PolyCurve curve)
        {
            return curve.Curves.Sum(crv => crv.IArea());
        }

        /***************************************************/

        public static double Area(this Polyline curve)
        {
            List<Point> pts = curve.ControlPoints;
            int ptsCount = pts.Count;
            if (ptsCount < 3) { return 0.0; }

            Plane p = pts.FitPlane();
            if (p == null) return 0.0;              // points are collinear

            double x = 0, y = 0, z = 0;
            for (int i = 0; i < ptsCount; i++)
            {
                int j = (i + 1) % ptsCount;
                Vector prod = CrossProduct(pts[i] - p.Origin, pts[j] - p.Origin);
                x += prod.X;
                y += prod.Y;
                z += prod.Z;
            }
            return Math.Abs((new Vector { X = x, Y = y, Z = z } * p.Normal) * 0.5);
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static double Area(this Mesh mesh)
        {
            Mesh tMesh = mesh.Triangulate();
            double area = 0;
            List<Face> faces = tMesh.Faces;
            List<Point> vertices = tMesh.Vertices;
            for (int i = 0; i < faces.Count; i++)
            {
                Point pA = vertices[faces[i].A];
                Point pB = vertices[faces[i].B];
                Point pC = vertices[faces[i].C];
                Vector AB = new Vector { X = pB.X - pA.X, Y = pB.Y - pA.Y, Z = pB.Z - pA.Z };
                Vector AC = new Vector { X = pC.X - pA.X, Y = pC.Y - pA.Y, Z = pC.Z - pA.Z };
                area += AB.CrossProduct(AC).Length();
            }
            return area / 2;
        }

        /***************************************************/

        public static double Area(this NurbSurface nurbs)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static double IArea(this IBHoMGeometry geometry)
        {
            return Area(geometry as dynamic);
        }

        /***************************************************/
    }
}
