using BH.oM.Geometry;
using System;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Point> LineIntersections(this Line line1, Line line2, double tolerance = Tolerance.Distance)
        {
            return new List<Point> { line1.LineIntersection(line2, false, tolerance) };
        }

        /***************************************************/

        public static Point LineIntersection(this Line line1, Line line2, bool useInfiniteLines = false, double tolerance = Tolerance.Distance)
        {
            Point pt1 = line1.Start;
            Point pt2 = line2.Start;
            Vector dir1 = (line1.End - pt1);//.Normalise();
            Vector dir2 = (line2.End - pt2);//.Normalise();
            Vector dir3 = pt2 - pt1;

            Vector cross = Query.CrossProduct(dir1, dir2);

            if (Math.Abs(dir3.DotProduct(cross)) > Tolerance.Distance)
                return null; // Lines are not coplanar

            // Test for parallel lines
            if (cross.X < tolerance && cross.X > -tolerance && cross.Y < tolerance && cross.Y > -tolerance && cross.Z < tolerance && cross.Z > -tolerance)
            {
                if (useInfiniteLines || line1.Infinite || line2.Infinite)
                    return null;
                else if (pt1 == pt2 || pt1 == line2.End)
                    return pt1;
                else if (pt2 == line1.End || line2.End == line1.End)
                    return line1.End;
                else
                    return null;
            }

            double t = Query.DotProduct(Query.CrossProduct(dir3, dir2), cross) / Query.SquareLength(cross);

            if (useInfiniteLines)  //TODO: Need to handle the cases where one of the line is Infinite as well
                return pt1 + t * dir1;
            else
            {
                double s = Query.DotProduct(Query.CrossProduct(dir3, dir1), cross) / Query.SquareLength(cross);
                if (t > -tolerance && t < 1 + tolerance && s > -tolerance && s < 1 + tolerance)
                    return pt1 + t * dir1;
                else
                    return null;
            }

            //if (useInfiniteLines)  //TODO: Need to handle the cases where one of the line is Infinite as well
            //    return pt1 + t * dir1;
            //else if (t > -tolerance && t < 1 /*Query.Length(dir1)*/ + tolerance)
            //    return pt1 + t * dir1;
            //else return null;
        }

        /***************************************************/

        public static List<Point> LineIntersections(this Polyline curve, Line line, double tolerance = Tolerance.Distance)
        {
            throw new NotImplementedException();
        }


        /***************************************************/
        /**** Public Methods - Interfaces               ****/
        /***************************************************/

        public static List<Point> ILineIntersections(this ICurve curve1, Line line, double tolerance = Tolerance.Distance)
        {
            return LineIntersections(curve1 as dynamic, line, tolerance);
        }

        /***************************************************/
    }
}