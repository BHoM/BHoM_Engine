using BH.oM.Acoustic;
using BH.oM.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Acoustic
{
    public static partial class Verify
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static bool IsObstructed(this Ray ray, List<Panel> surfaces)
        {
            for (int i = 0; i < surfaces.Count; i++)
                if (Geometry.Query.GetIntersections(ray.Geometry, surfaces[i].Geometry).Count == 0)
                    return true;
            return false;
        }
    }
}
