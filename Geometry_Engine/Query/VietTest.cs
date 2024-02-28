using BH.Engine.Geometry;
using BH.oM.Base.Attributes;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static Humanizer.In;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /****              Public methods               ****/
        /***************************************************/

        [Description("Checks if a Polyline intersects or contains a Line. Runs quicker than sequentially calling the 2 existing engine methods: LineIntersections and IsContaining).")]
        [Input("pLine", "A Polyline to check for geometric containment and intersection.")]
        [Input("line", "A Line to check for geometric containment and intersection.")]
        [Input("tolerance", "The tolerance to use during intersection and containment checks.")]
        [Output("bool", "True if the Polyline intersects or contains the Line.")]
        public static bool IntersectsOrContains(this Polyline pLine, Line line, double tolerance = Tolerance.Distance)
        {
            if (!pLine.IsClosed(tolerance))
                return false;

            List<Point> intPoints = pLine.LineIntersections(line, false, tolerance);

            if (intPoints.Count > 0)
                return true;

            intPoints = intPoints.CullDuplicates(tolerance);
            List<double> cParams = new List<double> { 0, 1 };

            foreach (Point iPt in intPoints)
            {
                cParams.Add(line.IParameterAtPoint(iPt, tolerance));
            }

            cParams.Sort();

            for (int i = 0; i < cParams.Count - 1; i++)
            {
                intPoints.Add(line.IPointAtParameter((cParams[i] + cParams[i + 1]) * 0.5));
            }

            return pLine.IsContaining(intPoints, true, tolerance);
        }

        [Description("Checks if a Polyline intersects or contains a Line. Runs quicker than sequentially calling the 2 existing engine methods: LineIntersections and IsContaining).")]
        [Input("pLine", "A Polyline to check for geometric containment and intersection.")]
        [Input("line", "A Line to check for geometric containment and intersection.")]
        [Input("tolerance", "The tolerance to use during intersection and containment checks.")]
        [Output("bool", "True if the Polyline intersects or contains the Line.")]
        public static bool IntersectsOrContains2(this Polyline pLine, Line line, double tolerance = Tolerance.Distance)
        {
            if (!pLine.IsClosed(tolerance))
                return false;

            Vector v1 = line.End - line.Start;
            Vector dir = v1.Normalise();
            Vector sideDir = new Vector { X = -dir.Y, Y = dir.X };

            // Collection of intersection parameters along infinite line
            // Coupled with direction of intersected subparts
            List<(double, Vector)> paramsWithDirs = new List<(double, Vector)>();
            
            List<Line> subParts = pLine.SubParts();
            foreach(Line subPart in  subParts)
            {
                Vector v2 = subPart.End - subPart.Start;
                Vector dir2 = v2.Normalise();

                if (1 - Math.Abs(dir.DotProduct(dir2)) <= tolerance)
                    continue;

                // Calculate parameters at which the lines intersect
                Point p1 = line.Start;
                Point p2 = subPart.Start;

                Vector cp = v1.CrossProduct(v2);
                Vector n1 = v1.CrossProduct(-cp);
                Vector n2 = v2.CrossProduct(cp);

                // If parameter on subpart inside range <0, 1>
                // We have an intersection of infinite line with finite subpart
                // Otherwise ignore, we are not interested in intersection with inifnite subparts
                double t2 = (p1 - p2) * n1 / (v2 * n1);
                if (t2 >= -tolerance && t2 <= 1 + tolerance)
                {
                    // If parameter on line inside range <0, 1>
                    // It is a physical intersection, so return immediately
                    double t1 = (p2 - p1) * n2 / (v1 * n2);
                    if (t1 >= -tolerance && t1 <= 1 + tolerance)
                        return true;

                    // Otherwise capture parameter along line coupled with direction of subpart
                    paramsWithDirs.Add((t1, dir2));
                }
            }

            if (paramsWithDirs.Count == 0)
                return false;

            // Take only intersection parameters 'before' start of the line
            // If the line is inside polygon, it needs to have at least 1 of such
            paramsWithDirs = paramsWithDirs.Where(x => x.Item1 < 0).OrderBy(x => x.Item1).ToList();
            if (paramsWithDirs.Count == 0)
                return false;

            double paramTol = tolerance / line.Length();
            
            // Count number of intersections
            int c = 0;
            for (int i = 0; i < paramsWithDirs.Count; i++)
            {
                // Ignore intersections with 2 subparts at 1 point, where both subparts are on the same side of line
                // This is the case where the line just scratches a convex corner
                if (i < paramsWithDirs.Count - 1 && paramsWithDirs[i + 1].Item1 - paramsWithDirs[i].Item1 < paramTol)
                {
                    double from = paramsWithDirs[i].Item2.DotProduct(sideDir);
                    double to = paramsWithDirs[i + 1].Item2.DotProduct(sideDir);
                    if (from * to < 0)
                        continue;
                }

                c++;    
            }

            // Winding number check
            return c++ % 2 == 1;
        }

        /***************************************************/
    }
}