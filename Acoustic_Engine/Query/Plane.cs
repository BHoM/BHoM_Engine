using BH.oM.Acoustic;
using BH.oM.Geometry;
using System.Collections.Generic;

namespace BH.Engine.Acoustic
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public  Methods                           ****/
        /***************************************************/

        public static Plane Plane(this Panel panel)
        {
            List<Point> Ver = panel.Surface.Vertices;
            return BH.Engine.Geometry.Create.Plane(Ver[0], Ver[1], Ver[2]);
        }

        /***************************************************/
    }
}
