using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine_Explore.BHoM.Base;
using Engine_Explore.Adapter.Link;


namespace Engine_Explore.Adapter
{
    public partial class GsaAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public string CustomKey { get; set; } = "GSA_id";


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public GsaAdapter(string filePath = "")
        {
            m_Link = new GSALink(filePath);

            PullFunctions = new Dictionary<string, PullFunction>
            {
                {"Node", PullNodes }
            };

            PushFunctions = new Dictionary<string, PushFunction>
            {
                {"Node", PushNodes }
            };

            DeleteFunctions = new Dictionary<string, ExecuteFunction>
            {
                {"", DeleteAll }
            };
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        // See rest of the partial class in Gsa_Adapter_Pull, GsaAdapter_Push, GsaAdapter_Delete , and GsaADapter_Execute


        /*******************************************/
        /****  Private Fields                   ****/
        /*******************************************/

        private GSALink m_Link;
    }
}
