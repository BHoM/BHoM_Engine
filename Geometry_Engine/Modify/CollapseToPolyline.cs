using BH.oM.Geometry;
using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods - Curves                   ****/
        /***************************************************/

        public static Polyline CollapseToPolyline(this Arc curve, double angleTolerance, int maxSegmentCount = 100)
        {
            return new Polyline { ControlPoints = curve.CollapseToPolylineVertices(angleTolerance, maxSegmentCount) };
        }

        /***************************************************/

        public static Polyline CollapseToPolyline(this Circle curve, double angleTolerance, int maxSegmentCount = 100)
        {
            return new Polyline { ControlPoints = curve.CollapseToPolylineVertices(angleTolerance, maxSegmentCount) };
        }

        /***************************************************/

        public static Polyline CollapseToPolyline(this Line curve, double angleTolerance, int maxSegmentCount = 100)
        {
            return new Polyline { ControlPoints = curve.CollapseToPolylineVertices(angleTolerance, maxSegmentCount) };
        }

        /***************************************************/

        public static Polyline CollapseToPolyline(this Polyline curve, double angleTolerance, int maxSegmentCount = 100)
        {
            return curve.Clone();
        }

        /***************************************************/

        public static Polyline CollapseToPolyline(this PolyCurve curve, double angleTolerance, int maxSegmentCount = 100)
        {
            List<Point> controlPoints = new List<Point> { curve.StartPoint() };
            foreach (ICurve c in curve.SubParts()) controlPoints.AddRange(c.ICollapseToPolylineVertices(angleTolerance, maxSegmentCount).Skip(1));
            return new Polyline { ControlPoints = controlPoints };
        }

        /***************************************************/

        [NotImplemented]
        public static Polyline CollapseToPolyline(this NurbCurve curve, double angleTolerance, int maxSegmentCount = 100)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static Polyline ICollapseToPolyline(this ICurve curve, double angleTolerance, int maxSegmentCount = 100)
        {
            return CollapseToPolyline(curve as dynamic, angleTolerance, maxSegmentCount);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static List<Point> CollapseToPolylineVertices(this Arc curve, double angleTolerance, int maxSegmentCount = 100)
        {
            int segmentCount = curve.CollapseToPolylineCount(angleTolerance, maxSegmentCount);
            double step = 1.0 / segmentCount;
            double param = step;
            List<Point> result = new List<Point> { curve.StartPoint() };
            for (int i = 0; i < segmentCount; i++)
            {
                result.Add(curve.PointAtParameter(param));
                param += step;
            }
            return result;
        }

        /***************************************************/

        private static int CollapseToPolylineCount(this Arc curve, double angleTolerance, int maxSegmentCount = 100)
        {
            double angle = curve.Angle();
            double factor = Math.Min(Math.PI * 0.25, Math.Max(angle * 0.5 / maxSegmentCount, angleTolerance));
            return System.Convert.ToInt32(Math.Ceiling(angle * 0.5 / factor));
        }

        /***************************************************/

        private static List<Point> CollapseToPolylineVertices(this Circle curve, double angleTolerance, int maxSegmentCount = 100)
        {
            int segmentCount = curve.CollapseToPolylineCount(angleTolerance, maxSegmentCount);
            double step = 1.0 / segmentCount;
            double param = step;
            List<Point> result = new List<Point> { curve.StartPoint() };
            for (int i = 0; i < segmentCount; i++)
            {
                result.Add(curve.PointAtParameter(param));
                param += step;
            }
            return result;
        }

        /***************************************************/

        private static int CollapseToPolylineCount(this Circle curve, double angleTolerance, int maxSegmentCount = 100)
        {
            double factor = Math.Min(Math.PI * 0.25, Math.Max(Math.PI / maxSegmentCount, angleTolerance));
            return System.Convert.ToInt32(Math.Ceiling(Math.PI / factor));
        }

        /***************************************************/

        private static List<Point> CollapseToPolylineVertices(this Line curve, double angleTolerance, int maxSegmentCount = 100)
        {
            return new List<Point> { curve.Start, curve.End };
        }

        /***************************************************/

        private static List<Point> CollapseToPolylineVertices(this Polyline curve, double angleTolerance, int maxSegmentCount = 100)
        {
            return curve.ControlPoints.Select(p => p.Clone()).ToList();
        }

        /***************************************************/

        private static List<Point> ICollapseToPolylineVertices(this ICurve curve, double angleTolerance, int maxSegmentCount = 100)
        {
            return CollapseToPolylineVertices(curve as dynamic, angleTolerance, maxSegmentCount);
        }

        /***************************************************/
    }
}
