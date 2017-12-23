using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static int IsParallel(this Vector v1, Vector v2, double tolerance = Tolerance.Angle)
        {
            double angle = v1.GetAngle(v2);

            return angle > Math.PI / 2 ? Math.PI - angle <= tolerance ? -1 : 0 : angle <= tolerance ? 1 : 0;
        }
    }
}
