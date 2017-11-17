using BH.oM.Acoustic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static List<Ray> FilterRays(this List<Ray> rays, List<int> sourceFilter = null, List<int> targetFilter = null, List<int> panelFilter = null)
        {
            if (sourceFilter != null) { rays = rays.FindAll(ray => sourceFilter.Contains(ray.SpeakerID)); }
            if (targetFilter != null) { rays = rays.FindAll(ray => targetFilter.Contains(ray.ReceiverID)); }
            if (panelFilter  != null) { rays = rays.Where(r => r.PanelsID.Any(x => panelFilter.Contains(x))).ToList(); }
            return rays;
        }
    }
}
