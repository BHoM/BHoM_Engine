using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /**** public Methods - Vectors                  ****/
        /***************************************************/

        public static List<Point> SortAlongCurve(this List<Point> points, Arc arc)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static List<Point> SortAlongCurve(this List<Point> points, Circle circle)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static List<Point> SortAlongCurve(this List<Point> points, Line line, double distanceTolerance = Tolerance.Distance, double angleTolerance = Tolerance.Angle)
        {
            if (line.Length() <= distanceTolerance) return points.Select(p => p.Clone()).ToList();

            Vector lDir = line.Direction();
            List<Tuple<Point, Point>> cData = points.Select(p => new Tuple<Point, Point>(p.Clone(), (p.Project(line)))).ToList();
            
            if ((Math.Abs(lDir.X)) > angleTolerance)
            {
                cData.Sort(delegate (Tuple<Point, Point> d1, Tuple<Point, Point> d2)
                {
                    return d1.Item2.X.CompareTo(d2.Item2.X);
                });
                if (lDir.X < 0) cData.Reverse();
            }
            else if ((Math.Abs(lDir.Y)) > angleTolerance)
            {
                cData.Sort(delegate (Tuple<Point, Point> d1, Tuple<Point, Point> d2)
                {
                    return d1.Item2.Y.CompareTo(d2.Item2.Y);
                });
                if (lDir.Y < 0) cData.Reverse();
            }
            else
            {
                cData.Sort(delegate (Tuple<Point, Point> d1, Tuple<Point, Point> d2)
                {
                    return d1.Item2.Z.CompareTo(d2.Item2.Z);
                });
                if (lDir.Z < 0) cData.Reverse();
            }
            return cData.Select(d => d.Item1).ToList();
        }

        /***************************************************/

        public static List<Point> SortAlongCurve(this List<Point> points, NurbCurve curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static List<Point> SortAlongCurve(this List<Point> points, PolyCurve curve)
        {
            throw new NotImplementedException();
        }

        /***************************************************/

        public static List<Point> SortAlongCurve(this List<Point> points, Polyline curve)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<Point> ISortAlongCurve(this List<Point> points, ICurve curve)
        {
            return SortAlongCurve(points, curve as dynamic);
        }
    }
}
