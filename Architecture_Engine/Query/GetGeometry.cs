using BH.oM.Architecture.Elements;
using BH.oM.Geometry;

namespace BH.Engine.Architecture
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ICurve GetGeometry(this Grid grid)
        {
            return grid.Curve;
        }

        /***************************************************/
    }
}
