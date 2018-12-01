using BH.oM.Environment.Elements;
using BH.oM.Geometry;
using BH.oM.Common;
using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IElement1D NewElement1D(this Opening opening, ICurve curve)
        {
            return curve.IClone();
        }

        /***************************************************/

        public static IElement1D NewElement1D(this Panel panel, ICurve curve)
        {
            return curve.IClone();
        }

        /***************************************************/
    }
}
