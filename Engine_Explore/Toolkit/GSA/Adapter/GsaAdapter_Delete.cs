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
        public bool Delete(string filter = "", string config = "")
        {
            if (filter.Count() == 0)
                return DeleteAll(config);
            else
            {
                // Need to identify that we want to delete tags
                // Delete Element with tag, Remove tag from element with multiple tags

                //Other types of delete filters
                return true;
            }
        }

        /*******************************************/

        public bool DeleteAll(string config = "")
        {
            return m_Link.DeleteAll();
        }
    }
}
