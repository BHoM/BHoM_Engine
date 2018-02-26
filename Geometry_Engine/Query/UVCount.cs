using System.Collections.Generic;
using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static List<int> UVCount(this NurbSurface surf)
        {
            List<int> degrees = surf.Degrees();            
            return new List<int> { surf.UKnots.Count - degrees[0] + 1, surf.VKnots.Count - degrees[1] + 1 };
        }

        /***************************************************/
    }
}
