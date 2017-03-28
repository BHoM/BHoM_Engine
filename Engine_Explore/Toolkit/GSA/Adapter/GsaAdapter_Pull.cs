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
        public IList Pull(string query, string config = "")
        {
            switch (query)
            {
                case "Node":
                    return PullNodes(query, config);
                default:
                    return new List<object>();
            }
        }

        /*******************************************/

        public List<Node> PullNodes(string query, string config = "")
        {
            int highestIndex = m_Link.PullInt("HIGHEST, NODE");
            return m_Link.PullNodes(Enumerable.Range(0, highestIndex+1)).Select(x => BHE.Convert.GsaElement.Read(x)).ToList();
        }

    }
}
