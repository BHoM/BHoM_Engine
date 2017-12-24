using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using System.Collections.Generic;

namespace BH.Engine.Structure
{
    public static partial class Query
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Point GetCentroid(this oM.Structural.Elements.MeshFace meshFace)
        {
            List<Point> pts = new List<Point>(4);

            foreach (Node n in meshFace.Nodes)
                pts.Add(n.Point);

            return pts.GetAverage();
        }

        /***************************************************/


    }
}
