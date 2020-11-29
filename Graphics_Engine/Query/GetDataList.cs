using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Graphics
{
    public static partial class Query
    {
        public static List<object> GetDataList(object obj)
        {
            List<object> list = new List<object>();
            if (obj is IEnumerable<object>)
            {
                var enumerator = ((IEnumerable<object>)obj).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current != null)
                        list.Add(enumerator.Current);
                }
            }
            return list;
        }
    }
}
