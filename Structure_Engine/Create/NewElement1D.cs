using BH.oM.Structure.Elements;
using BH.oM.Geometry;
using BH.oM.Common;
using BH.Engine.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IElement1D NewElement1D(this Opening opening, ICurve curve)
        {
            return new Edge { Curve = curve.IClone() };
        }

        /***************************************************/

        public static IElement1D NewElement1D(this PanelPlanar panel, ICurve curve)
        {
            return new Edge { Curve = curve.IClone() };
        }

        /***************************************************/
    }
}
