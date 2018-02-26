using BH.oM.Geometry;
using System;
using System.Linq;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods - BoundingBox              ****/
        /***************************************************/

        public static bool IsContaining(this BoundingBox box1, BoundingBox box2)
        {
            return (box1.Min.X <= box2.Min.X && box1.Min.Y <= box2.Min.Y && box1.Min.Z <= box2.Min.Z && box1.Max.X >= box2.Max.X && box1.Max.Y >= box2.Max.Y && box1.Max.Z >= box2.Max.Z);
        }

        /***************************************************/

        public static bool IIsContaining(this BoundingBox box, Point pt)
        {
            Point max = box.Max;
            Point min = box.Min;

            return (pt.X <= max.X && pt.X >= min.X && pt.Y <= max.Y && pt.Y >= min.Y && pt.Z <= max.Z && pt.Z >= min.Z);
        }

        /***************************************************/

        public static bool IsContaining(this BoundingBox box, IGeometry geometry)
        {
            return box.IsContaining(geometry.IBounds());
        }

        /***************************************************/
        /**** Public Methods - Curve                    ****/
        /***************************************************/

        public static bool IsContaining(this ICurve curve, ICurve other)
        {
            //if (curve.IsClosed())
            //{
            //    if (!curve.Bounds().IsContaining(other.Bounds()))
            //        return false;

            //    Plane p = curve.Plane();
            //    if (curve.IsInPlane(p) && other.IsInPlane(p))
            //    {
            //        List<Point> intersects = curve.Intersections(other);

            //        if (intersects.Count > 0) return false;

            //        Point start = other.StartPoint();
            //        return curve.Intersection(new Plane(start, start - p.Origin)) % 2 == 0;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            //else
            //{
            //    return false;
            //}
            //return true;

            throw new NotImplementedException();
        }

        /***************************************************/

        public static bool IsContaining(this ICurve curve, List<Point> points)
        {
            //Plane p = null;
            //if (!curve.IsNurbForm) curve.CreateNurbForm();
            //if (curve.IsClosed() && curve.TryPlane(out p))
            //{
            //    for (int i = 0; i < points.Count; i++)
            //    {
            //        if (p.InPlane(points[i], 0.001))
            //        {
            //            Vector direction = points[i] - p.Origin;
            //            Vector up = p.Normal;
            //            Plane cuttingPlane = new Plane(points[i], points[i] + direction, points[i] + up);
            //            List<Point> intersects = Intersect.PlaneCurve(cuttingPlane, curve, 0.001);
            //            intersects.Add(points[i]);
            //            intersects = PointUtils.RemoveDuplicates(intersects, 3);


                            // Bug: this does not work if the XYZ origin, plane origin and point are collinear! Use SortCollinear instead.
            //            intersects.Sort(delegate (Point p1, Point p2)
            //            {
            //                return ArrayUtils.DotProduct(p1, direction).CompareTo(ArrayUtils.DotProduct(p2, direction));
            //            });

            //            for (int j = 0; j < intersects.Count; j++)
            //            {
            //                if (j % 2 == 0 && intersects[j] == points[i]) return false;
            //            }
            //        }
            //        else return false;
            //    }
            //    return true;
            //}
            //return false;

            throw new NotImplementedException();
        }

        /***************************************************/

        public static bool IsContaining(this Polyline curve, List<Point> points, bool acceptOnEdge = true)
        {
            // Todo:
            // - to be replaced with a general method for a nurbs curve?
            // - this is very problematic for edge cases (cutting line going through a sharp corner, to be superseded?

            Plane p = curve.FitPlane();
            if (curve.IsClosed())
            {
                foreach (Point pt in points)
                {
                    if (pt.IsInPlane(p))
                    {
                        List<Point> intersects = curve.LineIntersections(new Line { Start = pt, End = p.Origin }, true); // what if the points are in exactly same spot?
                        if ((pt.ClosestPoint(intersects).SquareDistance(pt) <= Tolerance.SqrtDist))
                        {
                            if (acceptOnEdge) continue;
                            else return false;
                        }
                        intersects.Add(pt);
                        intersects = intersects.SortCollinear();
                        intersects = intersects.CullDuplicates();
                        for (int j = 0; j < intersects.Count; j++)
                        {
                            if (j % 2 == 0 && intersects[j] == pt) return false;
                        }
                    }
                    else return false;
                }
                return true;
            }
            return false;
        }

        /***************************************************/
    }
}
