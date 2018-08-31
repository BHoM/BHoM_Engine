using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BHE = BH.oM.Environment.Elements;
using BHG = BH.oM.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsClosed(BHE.Space space, double tolerance = BHG.Tolerance.MacroDistance)
        {
            return (BH.Engine.Environment.Query.UnmatchedElementPoints(space, tolerance).Count == 0);
        }
    }
}
