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
        public static List<object> Map(this List<object> list, LambdaExpression func)
        {
            Delegate dlg = func.Compile();
            if (func.Parameters.Count == 1)
            {
                return list.Select(obj => dlg.DynamicInvoke(obj)).ToList();
            }
            else if (func.Parameters.Count == 2 && func.Parameters[1].Type.IsAssignableFrom(typeof(int)))
            {
                return list.Select((obj, i) => dlg.DynamicInvoke(obj, i)).ToList();
            }
            throw new ArgumentException("Function is not a valid mapping function", "func");
            
        }
    }
}
