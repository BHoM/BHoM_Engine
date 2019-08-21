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
        public static NewArrayExpression NewArray(Type type, List<Expression> values = null)
        {
            if (values == null) values = new List<Expression>();
            return Expression.NewArrayInit(type, values);
        }
    }
}
