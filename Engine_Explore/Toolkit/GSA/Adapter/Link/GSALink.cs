using Interop.gsa_8_7;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.Adapter.Link
{
    public partial class GSALink
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public string Filename { get; }

        public List<string> ErrorLog { get; }


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public GSALink()
        {
            m_Gsa = new ComAuto();
            ErrorLog = new List<string>();
        }

        /***************************************************/

        public GSALink(string filePath) : this()
        {
            short result;
            if (filePath != "")
                result = m_Gsa.Open(filePath);
            else
                result = m_Gsa.NewFile();
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Execute(string command)
        {
            var result = m_Gsa.GwaCommand(command);

            if ((int)result != 1)
            {
                ErrorLog.Add("Application of command " + command + " error. Invalid arguments?");
                return false;
            }

            return true;
        }

        /***************************************************/

        public List<GsaNode> PullNodes(List<int> indices)
        {
            GsaNode[] nodes;
            m_Gsa.Nodes(indices.ToArray(), out nodes);

            return nodes.ToList();
        }

        /***************************************************/

        public int PullInt(string query)
        {
            return m_Gsa.GwaCommand(query);
        }

        /***************************************************/

        public bool DeleteAll()
        {
            return m_Gsa.Delete("RESULTS") == 0;
        }

        /*******************************************/
        /****  Private Fields                   ****/
        /*******************************************/

        private ComAuto m_Gsa;
    }
}

