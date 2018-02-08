using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Compute
    {
        /***************************************************/
        /****          public Methods - Lines           ****/
        /***************************************************/

        public static Line BooleanIntersection(this Line line, Line refLine)
        {
            if (line == null || line.Length() <= Tolerance.Distance || refLine.Length() <= Tolerance.Distance) return null;
            if (line.IsColinear(refLine))
            {
                List<Line> splitLine = line.SplitAtPoints(refLine.ControlPoints());
                if (splitLine.Count == 3)
                {
                    return splitLine[1];
                }
                else if (splitLine.Count == 2)
                {
                    Point aPt = refLine.ControlPoints().Average();
                    if (line.Start.SquareDistance(aPt) < line.End.SquareDistance(aPt)) return splitLine[0];
                    return splitLine[1];
                }
                else
                {
                    Point aPt = splitLine[0].ControlPoints().Average();
                    Point aRPt = refLine.ControlPoints().Average();
                    if (aRPt.SquareDistance(splitLine[0]) > Tolerance.SqrtDist && aPt.SquareDistance(refLine) > Tolerance.SqrtDist) return null;
                    return line.Clone();
                }
            }
            return null;
        }

        /***************************************************/

        public static List<Line> BooleanIntersection(this Line line, List<Line> refLines)
        {
            List<Line> result = new List<Line>();
            if (line.Length() <= Tolerance.Distance) return result;
            foreach (Line l in refLines)
            {
                Line intersection = line.BooleanIntersection(l);
                if (intersection != null) result.Add(intersection);
            }
            return result;
        }

        /***************************************************/

        public static Line BooleanIntersection(this List<Line> lines)
        {
            if (lines[0].Length() <= Tolerance.Distance) return null;
            Line result = lines[0].Clone();
            for (int i = 1; i < lines.Count; i++)
            {
                result = result.BooleanIntersection(lines[i]);
            }
            return result;
        }
    }
}
