using BH.oM.Structural.Elements;
using BH.oM.Geometry;
using BHE = BH.Engine.Geometry;
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
            // Todo:
            // - check if the edges are closed
            return new Opening { Edges = edges.Select(x => new Edge { Curve = x }).ToList() };
        }

        /***************************************************/

        public static Opening Opening(ICurve outline)
        {
            return BHE.Query.IIsClosed(outline) ? Opening(BHE.Query.ISubParts(outline)) : null;
        }
    }
}
