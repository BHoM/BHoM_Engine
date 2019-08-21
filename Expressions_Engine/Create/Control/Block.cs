using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace BH.Engine.Expressions
{
    public static partial class Create
    {
        public static BlockExpression Block(List<Expression> expressions)
        {
            return Expression.Block(expressions);
        }

        public static BlockExpression Block(List<ParameterExpression> variables, List<Expression> expressions)
        {
            return Expression.Block(variables, expressions);
        }
    }
}
