using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine_Explore.BHoM.Geometry;
using Engine_Explore.Engine;

namespace Engine_Explore.Engine.Geometry
{
    public static class Intersect
    {
        public static Point PlaneLine(Plane plane, Line line, bool finiteLineSegment = true, double tolerance = 0.0001)
        {
            Vector dir = Transform.Normalise(line.End - line.Start);

            //Return null if parallel
            if (Math.Abs(Measure.DotProduct(dir, plane.Normal)) < tolerance)
                return null; 

            double t = (Measure.DotProduct(plane.Normal, (plane.Origin - line.Start))) / (Measure.DotProduct(plane.Normal, dir));

            // Return null if intersection out of segment limits
            if (finiteLineSegment && (t < 0 || t > 1))
                return null;

            return line.Start + t * dir;
        }

        /***************************************************/

        public static Point LineLine(Line line1, Line line2, bool finiteLineSegment = true, double tolerance = 0.0001)
        {
            Point s1 = line1.Start;
            Point s2 = line2.Start;
            Vector d1 = Transform.Normalise(line1.End - s1);
            Vector d2 = Transform.Normalise(line2.End - s2);

            Vector cross = Measure.CrossProduct(d1, d2);
 
            // Test for parallel lines
            if (cross.X < tolerance && cross.X > -tolerance && cross.Y < tolerance && cross.Y > -tolerance && cross.Z < tolerance && cross.Z > -tolerance)
            {
                if (finiteLineSegment)
                {
                    return null; // TODO: check for matching end/start points
                }
                else
                    return null; 
            }

            double t = Measure.DotProduct(cross, Measure.CrossProduct(s2 - s1, d2)) / Measure.Length(cross);

            if (finiteLineSegment && t > -tolerance && t < Measure.Length(d1) + tolerance)
                return s1 + t * d1;
            else return null;
        }

        /***************************************************/

        public static List<Point> Lines(List<Line> lines, bool finiteLineSegment = true, double tolerance = 0.0001)
        {
            // Get the bounding boxes
            List<BoundingBox> boxes = lines.Select(x => Bound.Calculate(x)).ToList();

            // Get the intersections
            List<Point> intersections = new List<Point>();
            for (int i = lines.Count - 1; i >= 0 ; i--)     // We should use an octoTree/point matrix instead of using bounding boxes
            {
                for (int j = lines.Count - 1; j > i; j--)
                {
                    if (Bound.InRange(boxes[i], boxes[j]))
                    {
                        Point result = Intersect.LineLine(lines[i], lines[j], finiteLineSegment, tolerance);
                        if (result != null)
                            intersections.Add(result);
                    }
                }
            }

            return intersections;
        }
    }
}
