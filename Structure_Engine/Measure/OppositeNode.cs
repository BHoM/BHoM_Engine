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

        public static Node GetOppositeNode(this Bar bar, Node node)
        {
            if (bar.EndNode.BHoM_Guid == node.BHoM_Guid)
                return bar.StartNode;
            else
                return bar.EndNode;
        }

        /***************************************************/
    }

}
