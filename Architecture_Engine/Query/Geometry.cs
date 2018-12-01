using BH.oM.Architecture.Elements;
using BH.oM.Geometry;

namespace BH.Engine.Architecture
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static ICurve Geometry(this Grid grid)
        {
            return grid.Curve;
        }

        /***************************************************/
    }
}
