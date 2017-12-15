using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Acoustic;
using BH.Engine.Geometry;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static List<Ray> VisibleRays(this List<Ray> rays, List<Panel> panels)
        {
            if (panels == null) { return rays; }
            return rays.Where(ray => !ray.IsObstructed(panels)).ToList();
        }
    }
}
