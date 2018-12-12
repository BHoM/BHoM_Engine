using System.Linq;
using System.Collections.Generic;
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

        public static List<BuildingElement> CleanSpace(this List<BuildingElement> space)
        {
            //Remove elements which have 1 or less connections with other elements
            List<BuildingElement> cleanSpace = new List<BuildingElement>();

            List<Line> allEdges = space.Edges();

            foreach(BuildingElement be in space)
            {
                List<Line> edges = be.Edges();
                bool addSpace = true;
                foreach (Line l in edges)
                {
                    if (allEdges.Where(x => x.BooleanIntersection(l) != null).ToList().Count < 2)
                        addSpace = false;
                }

                if(addSpace)
                    cleanSpace.Add(be);
            }

            cleanSpace = cleanSpace.CullDuplicates();

            return cleanSpace;
        }
    }
}