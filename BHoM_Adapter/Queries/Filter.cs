using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.Queries
{
    public class FilterQuery : IQuery
    {
        Dictionary<string, object> Equalities { get; set; }
    }
}
