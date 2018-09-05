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
            List<Point> controlPoints = pline.ControlPoints.CullDuplicates();
            Line line = new Line();

            Line segment = new Line();

            for (int i = 0; i < controlPoints.Count() - 1; i++)
            {
                segment = BH.Engine.Geometry.Create.Line(controlPoints[i], controlPoints[i + 1]);
                if (BH.Engine.Geometry.Query.IsContaining(segment, pt))
                    line = segment;
            }

            segment = BH.Engine.Geometry.Create.Line(controlPoints.Last(), controlPoints[0]);
            if (BH.Engine.Geometry.Query.IsContaining(segment, pt))
                line = segment;

            return line;
        }
    }
}