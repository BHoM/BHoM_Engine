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

        public static List<Point> Vertices(this BuildingElement element)
        {
            return element.PanelCurve.IControlPoints();
        }

        public static List<Point> Vertices(this List<BuildingElement> space)
        {
            List<Point> vertexPts = new List<Point>();

            foreach (BuildingElement be in space)
                vertexPts.AddRange(be.Vertices());

            return vertexPts;
        }
    }
}
