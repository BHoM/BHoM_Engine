using BH.oM.Geometry;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Line GetLineSegment(this Polyline pline, Point pt)
        {
            List<Line> pLineSegments = pline.SubParts();
            Line line = new Line();

            foreach (Line segment in pLineSegments)
            {
                if (BH.Engine.Geometry.Query.IsOnCurve(segment, pt))
                    line = segment;
            }

            return line;
        }
    }
}