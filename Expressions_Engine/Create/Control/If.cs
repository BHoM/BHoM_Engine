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
        public static ConditionalExpression If(Expression condition, Expression ifTrue, Expression ifFalse = null)
        {
            if (ifFalse == null)
            {
                return Expression.IfThen(condition, ifTrue);
            }
            else
            {
                return Expression.IfThenElse(condition, ifTrue, ifFalse);
            }
        }
    }
}
