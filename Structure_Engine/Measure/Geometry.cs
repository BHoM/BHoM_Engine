using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Structure
{
    public static partial class Measure
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static IBHoMGeometry GetGeometry(this Bar bar)
        {
            return new Line(bar.StartNode.Point, bar.EndNode.Point);
        }

        /***************************************************/

        public static IBHoMGeometry GetGeometry(this Node node)
        {
            return node.Point;
        }

        /***************************************************/


    }
}
