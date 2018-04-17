using BH.oM.Geometry;
using System;
using System.Linq;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Modify
    {
        /***************************************************/
        /****          Split curve at points            ****/
        /***************************************************/

        public static List<Arc> SplitAtPoints(this Arc arc, List<Point> points)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static List<Circle> SplitAtPoints(this Circle circle, List<Point> points)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static List<Line> SplitAtPoints(this Line line, List<Point> points, double tolerance = Tolerance.Distance)
        {
            List<Line> result = new List<Line>();
            List<Point> cPts = new List<Point> { line.Start, line.End };
            double sqTol = tolerance * tolerance;
            foreach (Point point in points)
            {
                if (point.SquareDistance(line.Start) > sqTol && point.SquareDistance(line.End) > sqTol && point.SquareDistance(line) <= sqTol) cPts.Add(point);
            }

            if (cPts.Count > 2)
            {
                cPts = cPts.CullDuplicates(tolerance);
                cPts = cPts.SortAlongCurve(line, tolerance);
                for (int i = 0; i < cPts.Count - 1; i++)
                {
                    result.Add(new Line { Start = cPts[i], End = cPts[i + 1] });
                }
            }
            else result.Add(line.Clone());
            return result;
        }

        /***************************************************/

        public static List<NurbCurve> SplitAtPoints(this NurbCurve curve, List<Point> points)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static List<PolyCurve> SplitAtPoints(this PolyCurve curve, List<Point> points)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static List<Polyline> SplitAtPoints(this Polyline curve, List<Point> points, double tolerance = Tolerance.Distance)
        {
            if (points.Count == 0) return new List<Polyline> { curve.Clone() };
            List<Polyline> result = new List<Polyline>();
            List<Line> segments = curve.SubParts();
            Polyline section = new Polyline {ControlPoints = new List<Point>() };
            bool closed = curve.IsClosed(tolerance);
            double sqTol = tolerance * tolerance;
            for (int i = 0; i < segments.Count; i++)
            {
                Line l = segments[i];
                bool intStart = false;
                List<Point> iPts = new List<Point>();
                foreach (Point point in points)
                {
                    if (point.SquareDistance(l.Start) <= sqTol)
                    {
                        intStart = true;
                        if (i == 0) closed = false;
                    }
                    else if (point.SquareDistance(l.End) > sqTol && point.SquareDistance(l) <= sqTol) iPts.Add(point);
                }
                section.ControlPoints.Add(l.Start);
                if (intStart && section.ControlPoints.Count > 1)
                {
                    result.Add(section);
                    section = new Polyline { ControlPoints = new List<Point> { l.Start.Clone() } };
                }
                
                if (iPts.Count > 0)
                {
                    iPts = iPts.CullDuplicates(tolerance);
                    iPts = iPts.SortAlongCurve(l, tolerance);
                    foreach (Point iPt in iPts)
                    {
                        section.ControlPoints.Add(iPt);
                        result.Add(section);
                        section = new Polyline { ControlPoints = new List<Point> { iPt } };
                    }
                }
            }
            section.ControlPoints.Add(curve.ControlPoints.Last());
            result.Add(section);

            if (result.Count > 1 && closed)
            {
                result[0].ControlPoints.RemoveAt(0);
                result[0].ControlPoints.InsertRange(0, result.Last().ControlPoints);
                result.RemoveAt(result.Count - 1);
            }
            return result;
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static bool ISplitAtPoints(this ICurve curve, List<Point> points)
        {
            return SplitAtPoints(curve as dynamic, points);
        }
    }
}
