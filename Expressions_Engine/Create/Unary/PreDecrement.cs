using BH.oM.Reflection.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BH.Engine.Expressions
{
    public static partial class Create
    {
        [Input("expression","The expression to decrement")]
        [Output("A UnaryExpression that decrements an expression and returns the resulting value")]
        [Description("Creates a UnaryExpression that decrements an expression and returns resulting value")]
        public static UnaryExpression PreDecrement(Expression expression)
        {
            return Expression.PreDecrementAssign(expression);
        }
    }
}
