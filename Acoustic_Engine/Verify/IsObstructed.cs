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

        public static bool IsObstructed(Ray ray, List<Panel> surfaces, bool ClearRays = true, double tol = Tolerance.Distance)
        {
            for (int i = 0; i < surfaces.Count; i++)       //foreach surface
            {
                if (Geometry.Query.GetIntersections(ray.Geometry, surfaces[i].Geometry) == null)    // if ray hits a surface
                {
                    return true;
                }
            }
            return false;
        }
    }
}
