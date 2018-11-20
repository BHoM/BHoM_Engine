using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BH.oM.Environment.Elements;
using BH.oM.Geometry;

using BH.Engine.Geometry;

namespace BH.Engine.Environment
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsClosed(Space space, double tolerance = Tolerance.MacroDistance)
        {
            return (BH.Engine.Environment.Query.UnmatchedElementPoints(space, tolerance).Count == 0);
        }

        public static bool IsClosed(this List<BuildingElement> space, double tolerance = Tolerance.Distance)
        {
            //Check that each edge is connected to at least one other edge
            List<Line> edgeParts = space.Edges();
            List<Line> unique = edgeParts.Distinct().ToList();

            foreach(Line l in unique)
            {
                if(edgeParts.Where(x => x.BooleanIntersection(l) != null).ToList().Count < 2)
                    return false;
            }

            return true;
        }
    }
}
