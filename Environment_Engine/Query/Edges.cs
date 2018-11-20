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

        public static List<Line> Edges(this BuildingElement element)
        {
            return element.PanelCurve.ISubParts() as List<Line>;
        }

        public static List<Line> Edges(this List<BuildingElement> space)
        {
            List<Line> parts = new List<Line>();

            foreach (BuildingElement be in space)
                parts.AddRange(be.Edges());

            return parts;
        }

        public static bool EdgeIntersects(this Line edge, List<Line> possibleIntersections)
        {
            return possibleIntersections.Where(x => x.BooleanIntersection(edge) != null).ToList().Count > 0;
        }

        public static bool EdgeIntersects(this List<Line> edges, List<Line> possibleIntersections)
        {
            foreach(Line l in edges)
            {
                if (l.EdgeIntersects(possibleIntersections)) return true;
            }

            return false;
        }

        public static List<Line> UnconnectedEdges(this BuildingElement element, List<BuildingElement> space)
        {
            List<Line> edges = element.Edges();

            List<Line> unconnected = new List<Line>();

            List<Line> allEdges = space.Edges();

            foreach(Line l in edges)
            {
                if (allEdges.Where(x => x.BooleanIntersection(l) != null).ToList().Count < 2)
                    unconnected.Add(l);
            }

            return unconnected;
        }
    }
}
