using BH.oM.Structural.Elements;
using System.Collections.Generic;
using BH.oM.Structural.Properties;
using System.Linq;

namespace BH.Engine.Structure
{
    public static partial class Create
    {
        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public static RigidLink RigidLink(Node masterNode, IEnumerable<Node> slaveNodes, LinkConstraint constriant = null)
        {
            RigidLink link = new RigidLink { MasterNode = masterNode, SlaveNodes = slaveNodes.ToList() };

            if (constriant != null)
                link.Constraint = constriant;

            return link;
        }


        /***************************************************/
    }
}
