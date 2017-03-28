using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.Base
{
    public interface IAdapter
    {
        bool Push(IEnumerable<object> data, bool overwrite = true, string config = "");

        IList Pull(string query, string config = "");

        bool Delete(string filter = "", string config = "");

        bool Execute(string command, string config = "");

        List<string> ErrorLog { get; set; }
    }
}
