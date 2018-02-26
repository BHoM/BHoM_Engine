using BH.oM.Structural.Elements;
using System.Collections.Generic;
using BH.oM.Structural.Design;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static StructuralLayout StructuralLayout(IEnumerable<Bar> bars)
        {
            return new StructuralLayout { AnalyticBars = bars.ToList() };
        }

        /***************************************************/
    }
}
