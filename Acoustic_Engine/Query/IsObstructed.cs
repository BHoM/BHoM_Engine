using BH.oM.Acoustic;
using BH.oM.Geometry;
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

        public static bool IsObstructed(this Ray ray, List<Panel> panels)
        {
            for (int i = 0; i < panels.Count; i++)
                if (Engine.Geometry.Query.GetIntersections(ray.Geometry, panels[i].Geometry).Count == 0)
                    return true;
            return false;
        }

        /***************************************************/
    }
}
