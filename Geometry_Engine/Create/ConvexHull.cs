using BH.oM.Geometry;
using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        //TODO: Only works for points in the XY plane - add plane as input?

        [Description("Creates a Convex Hull from a list of points. Currently only works for points in the XY plane")]
        public static Polyline ConvexHull(List<Point> points)
        {
            List<Point> hull = new List<Point>();
            hull.Add(points.First());
            for(int x = 1; x < points.Count; x++)
            {
                if (hull[0].X > points[x].X)
                    hull[0] = points[x];
                else if (hull[0].X == points[x].X)
                {
                    if (hull[0].Y > points[x].Y)
                        hull[0] = points[x];
                }
            }

            Point nextPt = new Point();
            int counter = 0;
            while (counter < hull.Count)
            {
                nextPt = NextHullPoint(points, hull[counter]);
                if (nextPt != hull[0])
                    hull.Add(nextPt);
                counter++;
            }

            hull.Add(hull[0]);

            Polyline hullBoundary = new Polyline() { ControlPoints = hull };
            return hullBoundary;
        }

        public static Point NextHullPoint(List<Point> points, Point currentPt)
        {
            int right = -1;
            int none = 0;

            Point nextPt = currentPt;
            int t;
            foreach (Point pt in points)
            {
                t = ((nextPt.X - currentPt.X) * (pt.Y - currentPt.Y) - (pt.X - currentPt.X) * (nextPt.Y - currentPt.Y)).CompareTo(0);
                if (t == right || t == none && Geometry.Query.Distance(currentPt, pt) > Geometry.Query.Distance(currentPt, nextPt))
                    nextPt = pt;
            }

            return nextPt;
        }
    }
}