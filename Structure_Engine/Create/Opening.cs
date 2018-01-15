using BH.oM.Structural.Elements;
using BH.oM.Geometry;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Create 
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Opening Opening(IEnumerable<ICurve> edges)
        {
            return new Opening { Edges = edges.ToList() };
        }

        /***************************************************/
    }
}
