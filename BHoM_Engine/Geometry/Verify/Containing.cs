using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Verify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsContaining(this BoundingBox box1, BoundingBox box2)
        {
            return (box1.Min.X <= box2.Min.X && box1.Min.Y <= box2.Min.Y && box1.Min.Z <= box2.Min.Z && box1.Max.X >= box2.Max.X && box1.Max.Y >= box2.Max.Y && box1.Max.Z >= box2.Max.Z);
        }

        /***************************************************/

        public static bool IsContaining(this BoundingBox box1, IBHoMGeometry geometry)
        {
            return _IsContaining(box1, geometry as dynamic);
        }

        /***************************************************/

        public static bool IsContaining(this ICurve curve, ICurve other)
        {
            //if (curve.IsClosed())
            //{
            //    if (!curve.GetBounds().IsContaining(other.GetBounds()))
            //        return false;

            //    Plane p = curve.GetPlane();
            //    if (curve.IsInPlane(p) && other.IsInPlane(p))
            //    {
            //        List<Point> intersects = curve.GetIntersections(other);

            //        if (intersects.Count > 0) return false;

            //        Point start = other.GetStartPoint();
            //        return curve.GetIntersection(new Plane(start, start - p.Origin)) % 2 == 0;
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
            //if (curve.IsClosed() && curve.TryGetPlane(out p))
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
        /**** Private Methods - Bounding Box            ****/
        /***************************************************/

        private static bool _IsContaining(this BoundingBox box, IBHoMGeometry geometry)
        {
            return box.IsContaining(geometry.GetBounds());
        }

        /***************************************************/

        private static bool _IsContaining(this BoundingBox box, Point pt)
        {
            Point max = box.Max;
            Point min = box.Min;

            return (pt.X <= max.X && pt.X >= min.X && pt.Y <= max.Y && pt.Y >= min.Y && pt.Z <= max.Z && pt.Z >= min.Z);
        }
    }
}
