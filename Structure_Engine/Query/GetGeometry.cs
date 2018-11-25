using BH.oM.Geometry;
using BH.oM.Structure.Elements;
using System.Collections.Generic;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Query
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Point GetGeometry(this Node node)
        {
            return node.Position;
        }

        /***************************************************/

        public static ICurve GetGeometry(this Bar bar)
        {
            return bar.Centreline();
        }

        /***************************************************/

        public static ICurve GetGeometry(this Edge edge)
        {
            return edge.Curve;
        }
        
        /***************************************************/
    }
}
