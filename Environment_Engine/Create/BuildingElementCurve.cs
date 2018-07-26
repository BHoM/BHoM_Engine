using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static BuildingElementCurve BuildingElementCurve(ICurve curve)
        {
            return new BuildingElementCurve
            {
                Curve = curve
            };
        }

        /***************************************************/
    }
}
