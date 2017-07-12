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
        /**** Public Methods - With Line                ****/
        /***************************************************/

        public static Point GetIntersection(this Plane plane, Line line, bool useInfiniteLine = true, double tolerance = 0.0001)
        {
            useInfiniteLine &= line.Infinite;

            Vector dir = (line.End - line.Start).GetNormalised();

            //Return null if parallel
            if (Math.Abs(dir * plane.Normal) < tolerance)
                return null;

            double t = (plane.Normal * (plane.Origin - line.Start)) / (plane.Normal * dir);

            // Return null if intersection out of segment limits
            if (!useInfiniteLine && (t < 0 || t > 1))
                return null;

            return line.Start + t * dir;
        }

        /***************************************************/

        public static Point GetIntersection(this Line line1, Line line2, bool useInfiniteLine = true, double tolerance = 0.0001)
        {
            useInfiniteLine &= line2.Infinite;

            Point pt1 = line1.Start;
            Point pt2 = line2.Start;
            Vector dir1 = (line1.End - pt1).GetNormalised();
            Vector dir2 = (line2.End - pt2).GetNormalised();

            Vector cross = Measure.GetCrossProduct(dir1, dir2);

            // Test for parallel lines
            if (cross.X < tolerance && cross.X > -tolerance && cross.Y < tolerance && cross.Y > -tolerance && cross.Z < tolerance && cross.Z > -tolerance)
            {
                if (useInfiniteLine)
                    return null;
                else if (pt1 == pt2 || pt1 == line2.End)
                    return pt1;
                else if (pt2 == line1.End || line2.End == line1.End)
                    return line1.End;
                else
                    return null;
            }

            double t = Measure.GetDotProduct(cross, Measure.GetCrossProduct(pt2 - pt1, dir2)) / Measure.GetLength(cross);

            if (!useInfiniteLine && t > -tolerance && t < Measure.GetLength(dir1) + tolerance)
                return pt1 + t * dir1;
            else return null;
        }

        /***************************************************/

        public static List<Point> GetIntersections(this List<Line> lines, bool useInfiniteLine = true, double tolerance = 0.0001)
        {
            // Get the bounding boxes
            List<BoundingBox> boxes = lines.Select(x => x.GetBounds()).ToList();

            // Get the intersections
            List<Point> intersections = new List<Point>();
            for (int i = lines.Count - 1; i >= 0; i--)     // We should use an octoTree/point matrix instead of using bounding boxes
            {
                for (int j = lines.Count - 1; j > i; j--)
                {
                    if (Verify.IsInRange(boxes[i], boxes[j]))
                    {
                        Point result = GetIntersection(lines[i], lines[j], useInfiniteLine, tolerance);
                        if (result != null)
                            intersections.Add(result);
                    }
                }
            }

            return intersections;
        }
    }
}
