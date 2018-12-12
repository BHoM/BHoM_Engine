using BH.oM.Geometry;
using System;
using System.Linq;
using System.Collections.Generic;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsOnCurve(this Line line, Point pt, double tolerance = Tolerance.Distance)
        {
            double distToStart = Query.Distance(pt, line.Start);
            double distToEnd = Distance(line.End, pt);

            double maxTol = (distToStart + distToEnd) + tolerance;
            double minTol = (distToStart + distToEnd) - tolerance;

            return line.Length() >= minTol && line.Length() <= maxTol;
        }

        /***************************************************/
    }
}