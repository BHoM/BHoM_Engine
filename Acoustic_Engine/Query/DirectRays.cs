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

        public static Ray DirectRays(this Receiver target, Speaker source, List<Panel> surfaces)
        {
            Ray ray = Create.Ray(source, target);
            return Verify.IsObstructed(ray, surfaces) ? ray : default(Ray);
        }

        /***************************************************/

        public static List<Ray> DirectRays(this List<Receiver> targets, List<Speaker> sources, List<Panel> surfaces)
        {
            List<Ray> rays = new List<Ray>();
            for (int i = 0; i < sources.Count; i++)
                for (int j = 0; j < targets.Count; j++)
                    rays.Add(DirectRays(targets[j], sources[i], surfaces));
            return rays;
        }
    }
}
