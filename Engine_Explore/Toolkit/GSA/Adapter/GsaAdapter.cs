using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine_Explore.BHoM.Base;
using Engine_Explore.Adapter.Link;
using Engine_Explore.BHoM.Structural.Elements;
using System.Collections;

namespace Engine_Explore.Adapter
{
    public partial class GsaAdapter : IAdapter
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public List<string> ErrorLog { get; set; } = new List<string>();


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public GsaAdapter(string filePath = "")
        {
            m_Link = new GSALink(filePath);
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/


        /*******************************************/
        /****  Private Fields                   ****/
        /*******************************************/

        private GSALink m_Link;
        Dictionary<Type, IList> m_PulledObjects;
    }
}
