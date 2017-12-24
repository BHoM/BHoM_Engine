using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Geometry
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static PolyCurve PolyCurve(IEnumerable<ICurve> curves)
        {
            return new PolyCurve { Curves = curves.ToList() };
        }

        /***************************************************/
    }
}
