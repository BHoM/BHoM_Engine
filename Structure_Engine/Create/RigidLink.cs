using BH.oM.Structure.Elements;
using System.Collections.Generic;
using BH.oM.Structure.Properties;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static RigidLink RigidLink(Node masterNode, IEnumerable<Node> slaveNodes, LinkConstraint constraint = null)
        {

            return new RigidLink
            {
                MasterNode = masterNode,
                SlaveNodes = slaveNodes.ToList(),
                Constraint = constraint == null ? LinkConstraintFixed() : constraint
            };

        }


        /***************************************************/
    }
}
