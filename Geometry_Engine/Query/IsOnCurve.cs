using BH.oM.Geometry;
using System;
using System.Linq;
using System.Collections.Generic;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsOnCurve(this Line line, Point pt, double tolerance = Tolerance.Distance)
        {
            double distToStart = BH.Engine.Geometry.Query.Distance(pt, line.Start);
            double distToEnd = BH.Engine.Geometry.Query.Distance(line.End, pt);

            double maxTol = (distToStart + distToEnd) + tolerance;
            double minTol = (distToStart + distToEnd) - tolerance;

            if (line.Length() >= minTol && line.Length() <= maxTol)
                return true;
            return false;
        }
    }
}