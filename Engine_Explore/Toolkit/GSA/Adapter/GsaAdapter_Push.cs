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
        public bool PushNodes(IEnumerable<object> nodes, bool overwrite = true, string config = "")
        {
            int highestIndex = m_Link.PullInt("HIGHEST, NODE");
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
