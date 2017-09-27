using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Structure
{
    public static partial class Verify
    {

        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static bool IsConstrained(this Node node)
        {
            return node.Constraint != null;
        }

        /***************************************************/

    }
}
