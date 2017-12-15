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


        public static List<Ray> DirectRays(this List<Receiver> receivers, List<Speaker> speakers, List<Panel> panels = null)
        {
            List<Ray> rays = new List<Ray>();
            for (int i = 0; i < speakers.Count; i++)
                for (int j = 0; j < receivers.Count; j++)
                    rays.Add(Create.Ray(speakers[i], receivers[i]));
            return rays.VisibleRays(panels);
        }
    }
}
