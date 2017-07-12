using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Measure
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Point GetCentre(this Arc arc)
        {
            Vector v1 = arc.Start - arc.Middle;
            Vector v2 = arc.End - arc.Middle;
            Vector normal = v1.GetCrossProduct(v2);

            return Measure.GetIntersection(
                new Line(arc.Middle + v1 / 2, v1.GetCrossProduct(normal)),
                new Line(arc.Middle + v2 / 2, v2.GetCrossProduct(normal))
            );
        }

        /***************************************************/

        public static double GetRadius(this Arc arc)
        {
            Point centre = arc.GetCentre();
            if (centre != null)
                return centre.GetDistance(arc.Start);
            else
                return 0; 
        }
    }
}
