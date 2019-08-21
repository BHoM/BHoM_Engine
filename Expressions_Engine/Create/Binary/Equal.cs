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
        public static BinaryExpression Equal(Expression left, Expression right)
        {
            return Expression.Equal(left, right);
        }
    }
}
