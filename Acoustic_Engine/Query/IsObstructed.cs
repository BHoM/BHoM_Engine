using BH.oM.Acoustic;
using System.Collections.Generic;

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
                if (Engine.Geometry.Query.GetIntersections(ray.Path, panels[i].Surface).Count == 0)
                    return true;
            return false;
        }

        /***************************************************/
    }
}
