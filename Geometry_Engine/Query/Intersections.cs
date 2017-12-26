using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<Point> Intersections(this List<Line> lines, bool useInfiniteLine = true, double tolerance = Tolerance.Distance)
        {
            // Get the bounding boxes
            List<BoundingBox> boxes = lines.Select(x => x.Bounds()).ToList();

            // Get the intersections
            List<Point> intersections = new List<Point>();
            for (int i = lines.Count - 1; i >= 0; i--)     // We should use an octoTree/point matrix instead of using bounding boxes
            {
                for (int j = lines.Count - 1; j > i; j--)
                {
                    if (Query.IsInRange(boxes[i], boxes[j]))
                    {
                        Point result = LineIntersection(lines[i], lines[j], useInfiniteLine, tolerance);
                        if (result != null)
                            intersections.Add(result);
                    }
                }
            }

            return intersections;
        }

        /***************************************************/
    }
}