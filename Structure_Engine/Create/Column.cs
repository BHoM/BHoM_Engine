using BH.oM.Structural.Elements;
using BH.oM.Geometry;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Column Column(ICurve locationCurve)
        {
            return new Column { LocationCurve = locationCurve };
        }

        /***************************************************/
    }
}
