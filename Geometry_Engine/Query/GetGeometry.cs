using BH.oM.Geometry;

namespace BH.Engine.Geometry
{
    public static partial class Query
    {
        /***************************************************/
        /****               Public Methods              ****/
        /***************************************************/

        public static ICurve GetGeometry(this ICurve curve)
        {
            return curve.IClone();
        }

        /***************************************************/
    }
}
