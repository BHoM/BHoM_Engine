using BH.Adapter.Queries;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter
{
    public interface ILink
    {
        bool Create(IEnumerable<object> data, Dictionary<string, string> config = null);

        bool Update(IEnumerable<object> oldData, IEnumerable<object> newData, Dictionary<string, string> config = null);

        List<object> Read(IEnumerable<IQuery> query, Dictionary<string, string> config = null);

        bool Delete(IEnumerable<object> data, Dictionary<string, string> config = null);

        bool Execute(string command, Dictionary<string, string> config = null);
    }
}
