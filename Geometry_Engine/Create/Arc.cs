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

        public static Arc CreateArc(Point centre, Point start, Point end)
        {
            Vector v1 = start - centre;
            Vector v2 = end - centre;
            Vector normal = v1.GetCrossProduct(v2).GetNormalised();

            if (double.IsNaN(normal.X))
                normal = Vector.ZAxis;

            double angle = v1.GetSignedAngle(v2, normal);
            Vector midDir = ((Vector)v1.GetRotated(angle / 2, normal)).GetNormalised();
            double midRadius = (start.GetDistance(centre) + end.GetDistance(centre)) / 2;

            return new Arc(start, centre + midRadius * midDir, end);
             
        }
    }
}
