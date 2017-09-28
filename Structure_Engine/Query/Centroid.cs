using BH.Engine.Geometry;
using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Structure
{
    public static partial class Query
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static Point GetCentroid(this oM.Structural.Elements.PanelFace panelFace)
        {
            List<Point> pts = new List<Point>(4);

            foreach (Node n in panelFace.Nodes)
                pts.Add(n.Point);

            return pts.GetAverage();
        }

        /***************************************************/


    }
}
