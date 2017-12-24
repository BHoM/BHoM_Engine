using System.Collections.Generic;
using System.Linq;
using BH.oM.Acoustic;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static List<Ray> FilterVisibleRays(this List<Ray> rays, List<Panel> panels)
        {
            if (panels == null) { return rays; }
            return rays.Where(ray => !ray.IsObstructed(panels)).ToList();
        }

        /***************************************************/
    }
}
