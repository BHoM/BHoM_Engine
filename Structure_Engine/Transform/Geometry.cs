using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Structure
{
    public static partial class Transform
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static void SetGeometry(this Bar bar, Line line)
        {
            bar.StartNode.Point = line.Start;
            bar.EndNode.Point = line.End;
        }

        /***************************************************/

        public static void SetGeometry(this Node node, Point point)
        {
            node.Point = point;
        }

    }
}
