using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Measure
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Point GetClosestPoint(this IBHoMGeometry geometry, Point point)
        {
            return _GetClosestPoint(geometry as dynamic, point);
        }

        /***************************************************/

        public static Point GetClosestPoint(this Point point, IEnumerable<Point> cloud)
        {
            double minDist = Double.PositiveInfinity;
            double dist = 0;
            Point cp = null;
            foreach (Point pt in cloud)
            {
                dist = GetDistance(point, pt);
                if (dist < minDist)
                {
                    minDist = dist;
                    cp = pt;
                }
            }
            return cp;
        }

        /***************************************************/
        /**** Private Methods - Vectors                 ****/
        /***************************************************/

        private static Point _GetClosestPoint(this Point pt, Point point)
        {
            return pt;
        }

        /***************************************************/

        private static Point _GetClosestPoint(this Vector vector, Point point)
        {
            return null;
        }

        /***************************************************/

        private static Point _GetClosestPoint(this Plane plane, Point point)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Private Methods - Curves                  ****/
        /***************************************************/

        private static Point _GetClosestPoint(this Arc arc, Point point)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static Point _GetClosestPoint(this Circle circle, Point point)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static Point _GetClosestPoint(this Line line, Point point)
        {
            Vector dir = line.GetDirection();
            double t = Math.Min(Math.Max(dir * (point - line.Start), 0), line.GetLength());
            return line.Start + t * dir;
        }

        /***************************************************/

        private static Point _GetClosestPoint(this NurbCurve curve, Point point)
        {
            throw new NotImplementedException();
        }


        /***************************************************/

        private static Point _GetClosestPoint(this PolyCurve curve, Point point)
        {
            double minDist = 1e10;
            Point closest = null;
            List<ICurve> curves = curve.Curves;

            for (int i = 0; i < curves.Count; i++)
            {
                Point cp = curve.Curves[i].GetClosestPoint(point);
                double dist = cp.GetDistance(point);
                if (dist < minDist)
                {
                    closest = cp;
                    minDist = dist;
                }
            }

            return closest;
        }

        /***************************************************/

        private static Point _GetClosestPoint(this Polyline curve, Point point)
        {
            List<Point> points = curve.ControlPoints;

            double minDist = 1e10;
            Point closest = (points.Count > 0) ? points[0] : new Point(Double.PositiveInfinity, Double.PositiveInfinity, Double.PositiveInfinity);
            for (int i = 1; i < points.Count; i++)
            {
                Vector dir = (points[i] - points[i - 1]).GetNormalised();
                double t = Math.Min(Math.Max(dir * (point - points[i - 1]), 0), points[i].GetDistance(points[i - 1]));
                Point cp = points[i - 1] + t * dir;

                double dist = cp.GetSquareDistance(point);
                if (dist < minDist)
                {
                    closest = cp;
                    minDist = dist;
                }
            }
            return closest;
        }


        /***************************************************/
        /**** Private Methods - Surfaces                ****/
        /***************************************************/

        private static Point _GetClosestPoint(this Extrusion surface, Point point)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static Point _GetClosestPoint(this Loft surface, Point point)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static Point _GetClosestPoint(this NurbSurface surface, Point point)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static Point _GetClosestPoint(this Pipe surface, Point point)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static Point _GetClosestPoint(this PolySurface surface, Point point)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Private Methods - Others                  ****/
        /***************************************************/

        private static Point _GetClosestPoint(this Mesh mesh, Point point)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        private static Point _GetClosestPoint(this GeometryGroup group, Point point)
        {
            throw new NotImplementedException();
        }
    }
}
