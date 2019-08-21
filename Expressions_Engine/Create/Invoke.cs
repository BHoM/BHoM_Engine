using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Expressions
{
    public static partial class Create
    {
        public static InvocationExpression Invoke(Expression lambda, List<Expression> arguments = null)
        {
            if (arguments == null) arguments = new List<Expression>();
            return Expression.Invoke(lambda, arguments);
        }
    }
}
