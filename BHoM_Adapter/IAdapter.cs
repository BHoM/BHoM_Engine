using BH.Adapter.Queries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter
{
    public interface IAdapter
    {
        bool Push(IEnumerable<object> objects, string key = "", Dictionary<string, string> config = null);

        IList Pull(IEnumerable<IQuery> query, Dictionary<string, string> config = null);

        int Update(FilterQuery filter, Dictionary<string, object> changes, Dictionary<string, string> config = null);

        int Delete(FilterQuery filter, Dictionary<string, string> config = null);

        bool Execute(string command, Dictionary<string, object> parameters = null, Dictionary<string, string> config = null);

        List<string> ErrorLog { get; set; }
    }
}
