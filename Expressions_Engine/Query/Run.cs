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
        public static object Run(this LambdaExpression expr, List<object> args = null)
        {
            return Run(expr.Compile());
        }

        public static object Run(this Delegate expr, List<object> args = null)
        {
            if (args == null) args = new List<object>();
            return expr.DynamicInvoke(args.ToArray());
        }
    }
}
