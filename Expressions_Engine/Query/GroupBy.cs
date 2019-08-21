using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Expressions.Query
{
    public static partial class Query
    {
        public static Dictionary<object, List<object>> GroupBy(this List<object> list, LambdaExpression func)
        {
            Delegate dlg = func.Compile();
            return list.GroupBy(obj => dlg.DynamicInvoke(obj)).ToDictionary(group => group.Key, group => group.ToList());
        }
    }
}
