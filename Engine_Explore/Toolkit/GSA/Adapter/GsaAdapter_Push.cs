using Engine_Explore.BHoM.Structural.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BHE = Engine_Explore.Engine;

namespace Engine_Explore.Adapter
{
    public partial class GsaAdapter 
    {
        public bool Push(IEnumerable<object> data, bool overwrite = true, string config = "")
        {
            bool ok = true;

            foreach (var group in Engine.Reflection.Types.GroupByType(data))
            {
                string typeName = group.Key.Name;

                switch (typeName)
                {
                    case "Node":
                        ok &= PushNodes(group.Value as List<Node>, overwrite, config);
                        break;
                    default:
                        ok = false;
                        break;
                }
            }

            return ok;
        }

        /*******************************************/

        public bool PushNodes(IEnumerable<Node> nodes, bool overwrite = true, string config = "")
        {
            string CustomKey = Engine.Convert.GsaElement.CustomKey;
            int highestIndex = m_Link.PullInt("HIGHEST, NODE") + 1;
            bool ok = true;

            foreach (Node node in nodes)
            {
                string id = highestIndex.ToString();
                if (node.CustomData.ContainsKey(CustomKey))
                {
                    id = node.CustomData[CustomKey].ToString();
                }
                else
                {
                    node.CustomData[CustomKey] = highestIndex;
                    highestIndex++;
                }

                ok &= m_Link.Execute(BHE.Convert.GsaCommand.Write(node, id));
            }

            return ok;
        }

    }
}
