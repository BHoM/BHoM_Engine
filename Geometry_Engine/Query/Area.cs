using BH.oM.Geometry;
using System;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static double Area(this Arc curve)
        {
            return curve.IsClosed() ? curve.Angle() * Math.Pow(curve.Radius(), 2) : 0;
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
            if (!curve.IsClosed()) return 0;
            Plane p = curve.FitPlane();
            if (p == null) return 0.0;              // points are collinear

            bool isClockwise = curve.IsClockwise(p.Normal);
            Point sPt = curve.StartPoint();
            double area = 0;
            foreach (ICurve c in curve.SubParts())
            {
                if (c is NurbCurve) throw new NotImplementedException();

                Point ePt = c.IEndPoint();
                Vector prod = CrossProduct(sPt - p.Origin, ePt - p.Origin);
                if (isClockwise) area += prod * p.Normal * 0.5;
                else area -= prod * p.Normal * 0.5;

                if (c is Arc)
                {
                    Arc arc = c as Arc;
                    double radius = arc.Radius();
                    double angle = arc.Angle();
                    double arcArea = (angle - Math.Sin(angle)) * radius * radius * 0.5;
                    if (arc.CoordinateSystem.Z.DotProduct(p.Normal) > 0 == isClockwise) area += arcArea;
                    else area -= arcArea;
                }

                sPt = ePt.Clone();
            }
            return area;
        }

        /***************************************************/

        public static double Area(this Polyline curve)
        {
            if (!curve.IsClosed()) return 0;

            List<Point> pts = curve.ControlPoints;
            int ptsCount = pts.Count;
            if (ptsCount < 4) { return 0.0; }

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

        public static double IArea(this IGeometry geometry)
        {
            return Area(geometry as dynamic);
        }

        /***************************************************/
    }
}
