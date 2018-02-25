using BH.oM.Geometry;
using System;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - Vectors                  ****/
        /***************************************************/

        public static Point ClosestPoint(this Point pt, List<Point> points)
        {
            // Temporary, PointMatrix to be used?!
            if (points.Count == 0) return null;
            double minDist = pt.SquareDistance(points[0]);
            Point cPt = points[0];
            for (int i = 1; i < points.Count; i++)
            {
                double dist = pt.SquareDistance(points[i]);
                if (dist <= minDist)
                {
                    cPt = points[i];
                    minDist = dist;
                }
            }
            return cPt.Clone();
        }

        /***************************************************/

        public static Point ClosestPoint(this Vector vector, Point point)
        {
            return null;
        }

        /***************************************************/

        public static Point ClosestPoint(this Plane plane, Point point)
        {
            return point.Project(plane);
        }


        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Point ClosestPoint(this Arc arc, Point point)
        {
            Point center = arc.Centre();
            Point midPoint = arc.PointAtParameter(0.5);
            Plane p = arc.FitPlane();

            Point onCircle = center + (point.Project(p) - center).Normalise() * (arc.Start-center).Length();
            double sqrd = midPoint.SquareDistance(arc.Start);
            return midPoint.SquareDistance(onCircle) <= sqrd ? onCircle : onCircle.ClosestPoint(new List<Point> { arc.Start, arc.End });
        }

        /***************************************************/

        public static Point ClosestPoint(this Circle circle, Point point)
        {
            Plane p = new Plane { Origin = circle.Centre, Normal = circle.Normal };
            return circle.Centre + (point.Project(p) - circle.Centre).Normalise() * circle.Radius;
        }

        /***************************************************/

        public static Point ClosestPoint(this Line line, Point point, bool infiniteSegment = false)
        {
            Vector dir = line.Direction();
            double t = dir * (point - line.Start);
            if (!infiniteSegment)
                t = Math.Min(Math.Max(t, 0), line.Length());

            return line.Start + t * dir;
        }

        /***************************************************/

        public static Point ClosestPoint(this NurbCurve curve, Point point)
        {
            throw new NotImplementedException();
        }


        /***************************************************/

        public static Point ClosestPoint(this PolyCurve curve, Point point)
        {
            double minDist = 1e10;
            Point closest = null;
            List<ICurve> curves = curve.Curves;

            for (int i = 0; i < curves.Count; i++)
            {
                Point cp = curve.Curves[i].IClosestPoint(point);
                double dist = cp.Distance(point);
                if (dist < minDist)
                {
                    closest = cp;
                    minDist = dist;
                }
            }

            return closest;
        }

        /***************************************************/

        public static Point ClosestPoint(this Polyline curve, Point point)
        {
            List<Point> points = curve.ControlPoints;

            double minDist = 1e10;
            Point closest = (points.Count > 0) ? points[0] : null;
            for (int i = 1; i < points.Count; i++)
            {
                Vector dir = (points[i] - points[i - 1]).Normalise();
                double t = Math.Min(Math.Max(dir * (point - points[i - 1]), 0), points[i].Distance(points[i - 1]));
                Point cp = points[i - 1] + t * dir;

                double dist = cp.SquareDistance(point);
                if (dist < minDist)
                {
                    closest = cp;
                    minDist = dist;
                }
            }
            return closest;
        }


        /***************************************************/
        /**** Public Methods - Surfaces                 ****/
        /***************************************************/

        public static Point ClosestPoint(this Extrusion surface, Point point)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static Point ClosestPoint(this Loft surface, Point point)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static Point ClosestPoint(this NurbSurface surface, Point point)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static Point ClosestPoint(this Pipe surface, Point point)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static Point ClosestPoint(this PolySurface surface, Point point)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Public Methods - Others                   ****/
        /***************************************************/

        public static Point ClosestPoint(this Mesh mesh, Point point)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static Point ClosestPoint(this CompositeGeometry group, Point point)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static Point ClosestPoint(this IEnumerable<Point> cloud, Point point)
        {
            double minDist = Double.PositiveInfinity;
            double dist = 0;
            Point cp = null;
            foreach (Point pt in cloud)
            {
                dist = Distance(point, pt);
                if (dist < minDist)
                {
                    minDist = dist;
                    cp = pt;
                }
            }
            return cp;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Point IClosestPoint(this IGeometry geometry, Point point)
        {
            return ClosestPoint(geometry as dynamic, point);
        }

        /***************************************************/
    }
}
