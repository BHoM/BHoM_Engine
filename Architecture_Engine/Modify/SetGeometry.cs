using BH.Engine.Geometry;
using BH.oM.Architecture.Elements;
using BH.oM.Geometry;

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
            clone.Curve = curve.IClone();
            return clone;
        }

        /***************************************************/
    }
}
