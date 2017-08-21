using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Arc CreateArc(Plane p, Point start, Point end)
        {
            start = start.GetProjected(p) as Point;
            end = end.GetProjected(p) as Point;

            Point centre = p.Origin;
            Vector midDir = ((start + end) / 2 - centre).GetNormalised();
            double midRadius = (start.GetDistance(centre) + end.GetDistance(centre));

            if (Measure.GetCrossProduct(start - centre, end - centre).GetDotProduct(p.Normal) < 0)
                midDir = -midDir;

            return new Arc(start, centre + midRadius * midDir, end);
        }
    }
}
