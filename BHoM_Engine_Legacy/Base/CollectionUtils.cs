using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.oM.Base
{
    public static class CollectionUtils
    {

        public static string CollectionToString(IEnumerable data)
        {
            string result = "[";
            foreach (object value in data)
            {
                result += value + ",";
            }
            return result.Trim(',') + "]";
        }
    }
}
