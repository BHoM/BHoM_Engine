using BH.oM.Geometry;
using BH.oM.Architecture.Elements;

namespace BH.Engine.Architecture
{
    public static partial class Modify
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Grid SetGeometry(this Grid grid, ICurve curve)
        {
            Grid clone = grid.GetShallowClone() as Grid;
            clone.Curve = curve;
            return clone;
        }

        /***************************************************/
    }
}
