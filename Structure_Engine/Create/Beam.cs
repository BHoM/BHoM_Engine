using BH.oM.Structural.Elements;
using BH.oM.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Beam Beam(ICurve locationCurve)
        {
            return new Beam { LocationCurve = locationCurve };
        }

        /***************************************************/
    }
}
