using Engine_Explore.BHoM.Structural.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHE = Engine_Explore.Engine;

namespace Engine_Explore.Adapter
{
    public partial class GsaAdapter 
    {
        public bool Push(IEnumerable<object> data, string tag = "", string config = "")
        {
            bool ok = true;

            foreach (var group in Engine.Reflection.Types.GroupByType(data))
            {
                string typeName = group.Key.Name;

                switch (typeName)
                {
                    case "Node":
                        ok &= PushNodes(group.Value as List<Node>, tag, config);
                        break;
                    default:
                        ok = false;
                        break;
                }
            }

            return ok;
        }

        /*******************************************/

        public bool PushNodes(IEnumerable<Node> nodes, string tag = "", string config = "")
        {
            // Delete all nodes with the same tag
            Delete("{Type: \"Node\", Tag: \"" + tag + "\"}");

            // Pull the remaining nodes
            Type nodeType = typeof(Node);
            List<Node> pulled = PullNodes();
            m_PulledObjects[nodeType] = pulled;

            // Stich the new nodes with the existing model
            IEnumerable<Node> stiched = BHE.Sets.Stitch.As2Bs(nodes, pulled);

            // Push the stiched nodes to Gsa
            string CustomKey = Engine.Convert.GsaElement.CustomKey;
            int highestIndex = m_Link.PullInt("HIGHEST, NODE") + 1;
            bool ok = true;

            foreach (Node node in stiched)
            {
                // Make sure the Gsa index is stored in the node 
                if (!node.CustomData.ContainsKey(CustomKey))
                    node.CustomData[CustomKey] = highestIndex++;

                // Send the node to Gsa
                ok &= m_Link.Execute(BHE.Convert.GsaCommand.Write(node, node.CustomData[CustomKey].ToString()));
            }

            return ok;
        }

    }
}
