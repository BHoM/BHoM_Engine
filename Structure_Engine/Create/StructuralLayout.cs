using BH.oM.Structure.Elements;
using System.Collections.Generic;
using BH.oM.Structure.Design;
using System.Linq;
using BH.oM.Reflection.Attributes;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        [NotImplemented]
        public static StructuralLayout StructuralLayout(IEnumerable<Bar> bars)
        {
            return new StructuralLayout { AnalyticBars = bars.ToList() };
        }

        /***************************************************/
    }
}
