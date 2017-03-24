using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine_Explore.BHoM.Base
{
    public interface ILink
    {
        bool Push(IEnumerable<object> data, bool overwrite = true, string config = "");

        bool Push(IEnumerable<object> data, out object result, bool overwrite = true, string config = "");

        List<object> Pull(List<string> queries, string config = "");

        bool Delete(string filter = "", string config = "");

        bool Execute(string command, string config = "");
    }
}
