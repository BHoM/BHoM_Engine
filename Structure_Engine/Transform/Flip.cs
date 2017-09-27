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

        public static void FlipNodes(this Bar bar)
        {
            Node tempNode = bar.StartNode;
            bar.StartNode = bar.EndNode;
            bar.EndNode = tempNode;
        }

        /***************************************************/
    }
}
