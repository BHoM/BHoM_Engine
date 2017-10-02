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

        public static Circle Circle(Point pt1, Point pt2, Point pt3)
        {
            Vector v1 = pt1 - pt3;
            Vector v2 = pt2 - pt3;
            Vector normal = v1.GetCrossProduct(v2).GetNormalised();

            Point centre = Query.GetIntersection(
                new Line(pt3 + v1 / 2, v1.GetCrossProduct(normal)),
                new Line(pt3 + v2 / 2, v2.GetCrossProduct(normal)),
                true
            );

            return new Circle(centre, normal, pt1.GetDistance(centre));
        }
    }
}
