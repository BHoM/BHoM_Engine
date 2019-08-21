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
        public static Delegate Compile(LambdaExpression expr)
        {
            return expr.Compile();
        }
    }
}
