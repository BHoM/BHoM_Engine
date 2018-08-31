using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BHG = BH.oM.Geometry;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static double LongestSegment(BHG.Polyline pLine)
        {
            List<BHG.Point> pts = pLine.DiscontinuityPoints();
            double length = pts.Last().Distance(pts.First());

            for(int x = 0; x < pts.Count - 1; x++)
            {
                double dist = pts[x].Distance(pts[x + 1]);
                length = dist > length ? dist : length;
            }

            return length;
        }
    }
}
